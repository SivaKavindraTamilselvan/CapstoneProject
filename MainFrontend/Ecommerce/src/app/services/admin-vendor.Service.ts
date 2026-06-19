import { HttpClient } from "@angular/common/http";
import { Injectable } from "@angular/core";
import { BaseURL } from "../environment";
import { PagedResponse } from "../models/paged-response.model";
import { AdminVendorModel } from "../models/admin-vendor.model";

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
}