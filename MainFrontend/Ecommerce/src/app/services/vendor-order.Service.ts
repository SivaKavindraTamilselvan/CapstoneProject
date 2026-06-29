import { HttpClient, HttpParams } from "@angular/common/http";
import { Injectable } from "@angular/core";
import { AddUserOrderModel } from "../models/user/order/place-order.model";
import { Observable } from "rxjs";
import { BaseURL } from "../environment";
import { UserCouponModel } from "../models/user/coupon/user-coupon.model";
import { PagedResponse } from "../models/paged-response.model";
import { AdminUserFilter } from "../models/admin/admin-user/admin-user.filter";
import { AdminOrderFilter } from "../models/admin/admin-orders/get-order.filter";
import { VendorOrderFilter } from "../models/vendor/vendor-order/vendor-order.filter";
import { OrderItemSummaryModel } from "../models/admin/admin-orders/get-items.model";

@Injectable({
    providedIn: "root"
})
export class VendorOrderService {
    constructor(private http: HttpClient) {

    }

    getOrders(filter: VendorOrderFilter) {
        const url = BaseURL + "/Order/vendor";
        let params = new HttpParams();

        Object.entries(filter).forEach(([key, value]) => {
            if (value !== null && value !== undefined && value !== '') {
                params = params.set(key, value.toString());
            }
        });

        return this.http.get<PagedResponse<OrderItemSummaryModel>>(url, { params });
    }
}