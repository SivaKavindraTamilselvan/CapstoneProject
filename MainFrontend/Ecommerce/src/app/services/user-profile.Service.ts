import { HttpClient } from "@angular/common/http";
import { Injectable } from "@angular/core";
import { BaseURL } from "../environment";
import { ChangePasswordModel } from "../models/profile/change.password.model";

@Injectable({
    providedIn: "root"
})
export class ProfileService {
    constructor(private http: HttpClient) {

    }
    getProfile(){
        let url = BaseURL +  "/Authentication/Profile";
        return this.http.get(url, {});
    }
    changePassword(request : ChangePasswordModel)
    {
        let url = BaseURL + "/Authentication/change-password";
        return this.http.put(url,request);
    }
}