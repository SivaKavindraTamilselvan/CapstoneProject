import { HttpClient, HttpParams } from "@angular/common/http";
import { Injectable } from "@angular/core";
import { NotificationFilterModel } from "../models/notification/notification.filter";
import { BaseURL } from "../environment";
import { NotificationResponseModel } from "../models/notification/notification.model";
import { PagedResponse } from "../models/paged-response.model";

@Injectable({
    providedIn: "root"
})
export class NotificationService {
    constructor(private http: HttpClient) {

    }
    getNotification(filter: NotificationFilterModel) {
        let url = BaseURL + `/Notification/notifcation-list`;
        let params = new HttpParams();
        Object.entries(filter).forEach(([key, value]) => {
            if (value !== null && value !== undefined && value !== '') {
                params = params.set(key, value.toString());
            }
        });
        return this.http.get<PagedResponse<NotificationResponseModel>>(url, { params });
    }
    updateNotification(id: number) {
        const url = BaseURL + `/Notification/notifcation/${id}`;
        return this.http.put<NotificationResponseModel>(url, {});
    }

    deleteNotification(id: number) {
        const url = BaseURL + `/Notification/notification/${id}`;
        return this.http.delete(url, { responseType: 'text' });
    }

    clearAllNotifications() {
        const url = BaseURL + `/Notification/notification/clear`;
        return this.http.delete(url, { responseType: 'text' });
    }
}