import { HttpClient } from "@angular/common/http";
import { Injectable } from "@angular/core";
import { BaseURL } from "../environment";
import { PagedResponse } from "../models/paged-response.model";
import { AdminUserModel } from "../models/admin-user.model";

@Injectable({
    providedIn: "root"
})
export class AdminUserService {
    constructor(private http: HttpClient) {

    }
    getAdminUser() {
        let url = BaseURL + "/Admin/GetAdminUser";
        return this.http.get<PagedResponse<AdminUserModel>>(url);
    }
    activateAdminUser(adminId: number) {
        let url = `${BaseURL}/Admin/admin-users/${adminId}/activate`;
        return this.http.put(url,{});
    }
    deactivateAdminUser(adminId: number) {
        let url = `${BaseURL}/Admin/admin-users/${adminId}/deactivate`;
        return this.http.put(url,{});
    }
    getActiveAdminUser() {
        let url = BaseURL + "/Admin/GetAdminUser?status=true";
        return this.http.get<PagedResponse<AdminUserModel>>(url);
    }
    getDeactiveAdminUser() {
        let url = BaseURL + "/Admin/GetAdminUser?status=false";
        return this.http.get<PagedResponse<AdminUserModel>>(url);
    }
}