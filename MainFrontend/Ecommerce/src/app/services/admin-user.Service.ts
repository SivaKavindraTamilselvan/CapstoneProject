import { HttpClient, HttpParams } from "@angular/common/http";
import { Injectable } from "@angular/core";
import { BaseURL } from "../environment";
import { PagedResponse } from "../models/paged-response.model";
import { AdminUserModel } from "../models/admin-user.model";
import { AdminUserFilter } from "../models/admin-user.filter";
import { Observable } from "rxjs";

@Injectable({
    providedIn: "root"
})
export class AdminUserService {
    constructor(private http: HttpClient) {

    }
    getAdminUser(filter :AdminUserFilter) : Observable<PagedResponse<AdminUserModel>> {
        let url = BaseURL + "/Admin/GetAdminUser";
        let params = new HttpParams();
        Object.entries(filter).forEach(([key,value])=>{
            if(value!==null && value!==undefined && value!==''){
                params = params.set(key,value.toString());
            }
        });
        return this.http.get<PagedResponse<AdminUserModel>>(url,{params});
    }
    activateAdminUser(adminId: number) {
        let url = `${BaseURL}/Admin/admin-users/${adminId}/activate`;
        return this.http.put(url,{});
    }
    deactivateAdminUser(adminId: number) {
        let url = `${BaseURL}/Admin/admin-users/${adminId}/deactivate`;
        return this.http.put(url,{});
    }
    getActiveAdminUser(){
        let url = BaseURL + "/Admin/GetAdminUser?status=true";
        let params = new HttpParams();
        return this.http.get<PagedResponse<AdminUserModel>>(url,{params});
    }
    getDeactiveAdminUser(filter : AdminUserFilter) : Observable<PagedResponse<AdminUserModel>> {
        let url = BaseURL + "/Admin/GetAdminUser?status=false";
         let params = new HttpParams();
        Object.entries(filter).forEach(([key,value])=>{
            if(value!==null && value!==undefined && value!==''){
                params = params.set(key,value.toString());
            }
        });
        return this.http.get<PagedResponse<AdminUserModel>>(url,{params});
    }
}