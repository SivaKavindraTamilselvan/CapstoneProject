import { HttpClient, HttpParams } from "@angular/common/http";
import { Injectable } from "@angular/core";
import { AddUserOrderModel } from "../models/user/order/place-order.model";
import { Observable } from "rxjs";
import { BaseURL } from "../environment";
import { UserCouponModel } from "../models/user/coupon/user-coupon.model"; 
import { PagedResponse } from "../models/paged-response.model";
import { AdminUserFilter } from "../models/admin/admin-user/admin-user.filter";
import { AdminOrderFilter } from "../models/admin/admin-orders/get-order.filter";

@Injectable({
    providedIn: "root"
})
export class AdminOrderService {
    constructor(private http: HttpClient) {

    }

    getOrders(filter:AdminOrderFilter){
        const url = BaseURL + "/Order/admin";
        let params = new HttpParams();

        Object.entries(filter).forEach(([key, value]) => {
            if (value !== null && value !== undefined && value !== '') {
                params = params.set(key, value.toString());
            }
        });

        return this.http.get(url,{params});
    }
    getOrdersDetails(id:number){
        const url = BaseURL +  `/Order/GetAdminOrderById/${id}`;
        return this.http.get(url,{});
    }
}