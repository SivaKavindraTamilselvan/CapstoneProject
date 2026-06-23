import { HttpClient, HttpParams } from "@angular/common/http";
import { Injectable } from "@angular/core";
import { BaseURL } from "../environment";
import { PagedResponse } from "../models/paged-response.model";
import { AdminVendorModel } from "../models/admin/vendor/admin-vendor.model";
import { ReviewVendorRequestModel } from "../models/admin/vendor/review-vendor.dto";
import { filter, Observable } from "rxjs";
import { AdminVendorFilter } from "../models/vendor/admin-vendor.filter";

@Injectable({
    providedIn: "root"
})
export class AdminVendorService {
    constructor(private http: HttpClient) {

    }
    getVendor(filter : AdminVendorFilter) : Observable<PagedResponse<AdminVendorModel>> {
        let url = BaseURL + "/AdminVendor/GetVendor";
        let params = new HttpParams();
        Object.entries(filter).forEach(([key, value]) => {
            if (value !== null && value !== undefined && value !== '') {
                params = params.set(key, value.toString());
            }
        });
        return this.http.get<PagedResponse<AdminVendorModel>>(url, { params });
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
    reviewVendor(request: ReviewVendorRequestModel) {
        let url = BaseURL + "/AdminVendor/ReviewVendor";
        return this.http.put(url, request);
    }
    getActiveVendor(filter: AdminVendorFilter): Observable<PagedResponse<AdminVendorModel>> {
        let url = BaseURL + "/AdminVendor/GetVendor?IsActive=true";
        let params = new HttpParams();
        Object.entries(filter).forEach(([key, value]) => {
            if (value !== null && value !== undefined && value !== '') {
                params = params.set(key, value.toString());
            }
        });
        return this.http.get<PagedResponse<AdminVendorModel>>(url, { params });
    }
}