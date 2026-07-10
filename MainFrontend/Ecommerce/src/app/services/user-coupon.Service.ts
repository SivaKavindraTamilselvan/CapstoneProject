import { HttpClient } from "@angular/common/http";
import { Injectable } from "@angular/core";
import { AddUserOrderModel } from "../models/user/order/place-order.model";
import { Observable } from "rxjs";
import { BaseURL } from "../environment";
import { UserCouponModel } from "../models/user/coupon/user-coupon.model"; 

@Injectable({
    providedIn: "root"
})
export class UserCouponService {
    constructor(private http: HttpClient) {

    }

    getActiveCoupons(): Observable<UserCouponModel[]> {
        const url = BaseURL + "/Coupon/AvailableCoupons";
        return this.http.get<UserCouponModel[]>(url);
    }
}