import { HttpClient, HttpParams } from "@angular/common/http";
import { Injectable } from "@angular/core";
import { RegisterVendorUserModel } from "../models/authentication/register-vendor-user.model";
import { VendorUserFilter } from "../models/vendor/vendor-user/vendor-user.filter";
import { Observable } from "rxjs";
import { PagedResponse } from "../models/paged-response.model";
import { VendorUserModel } from "../models/vendor/vendor-user/response-vendor-user.model";
import { BaseURL } from "../environment";

@Injectable({
    providedIn: "root"
})
export class VendorUserService {
    constructor(private http: HttpClient) {

    }
    getAdminUser(filter: VendorUserFilter): Observable<PagedResponse<VendorUserModel>> {
        let url = BaseURL + "/Vendor/vendor-user";
        let params = new HttpParams();
        Object.entries(filter).forEach(([key, value]) => {
            if (value !== null && value !== undefined && value !== '') {
                params = params.set(key, value.toString());
            }
        });
        return this.http.get<PagedResponse<VendorUserModel>>(url, { params });
    }
    getAdminUserDetail(id: number) {
        let url = BaseURL + `/Vendor/vendor-user/${id}`;
        return this.http.get<VendorUserModel>(url, {});
    }
    activateAdminUser(adminId: number) {
        let url = `${BaseURL}/Vendor/vendor-users/${adminId}/activate`;
        return this.http.put(url, {});
    }
    deactivateAdminUser(adminId: number) {
        let url = `${BaseURL}/Vendor/vendor-users/${adminId}/deactivate`;
        return this.http.put(url, {});
    }
    getActiveAdminUser(filter: VendorUserFilter): Observable<PagedResponse<VendorUserModel>> {
        let url = BaseURL + "/Vendor/vendor-user?status=true";
        let params = new HttpParams();
        Object.entries(filter).forEach(([key, value]) => {
            if (value !== null && value !== undefined && value !== '') {
                params = params.set(key, value.toString());
            }
        });
        return this.http.get<PagedResponse<VendorUserModel>>(url, { params });
    }
    getDeactiveAdminUser(filter: VendorUserFilter): Observable<PagedResponse<VendorUserModel>> {
        let url = BaseURL + "/Vendor/vendor-user?status=false";
        let params = new HttpParams();
        Object.entries(filter).forEach(([key, value]) => {
            if (value !== null && value !== undefined && value !== '') {
                params = params.set(key, value.toString());
            }
        });
        return this.http.get<PagedResponse<VendorUserModel>>(url, { params });
    }
}