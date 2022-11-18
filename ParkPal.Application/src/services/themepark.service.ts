import axios, {AxiosError, AxiosResponse} from "axios";
import store from "../store";
import router from "../router";
import {authHeader} from "@/helpers/authHeaders.helper";
import WaitTimeFilter from "@/models/store/WaitTimeFilter";
import AttractionsRequest from "@/models/api/requests/themepark/AttractionsRequest";
import {RefresherCustomEvent} from "@ionic/vue";
import Park from "@/models/api/Park";
import Attraction from "@/models/api/Attraction";
import transformers from "@/transformers";

const instance = axios.create({
    baseURL: 'http://192.168.1.96:5002/themepark/',
    timeout: 10000
});

function getDestinations() {
    return instance.get(`destinations`, {
        headers: authHeader()
    });
}

function getParks(destinationId: string) {
    return instance.get(`destinations/${destinationId}/parks`, {
        headers: authHeader()
    });
}

function getAttractions(parkId: string, request: AttractionsRequest) {
    return instance.post(`parks/${parkId}/attractions`, request, {
        headers: authHeader()
    });
}

function getAttractionsWithImages(parkId: string, request: AttractionsRequest): Promise<Array<Attraction>> {
    return new Promise((resolve, reject) => {
        getAttractions(parkId, request).then(
            (response: AxiosResponse<Park>) => {
                transformers.transformApiAttractionsArrayToInternalAttractionsArray(response.data.attractions).then((attractions: Array<Attraction>) => {
                    console.log('Service Attractions', attractions);
                    resolve(attractions);
                })
            })
            .catch((error: AxiosError) => {
                reject(error);
            });
    })
}

//
// function getById(id: number) {
//     return instance.get(`${config.apiUrl}/diary/${id}`, {
//         headers: authHeader()
//     }).then((response) => {
//         console.log('Get diary by ID called', response);
//     });
// }
//
// function getForUser() {
//     return instance.post(`${config.apiUrl}/diary/GetForUser`, {}, {
//         headers: authHeader()
//     });
// }
//
// function getByDateForUser(date: Date) {
//     return instance.post(`${config.apiUrl}/diary/GetDiaryByDateForUser/`, {
//         date
//     }, {
//         headers: authHeader()
//     });
// }
//
// function update(diary: Diary) {
//     return instance.put(`${config.apiUrl}/diary/${diary.diaryId}`, JSON.stringify(diary), {
//         headers: authHeader()
//     }).then((response) => {
//         console.log('Called diary update.', response);
//     });
// }
//
// // prefixed function name with underscore because delete is a reserved word in javascript
// function _delete(id: number) {
//
//     return instance.delete(`${config.apiUrl}/diary/${id}`, {
//         headers: authHeader()
//     }).then((response) => {
//         console.log('Called diary delete?', response);
//     });
// }
//
// // Get a set of Products from the API endpoint using a search barcode.
// function getSymptomsBySearchTerm(searchTerm: string) {
//     return instance.post(`${config.apiUrl}/Symptom/Search`,
//         {
//             searchTerm: searchTerm
//         },
//         {
//             headers: authHeader()
//         })
//         .then(response => {
//             if(response.status == 200) {
//                 return response.data;
//             }else{
//                 // FeedbackEventBus.$emit(FeedbackEvents.Error, new Feedback({ message: "Product not found." }));
//                 return null;
//             }
//         });
// }
//
// function addSymptomToDiary(request: AddSymptomRequest) {
//     return instance.post(`${config.apiUrl}/Diary/AddSymptom`,
//         request, {
//             headers: authHeader()
//         });
// }
//
// function removeEntryFromDiary(entryId: number) {
//     return instance.delete(`${config.apiUrl}/Diary/RemoveEntryFromDiary/${entryId}`, {
//         headers: authHeader()
//     }).then((response) => {
//         console.log('Called diary delete?', response);
//     });
// }



export const themeparkService = {
    getDestinations,
    getParks,
    getAttractions,
    getAttractionsWithImages
};