import axios, {AxiosResponse} from "axios";
import { authHeader } from '../helpers/authHeaders.helper';
import AttractionTimer from "@/models/api/AttractionTimer";
import CreateNotificationRequest from "@/models/api/requests/notification/CreateNotificationRequest";
import EditNotificationRequest from "@/models/api/requests/notification/EditNotificationRequest";
import EnableDisableNotificationRequest from "@/models/api/requests/notification/EnableDisableNotificationRequest";
import TimerWithAttraction from "@/models/api/TimerWithAttraction";
import GetNotificationsRequest from "@/models/api/requests/notification/GetNotificationsRequest";

const instance = axios.create({
    baseURL: 'http://192.168.1.96:5002/notification/',
    timeout: 10000
});

function getAllNotifications(request: GetNotificationsRequest) {
    return instance.post('', request, {
        headers: authHeader()
    }).then((response: AxiosResponse<Array<TimerWithAttraction>>) => {
        const responseArray: Array<TimerWithAttraction> = [];

        response.data.forEach(timer => {
            responseArray.push(new TimerWithAttraction(timer));
        })

        return responseArray;
    })
}

function createNotification(request: CreateNotificationRequest) {
    return instance.post('create', request, {
        headers: authHeader()
    }).then((response: AxiosResponse<AttractionTimer>) => {
        return new AttractionTimer(response.data);
    })
}

function editNotification(request: EditNotificationRequest) {
    return instance.post('edit', request, {
        headers: authHeader()
    }).then((response: AxiosResponse<AttractionTimer>) => {
        return new AttractionTimer(response.data)
    })
}

function enableNotification(request: EnableDisableNotificationRequest) {
    return instance.post('enable', request, {
        headers: authHeader()
    }).then((response: AxiosResponse<AttractionTimer>) => {
        return new AttractionTimer(response.data);
    })
}

function disableNotification(request: EnableDisableNotificationRequest) {
    return instance.post('disable', request, {
        headers: authHeader()
    }).then((response: AxiosResponse<AttractionTimer>) => {
        return new AttractionTimer(response.data);
    })
}

function deleteNotification(attractionTimerId: number) {
    return instance.delete('delete/' + attractionTimerId, {
        headers: authHeader()
    }).then(() => {
        return true;
    }).catch(() => {
        return false;
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



export const notificationService = {
    getAllNotifications,
    createNotification,
    editNotification,
    enableNotification,
    disableNotification,
    deleteNotification
};