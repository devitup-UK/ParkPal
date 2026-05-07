using ParkPal.API.Services.Interfaces;
using ParkPal.Common.API.Models.Dtos;
using ParkPal.Common.Data.Interfaces;
using ParkPal.Common.Helpers;

public class PlanningService(IAttractionHistoryRepository historyRepository, IItineraryRepository itineraryRepository, IParkRepository parkRepository)
    : IPlanningService
{
    public async Task<SavedPlanDto> GenerateItineraryAsync(GeneratePlanRequestDto request)
    {
        // 1. Setup the Day
        var currentTime = TimeSpan.Parse(request.ArrivalTime); 
        var endOfDay = TimeSpan.Parse(request.DepartureTime);
        var generatedItems = new List<PlanItemDto>();
        var flexibleItems = new List<FlexibleItemDto>();
        var bookedSlots = new List<(TimeSpan Start, TimeSpan End)>();

        // ⭐️ We need to track where you are physically standing!
        var park = await parkRepository.GetParkDataAsync(request.ParkId);
        
        // ⭐️ THE FIX: Save the Park's central coordinates to act as our safety net!
        var parkLat = park?.Latitude ?? 0.0; 
        var parkLon = park?.Longitude ?? 0.0;
        
        // Fallback to 0.0 if null to keep the math happy
        var currentLat = parkLat; 
        var currentLon = parkLon;

        // 2. Fetch Historical Wait Times & Coordinates
        var dayOfWeek = request.TripDate.DayOfWeek;
        var mustDoHistory = await historyRepository.GetAveragesForDayAsync(request.MustDoAttractionIds, dayOfWeek);
        var niceToHaveHistory = await historyRepository.GetAveragesForDayAsync(request.NiceToHaveAttractionIds, dayOfWeek);
        
        // Fetch your double precision Lat/Lons from the DB!
        var attractions = await parkRepository.GetAttractionsWithLocationsForPark(request.ParkId);

        // 3. Set the Anchors (Meals & Fixed-Time Shows) 🍔🎭
        var anchors = new List<AnchorEvent>();

        // --- PASS 1: The "Hard" Anchors ---
        foreach (var meal in request.PlannedMeals)
        {
            var start = TimeSpan.Parse(meal.Time);
            var mealLocation = attractions.FirstOrDefault(l => l.AttractionId == meal.RestaurantId);
            
            anchors.Add(new AnchorEvent 
            { 
                StartTime = start, 
                EndTime = start.Add(TimeSpan.FromMinutes(60)), 
                Title = meal.RestaurantName, 
                Subtitle = "Meal Break", 
                Icon = "fork.knife", 
                ColorHex = "#FF9500",
                Lat = mealLocation?.Latitude ?? parkLat,
                Lon = mealLocation?.Longitude ?? parkLon,
                AttractionId = meal.RestaurantId
            });
        }

        var strictShows = request.SelectedShows.Where(s => !string.IsNullOrEmpty(s.PreferredTime));
        foreach (var show in strictShows)
        {
            var actualShowTime = TimeSpan.Parse(show.PreferredTime);

            // ⭐️ THE ENVELOPE: 15 mins queueing + 60 min show + 5 min exit
            var arrivalTime = actualShowTime.Subtract(TimeSpan.FromMinutes(15));
            var exitTime = actualShowTime.Add(TimeSpan.FromMinutes(65));

            var showLocation = attractions.FirstOrDefault(a => a.AttractionId == show.ShowId);

            anchors.Add(new AnchorEvent
            {
                StartTime = arrivalTime,
                EndTime = exitTime,
                Title = showLocation?.Name ?? "Showtime",
                Subtitle = $"Show starts at {DateTime.Today.Add(actualShowTime):hh:mm tt}",
                Icon = "theatermasks.fill",
                ColorHex = "#AF52DE",
                Lat = showLocation?.Latitude ?? parkLat,
                Lon = showLocation?.Longitude ?? parkLon,
                AttractionId = show.ShowId
            });
        }

        // --- PASS 2: The "Anytime" Best Fit Search ---
        var anytimeShows = request.SelectedShows.Where(s => string.IsNullOrEmpty(s.PreferredTime));
        
        foreach (var show in anytimeShows)
        {
            var showLocation = attractions.FirstOrDefault(a => a.AttractionId == show.ShowId);
            
            // Note: Ensure your IParkRepository has a method like GetShowtimesAsync to pull the times for this specific date!
            var availableTimes = show.ValidTimes.Count != 0
                ? show.ValidTimes
                : await parkRepository.GetShowtimesAsync(show.ShowId, request.TripDate);
            
            bool wasScheduled = false;

            if (availableTimes != null && availableTimes.Any())
            {
                foreach (var timeStr in availableTimes)
                {
                    if (!TimeSpan.TryParse(timeStr, out var startTime)) continue;

                    // Ensure the show falls within the user's actual park day
                    if (startTime < currentTime || startTime > endOfDay) continue;

                    // ⭐️ Create the theoretical envelope
                    var potentialStart = startTime.Subtract(TimeSpan.FromMinutes(15));
                    var potentialEnd = startTime.Add(TimeSpan.FromMinutes(65));

                    // ⭐️ OVERLAP CHECK: Does this slot crash into a meal or a strict show?
                    bool hasOverlap = anchors.Any(a => 
                        potentialStart < a.EndTime && potentialEnd > a.StartTime
                    );

                    if (!hasOverlap)
                    {
                        // 🎯 Found a gap! Add it to the strict timeline
                        anchors.Add(new AnchorEvent
                        {
                            StartTime = potentialStart,
                            EndTime = potentialEnd,
                            Title = showLocation?.Name ?? "Showtime",
                            Subtitle = $"Scheduled for you: {DateTime.Today.Add(startTime):hh:mm tt} ✨",
                            Icon = "theatermasks.fill",
                            ColorHex = "#AF52DE",
                            Lat = showLocation?.Latitude ?? parkLat,
                            Lon = showLocation?.Longitude ?? parkLon,
                            AttractionId = show.ShowId
                        });
                        
                        wasScheduled = true;
                        break; // Stop checking times for this specific show!
                    }
                }
            }

            // 🔄 FALLBACK: If no times fit, or no times exist (like the Arts Programme)
            if (!wasScheduled)
            {
                flexibleItems.Add(new FlexibleItemDto
                {
                    Id = Guid.NewGuid(),
                    AttractionId = show.ShowId,
                    Title = showLocation?.Name ?? "Showtime",
                    Subtitle = "Anytime Experience ✨",
                    IconName = "star.fill",
                    ColorHex = "#EA75FA"
                });
            }
        }

        // ⭐️ Sort all anchors (both strict and newly auto-slotted) chronologically!
        anchors = anchors.OrderBy(a => a.StartTime).ToList();
        
        // ⭐️ THE FIX: Vaporize any duplicates sent from the iOS App
        var pendingMustDos = request.MustDoAttractionIds.Distinct().ToList();
        var pendingNiceToHaves = request.NiceToHaveAttractionIds.Distinct().ToList();
        
        // ⭐️ Break Tracker!
        int activitiesInARow = 0;

        // 4. THE CHRONOLOGICAL LOOP ⏳
        while (currentTime < endOfDay)
        {
            var nextAnchor = anchors.FirstOrDefault();
            
            int walkToAnchor = nextAnchor != null 
                ? WalkingCalculator.GetWalkingTimeMinutes(currentLat, currentLon, nextAnchor.Lat, nextAnchor.Lon) 
                : 0;

            // SCENARIO A: An Anchor is approaching!
            if (nextAnchor != null)
            {
                if (currentTime.Add(TimeSpan.FromMinutes(walkToAnchor + 15)) >= nextAnchor.StartTime)
                {
                    bookedSlots.Add((nextAnchor.StartTime, nextAnchor.EndTime));
                    generatedItems.Add(new PlanItemDto
                    {
                        Id = Guid.NewGuid(),
                        Time = DateTime.Today.Add(nextAnchor.StartTime).ToString("hh:mm tt"),
                        Title = nextAnchor.Title,
                        Subtitle = nextAnchor.Subtitle,
                        Icon = nextAnchor.Icon,
                        ColorHex = nextAnchor.ColorHex,
                        AttractionId = nextAnchor.AttractionId
                    });

                    currentTime = nextAnchor.EndTime;
                    currentLat = nextAnchor.Lat; 
                    currentLon = nextAnchor.Lon;
                    anchors.Remove(nextAnchor);
                    
                    // ⭐️ THE FIX: Reset the counter! A meal/show counts as a break.
                    activitiesInARow = 0; 
                    continue;
                }
            }

            // SCENARIO B: We have free time! Let's hit a Ride.
            var pendingRides = pendingMustDos.Any() ? pendingMustDos : pendingNiceToHaves;

            if (pendingRides.Any())
            {
                var closestRide = attractions
                    .Where(a => pendingRides.Contains(a.AttractionId))
                    .OrderBy(a => WalkingCalculator.GetWalkingTimeMinutes(currentLat, currentLon, a.Latitude ?? parkLat, a.Longitude ?? parkLon))
                    .FirstOrDefault();

                if (closestRide == null)
                {
                    pendingRides.Clear();
                    continue;
                }

                var closestRideId = closestRide.AttractionId;
                int walkTimeMins = WalkingCalculator.GetWalkingTimeMinutes(currentLat, currentLon, closestRide.Latitude ?? parkLat, closestRide.Longitude ?? parkLon);
                var arrivalTime = currentTime.Add(TimeSpan.FromMinutes(walkTimeMins));

                var activeHistory = pendingMustDos.Any() ? mustDoHistory : niceToHaveHistory;
                var currentWaitBucket = activeHistory
                    .Where(h => h.AttractionId == closestRideId && h.BucketTime <= arrivalTime)
                    .OrderByDescending(h => h.BucketTime)
                    .FirstOrDefault() 
                    ?? activeHistory.Where(h => h.AttractionId == closestRideId).OrderBy(h => h.BucketTime).FirstOrDefault();

                int waitTimeMins = currentWaitBucket?.AverageWaitTime ?? 25;
                int rideDurationMins = 5; 

                var totalCost = TimeSpan.FromMinutes(walkTimeMins + waitTimeMins + rideDurationMins);
                var proposedEnd = currentTime.Add(totalCost);

                bool fitsBeforeClose = proposedEnd <= endOfDay;
                bool fitsBeforeAnchor = nextAnchor == null || proposedEnd.Add(TimeSpan.FromMinutes(15)) <= nextAnchor.StartTime;

                if (fitsBeforeClose && fitsBeforeAnchor)
                {
                    bookedSlots.Add((currentTime, proposedEnd));
                    
                    generatedItems.Add(new PlanItemDto
                    {
                        Id = Guid.NewGuid(),
                        Time = DateTime.Today.Add(currentTime).ToString("hh:mm tt"),
                        Title = closestRide.Name, 
                        Subtitle = $"{walkTimeMins}m walk • {waitTimeMins}m wait",
                        Icon = pendingMustDos.Any() ? "heart.fill" : "hand.thumbsup.fill",
                        ColorHex = pendingMustDos.Any() ? "#FF3B30" : "#34C759",
                        AttractionId = closestRide.AttractionId,
                    });

                    currentTime = proposedEnd;
                    currentLat = closestRide.Latitude ?? parkLat;
                    currentLon = closestRide.Longitude ?? parkLon;
                    pendingRides.Remove(closestRideId);
                    
                    activitiesInARow++;

                    // ⭐️ THE RULE OF THREE: Inject the break natively on the timeline
                    if (activitiesInARow >= 3)
                    {
                        var breakEnd = currentTime.Add(TimeSpan.FromMinutes(20));
                        
                        // Safety check: Don't take a break if park closes in 5 mins
                        if (breakEnd <= endOfDay) 
                        {
                            bookedSlots.Add((currentTime, breakEnd));
                            generatedItems.Add(new PlanItemDto
                            {
                                Id = Guid.NewGuid(),
                                Time = DateTime.Today.Add(currentTime).ToString("hh:mm tt"),
                                Title = "Quiet Break / Reset ✨",
                                Subtitle = "Find a bench, grab a drink, and recharge.",
                                Icon = "drop.fill", 
                                ColorHex = "#5AC8FA" 
                            });
                            
                            currentTime = breakEnd; 
                        }
                        
                        activitiesInARow = 0; 
                    }

                    continue;
                }
                else
                {
                    if (nextAnchor != null) 
                    {
                        currentTime = nextAnchor.StartTime.Subtract(TimeSpan.FromMinutes(walkToAnchor));
                        continue;
                    }
                    else
                    {
                        pendingRides.Remove(closestRideId);
                        continue; 
                    }
                }
            }

            // ⭐️ THE FIX: The Skipped Dinner Bug!
            if (!pendingRides.Any())
            {
                if (nextAnchor != null)
                {
                    // No rides left, but we have an anchor (meal/show) later! 
                    currentTime = nextAnchor.StartTime.Subtract(TimeSpan.FromMinutes(walkToAnchor));
                    continue;
                }
                else
                {
                    // Out of rides AND out of anchors. The day is officially complete!
                    break; 
                }
            }
        }

        // 5. Inject Free Time (The 90-minute gaps)
        var itemsWithBreaks = new List<PlanItemDto>();
        for (int i = 0; i < generatedItems.Count; i++)
        {
            itemsWithBreaks.Add(generatedItems[i]);

            if (i < generatedItems.Count - 1)
            {
                var currentStartTime = DateTime.Parse(generatedItems[i].Time).TimeOfDay;
                var nextStartTime = DateTime.Parse(generatedItems[i + 1].Time).TimeOfDay;

                var gap = nextStartTime - currentStartTime;
                if (gap.TotalMinutes >= 90)
                {
                    var currentSlot = bookedSlots.FirstOrDefault(b => b.Start == currentStartTime);
                    var freeTimeStart = currentSlot != default ? currentSlot.End : currentStartTime.Add(TimeSpan.FromMinutes(60));
            
                    itemsWithBreaks.Add(new PlanItemDto
                    {
                        Id = Guid.NewGuid(),
                        Time = DateTime.Today.Add(freeTimeStart).ToString("hh:mm tt"),
                        Title = "Free Time / Explore ✨",
                        Subtitle = "Grab a snack, shop, or catch a parade!",
                        Icon = "map.fill", 
                        ColorHex = "#8E8E93" 
                    });
                }
            }
        }
        
        generatedItems = itemsWithBreaks;
        
        // 6. Evening Magic Capstone
        if (generatedItems.Any())
        {
            var eveningMagicStart = endOfDay.Subtract(TimeSpan.FromMinutes(45));

            if (bookedSlots.Any())
            {
                var latestEndTime = bookedSlots.Max(b => b.End);
                if (eveningMagicStart < latestEndTime) 
                {
                    eveningMagicStart = latestEndTime; 
                }
            }

            if (eveningMagicStart < endOfDay)
            {
                generatedItems.Add(new PlanItemDto
                {
                    Id = Guid.NewGuid(),
                    Time = DateTime.Today.Add(eveningMagicStart).ToString("hh:mm tt"),
                    Title = "Evening Magic ✨",
                    Subtitle = "Souvenir shopping, fireworks, or a final snack!",
                    Icon = "sparkles", 
                    ColorHex = "#5E5CE6"
                });
            }
        }

        var finalPlan = new SavedPlanDto
        {
            Id = Guid.NewGuid(),
            Title = $"{park.Name} Adventure", 
            TripDate = request.TripDate,
            DestinationName = request.DestinationName,
            ParkName = park.Name,
            ParkId = park.ParkId,
            ArrivalTime = request.ArrivalTime,
            DepartureTime = request.DepartureTime,
            TotalActivities = generatedItems.Count,
            Items = generatedItems,
            FlexibleItems = flexibleItems,
        };

        return finalPlan;
    }

    private class AnchorEvent
    {
        public TimeSpan StartTime { get; set; }
        public TimeSpan EndTime { get; set; }
        public string Title { get; set; }
        public string Subtitle { get; set; }
        public string Icon { get; set; }
        public string ColorHex { get; set; }
        public double Lat { get; set; }
        public double Lon { get; set; }
        public string? AttractionId  { get; set; }
    }

    public async Task SavePlanAsync(string appUserId, SavedPlanDto request)
    {
        await itineraryRepository.SavePlanAsync(appUserId, request);
    }
}