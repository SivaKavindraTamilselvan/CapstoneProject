import { HttpClient } from "@angular/common/http";
import { Injectable } from "@angular/core";
import { LoginModel } from "../models/authentication/login.model";
import { BaseURL } from "../environment";
import { RegisterModel } from "../models/authentication/register-user.model";
import { RegisterAdminModel } from "../models/register-admin.model";
import { RegisterVendorModel } from "../models/authentication/regiser-vendor.model";

@Injectable({
    providedIn: 'root'
})
export class AuthService {
    constructor(private http: HttpClient) {

    }
    public loginAPICall(loginModel: LoginModel) {
        let url = BaseURL + "/Authentication/Login";
        return this.http.post(url,loginModel);
    }
    public registerUserAPICall(registerModel : RegisterModel){
        let url = BaseURL + "/Authentication/Register";
        return this.http.post(url,registerModel);
    }
    public registerAdminAPICall(registerModel : RegisterAdminModel){
        let url = BaseURL + "/Admin/RegisterAdmin";
        return this.http.post(url,registerModel);
    }
    public registerVendorAPICall(registerModel : RegisterVendorModel){
        let url = BaseURL + "/Authentication/RegisterVendor";
        return this.http.post(url,registerModel);
    }
}