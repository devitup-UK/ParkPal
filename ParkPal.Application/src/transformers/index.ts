import AttractionTimer from "@/models/api/AttractionTimer";
import TimerWithAttraction from "@/models/api/TimerWithAttraction";
import Destination from "@/models/api/Destination";
import Park from "@/models/api/Park";
import Attraction from "@/models/api/Attraction";

function transformApiDestinationArrayToInternalDestinationArray(destinations: Array<Destination>) {
    return new Promise((resolve) => {
        const transformedDestinations: Array<Destination> = [];

        // Transform our destination from the API to an internal one.
        destinations.forEach((destination: Destination) => {
            // Create the destination to add, so we can check if the image we are working with has a valid image.
            const destinationToAdd = new Destination(destination);
            // destinationToAdd.checkImageExists().then((exists) => {
            //    destinationToAdd.hidden = !exists;
            //
            //    if(!destinationToAdd.hidden) {
            //        destinationToAdd.parks.forEach((park: Park) => {
            //            park.checkImageExists().then(parkImageExists => {
            //                park.hidden = !parkImageExists;
            //            })
            //        })
            //    }
            //
            // });

            transformedDestinations.push(destinationToAdd);
        });

        resolve(transformedDestinations);
    });
}

function transformApiParksArrayToInternalParksArray(parks: Array<Park>) {
    const transformedParks: Array<Park> = [];

    // Transform our destination from the API to an internal one.
    parks.forEach((park: Park) => {
        const transformedPark = new Park(park);

        transformedParks.push(transformedPark);
    });

    return transformedParks;
}

function transformApiAttractionsArrayToInternalAttractionsArray(attractions: Array<Attraction>): Promise<Array<Attraction>> {
    return new Promise((resolve) =>  {
        const transformedAttractions: Array<Attraction> = [];

        // Transform our destination from the API to an internal one.
        attractions.forEach((attraction: Attraction) => {
            const attractionToAdd = new Attraction(attraction);
            transformedAttractions.push(attractionToAdd);
        });

        resolve(transformedAttractions);
    })

}

function transformApiAttractionTimerArrayToInternalAttractionTimerArray(timers: Array<AttractionTimer>) {
    const transformedTimers: Array<AttractionTimer> = [];

    // Transform our attraction timer to an internal timer.
    timers.forEach((timer: AttractionTimer) => {
        transformedTimers.push(new AttractionTimer(timer));
    });

    return transformedTimers;
}

function transformApiTimerWithAttractionArrayToInternalTimerWithAttractionArray(timers: Array<TimerWithAttraction>) {
    const transformedTimers: Array<TimerWithAttraction> = [];

    // Transform our attraction timer to an internal timer.
    timers.forEach((timer: TimerWithAttraction) => {
        transformedTimers.push(new TimerWithAttraction(timer));
    });

    return transformedTimers;
}


export default {
    transformApiDestinationArrayToInternalDestinationArray,
    transformApiParksArrayToInternalParksArray,
    transformApiAttractionsArrayToInternalAttractionsArray,
    transformApiAttractionTimerArrayToInternalAttractionTimerArray,
    transformApiTimerWithAttractionArrayToInternalTimerWithAttractionArray
};