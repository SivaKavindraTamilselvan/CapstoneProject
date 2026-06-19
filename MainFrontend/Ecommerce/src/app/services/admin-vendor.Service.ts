import { HttpClient } from "@angular/common/http";
import { Injectable } from "@angular/core";
import { BaseURL } from "../environment";
import { PagedResponse } from "../models/paged-response.model";
import { AdminVendorModel } from "../models/admin-vendor.model";
import { ReviewVendorRequestModel } from "../models/review-vendor.dto";

@Injectable({
    providedIn : "root"
})
export class AdminVendorService{
    constructor(private http:HttpClient)
    {

    }
    getVendor(){
        let url = BaseURL + "/AdminVendor/GetVendor";
        return this.http.get<PagedResponse<AdminVendorModel>>(url);
    }
    getPendingVendor(){
        let url = BaseURL + "/AdminVendor/GetVendor?ApprovalStatusId=1";
        return this.http.get<PagedResponse<AdminVendorModel>>(url);
    }
    reviewVendor(request : ReviewVendorRequestModel){
        let url = BaseURL + "/AdminVendor/ReviewVendor";
        return this.http.put(url,request);
    }
    getActiveVendor(){
        let url = BaseURL + "/AdminVendor/GetVendor?IsActive=true";
        return this.http.get<PagedResponse<AdminVendorModel>>(url);
    }
}