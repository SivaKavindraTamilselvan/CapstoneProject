import { HttpClient, HttpParams } from "@angular/common/http";
import { Injectable } from "@angular/core";
import { BaseURL } from "../environment";
import { AdminOrderFilter } from "../models/admin/admin-orders/get-order.filter";
import { RequestAdminCancelFilter } from "../models/user/order/cancel.order.model";
import { AdminCreateReturnRefund, RequestAdminReturnFilter } from "../models/admin/admin-orders/get-order.model";
import { OrderItemModel } from "../models/admin/admin-orders/get-orderitem.model";

@Injectable({
    providedIn: "root"
})
export class AdminOrderService {
    constructor(private http: HttpClient) {

    }

    getOrders(filter: AdminOrderFilter) {
        const url = BaseURL + "/Order/admin";
        let params = new HttpParams();

        Object.entries(filter).forEach(([key, value]) => {
            if (value !== null && value !== undefined && value !== '') {
                params = params.set(key, value.toString());
            }
        });

        return this.http.get(url, { params });
    }

    getOrdersDetails(id: number) {
        const url = BaseURL + `/Order/GetAdminOrderById/${id}`;
        return this.http.get(url, {});
    }

    getOrdersItemsDetails(id: number) {
            const url = BaseURL + `/Order/GetUserOrderItemsById/${id}`;
            return this.http.get<OrderItemModel>(url, {});
        }

    getCancelledOrder(filter: RequestAdminCancelFilter) {
        const url = BaseURL + "/Cancel/admin-cancels";
        let params = new HttpParams();

        Object.entries(filter).forEach(([key, value]) => {
            if (value !== null && value !== undefined && value !== '') {
                params = params.set(key, value.toString());
            }
        });

        return this.http.get(url, { params });
    }

    getReturnedOrder(filter: RequestAdminReturnFilter) {
        const url = BaseURL + "/AdminReturn/returns";
        let params = new HttpParams();

        Object.entries(filter).forEach(([key, value]) => {
            if (value !== null && value !== undefined && value !== '') {
                params = params.set(key, value.toString());
            }
        });

        return this.http.get(url, { params });
    }

    getCancelOrdersDetails(id: number) {
        const url = BaseURL + `/Cancel/admin-cancel/${id}`;
        return this.http.get(url, {});
    }

    getReturnOrdersDetails(id: number) {
        const url = BaseURL + `/AdminReturn/returns/${id}`;
        return this.http.get(url, {});
    }

    createReturnOrdersDetails(model: AdminCreateReturnRefund) {
        const url = BaseURL + `/AdminReturn/CreateReturnRefund`;
        return this.http.post(url, model);
    }

}