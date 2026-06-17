import { HttpClient } from "@angular/common/http";
import { Injectable } from "@angular/core";
import { LoginModel } from "../models/login.model";
import { BaseURL } from "../environment";

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
}