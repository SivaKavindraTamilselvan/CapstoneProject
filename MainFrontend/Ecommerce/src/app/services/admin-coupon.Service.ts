import { HttpClient, HttpParams } from "@angular/common/http";
import { Injectable } from "@angular/core";
import { Observable } from "rxjs";
import { BaseURL } from "../environment";
import { UserCouponModel } from "../models/user/coupon/user-coupon.model";
import { AddCouponModel } from "../models/admin/admin-coupon/add-coupon.model";
import { AdminCouponFilter } from "../models/admin/admin-coupon/coupon.filter";
import { PagedResponse } from "../models/paged-response.model";
import { CouponDetailModel, CouponListModel } from "../models/admin/admin-coupon/get-coupon.model";

@Injectable({
    providedIn: "root"
})
export class AdminCouponService {
    constructor(private http: HttpClient) {

    }
    addCoupon(request: AddCouponModel) {
        const url = BaseURL + "/Coupon/AddCoupon";
        return this.http.post(url, request);
    }
    getCoupon(filter: AdminCouponFilter) {
        let url = BaseURL + "/Coupon/coupons";
        let params = new HttpParams();

        Object.entries(filter).forEach(([key, value]) => {
            if (value !== null && value !== undefined && value !== '') {
                params = params.set(key, value.toString());
            }
        });

        return this.http.get<PagedResponse<CouponListModel>>(url, { params });
    }
    getCouponById(couponId : number) {
        let url = BaseURL + `/Coupon/coupons/${couponId}`;
        return this.http.get<CouponDetailModel>(url, {});
    }
    deactivateCoupon(couponId : number)
    {
        let url = BaseURL + `/Coupon/coupons/deactivate/${couponId}`;
        return this.http.patch(url,{});
    }
    activateCoupon(couponId : number)
    {
        let url = BaseURL + `/Coupon/coupons/activate/${couponId}`;
        return this.http.patch(url,{});
    }
}