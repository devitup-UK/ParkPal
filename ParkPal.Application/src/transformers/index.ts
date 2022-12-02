import NotificationProperties from "@/models/api/NotificationProperties";
import Notification from "@/models/api/Notification";
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

function transformApiAttractionTimerArrayToInternalAttractionTimerArray(timers: Array<NotificationProperties>) {
    const transformedTimers: Array<NotificationProperties> = [];

    // Transform our attraction timer to an internal timer.
    timers.forEach((timer: NotificationProperties) => {
        transformedTimers.push(new NotificationProperties(timer));
    });

    return transformedTimers;
}

function transformApiNotificationWithEntityArrayToInternalNotificationWithEntityArray(timers: Array<Notification>) {
    const transformedTimers: Array<Notification> = [];

    // Transform our attraction timer to an internal timer.
    timers.forEach((timer: Notification) => {
        transformedTimers.push(new Notification(timer));
    });

    return transformedTimers;
}


export default {
    transformApiDestinationArrayToInternalDestinationArray,
    transformApiParksArrayToInternalParksArray,
    transformApiAttractionsArrayToInternalAttractionsArray,
    transformApiAttractionTimerArrayToInternalAttractionTimerArray,
    transformApiNotificationWithEntityArrayToInternalNotificationWithEntityArray
};