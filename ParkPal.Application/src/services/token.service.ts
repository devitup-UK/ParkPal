import axios, {AxiosResponse} from "axios";
import { authHeader } from '../helpers/authHeaders.helper';

const instance = axios.create({
    baseURL: 'http://192.168.1.96:5002/token/',
    timeout: 10000
});

function verify(token: string) {
    return instance.post(`verify`, {
        token
    });
}

function generate() {
    return instance.post(`generate`).then((response: AxiosResponse<{ token: string }>) => {
        return response.data.token;
    }).catch((error) => {
        console.error(error);
        return undefined;
    });
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



export const tokenService = {
    verify,
    generate
};