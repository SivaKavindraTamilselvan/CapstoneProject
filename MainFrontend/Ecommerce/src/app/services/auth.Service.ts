import { HttpClient } from "@angular/common/http";
import { Injectable } from "@angular/core";
import { LoginModel } from "../models/authentication/login.model";
import { BaseURL } from "../environment";
import { RegisterModel } from "../models/authentication/register-user.model";
import { RegisterAdminModel, RequestResendInviteDTO, RequestSetPasswordDTO, ResponseSetPasswordDTO } from "../models/authentication/register-admin.model";
import { RegisterVendorModel } from "../models/authentication/regiser-vendor.model";
import { RegisterVendorUserModel } from "../models/authentication/register-vendor-user.model";
import { Observable } from "rxjs";

@Injectable({
    providedIn: 'root'
})
export class AuthService {
    constructor(private http: HttpClient) {

    }
    public loginAPICall(loginModel: LoginModel) {
        let url = BaseURL + "/Authentication/Login";
        return this.http.post(url, loginModel);
    }
    public registerUserAPICall(registerModel: RegisterModel) {
        let url = BaseURL + "/Authentication/Register";
        return this.http.post(url, registerModel);
    }
    public registerAdminAPICall(registerModel: RegisterAdminModel) {
        let url = BaseURL + "/Admin/RegisterAdmin";
        return this.http.post(url, registerModel);
    }
    public registerVendorAPICall(registerModel: RegisterVendorModel) {
        let url = BaseURL + "/Authentication/RegisterVendor";
        return this.http.post(url, registerModel);
    }
    public registerVendorUserAPICall(registerModel: RegisterVendorUserModel) {
        let url = BaseURL + "/Vendor/RegisterVendorUser";
        return this.http.post(url, registerModel);
    }
    setPassword(dto: RequestSetPasswordDTO): Observable<ResponseSetPasswordDTO> {
        return this.http.post<ResponseSetPasswordDTO>(`${BaseURL}/Authentication/set-password`, dto);
    }

    resendInvite(dto: RequestResendInviteDTO): Observable<ResponseSetPasswordDTO> {
        return this.http.post<ResponseSetPasswordDTO>(`${BaseURL}/Authentication/resend-invite`, dto);
    }
}