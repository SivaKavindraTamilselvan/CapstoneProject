import { HttpClient, HttpParams } from "@angular/common/http";
import { Injectable } from "@angular/core";
import { BaseURL } from "../environment";
import { PagedResponse } from "../models/paged-response.model";
import { AdminVendorModel } from "../models/admin/vendor/admin-vendor.model";
import { ReviewVendorRequestModel } from "../models/admin/vendor/review-vendor.dto";
import { filter, map, Observable } from "rxjs";
import { AdminVendorFilter } from "../models/admin/vendor/admin-vendor.filter";
import { AdminDeleteVendorModel } from "../models/admin/vendor/delete-vendor.model";
import { AdminVendorUserFilter } from "../models/admin/vendor/vendor-user.filter";
import { AdminVendorUserModel } from "../models/admin/vendor/vendor-user.model";

@Injectable({
    providedIn: "root"
})
export class AdminVendorService {
    constructor(private http: HttpClient) {

    }
    getVendor(filter: AdminVendorFilter): Observable<PagedResponse<AdminVendorModel>> {
        let url = BaseURL + "/AdminVendor/GetVendor";
        let params = new HttpParams();
        Object.entries(filter).forEach(([key, value]) => {
            if (value !== null && value !== undefined && value !== '') {
                params = params.set(key, value.toString());
            }
        });
        return this.http.get<PagedResponse<AdminVendorModel>>(url, { params });
    }
    getVendorDetails(vendorId: number) {
        let url = BaseURL + `/AdminVendor/GetVendor/${vendorId}`;
        return this.http.get<AdminVendorModel>(url, {});
    }
    getPendingVendor(filter: AdminVendorFilter): Observable<PagedResponse<AdminVendorModel>> {
        let url = BaseURL + "/AdminVendor/GetVendor?ApprovalStatusId=1";
        let params = new HttpParams();
        Object.entries(filter).forEach(([key, value]) => {
            if (value !== null && value !== undefined && value !== '') {
                params = params.set(key, value.toString());
            }
        });
        return this.http.get<PagedResponse<AdminVendorModel>>(url, { params });
    }
    getDeletedVendor(filter: AdminVendorFilter): Observable<PagedResponse<AdminVendorModel>> {
        let url = BaseURL + "/AdminVendor/GetVendor?ApprovalStatusId=4";
        let params = new HttpParams();
        Object.entries(filter).forEach(([key, value]) => {
            if (value !== null && value !== undefined && value !== '') {
                params = params.set(key, value.toString());
            }
        });
        return this.http.get<PagedResponse<AdminVendorModel>>(url, { params });
    }
    reviewVendor(request: ReviewVendorRequestModel) {
        let url = BaseURL + "/AdminVendor/ReviewVendor";
        return this.http.put(url, request);
    }
    DeleteVendor(request: AdminDeleteVendorModel) {
        let url = BaseURL + "/AdminVendor/DeleteVendor";
        return this.http.patch(url, request);
    }
    getVendorUser(filter: AdminVendorUserFilter) : Observable<PagedResponse<AdminVendorUserModel>> {
        let url = BaseURL + `/AdminVendor/GetVendorUser`;
        let params = new HttpParams();
        Object.entries(filter).forEach(([key, value]) => {
            if (value !== null && value !== undefined && value !== '') {
                params = params.set(key, value.toString());
            }
        });
        return this.http.get<PagedResponse<AdminVendorUserModel>>(url, { params });
    }
}