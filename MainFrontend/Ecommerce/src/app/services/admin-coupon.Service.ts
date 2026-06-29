import { HttpClient } from "@angular/common/http";
import { Injectable } from "@angular/core";
import { Observable } from "rxjs";
import { BaseURL } from "../environment";
import { UserCouponModel } from "../models/user/coupon/user-coupon.model"; 
import { AddCouponModel } from "../models/admin/admin-coupon/add-coupon.model";

@Injectable({
    providedIn: "root"
})
export class AdminCouponService {
    constructor(private http: HttpClient) {

    }
    addCoupon(request : AddCouponModel) {
        const url = BaseURL + "/Coupon/AddCoupon";
        return this.http.post(url,request);
    }
}