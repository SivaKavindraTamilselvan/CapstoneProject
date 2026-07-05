import { HttpClient, HttpParams } from "@angular/common/http";
import { Injectable } from "@angular/core";
import { AddUserOrderModel } from "../models/user/order/place-order.model";
import { Observable } from "rxjs";
import { BaseURL } from "../environment";
import { UserOrderFilter } from "../models/user/order/order-fiter";
export interface ShippingCheckResponse {
    totalShippingCharge: number;
    isShippingAvailable: boolean;
}

@Injectable({
    providedIn: "root"
})
export class UserOrderService {
    constructor(private http: HttpClient) {

    }
    placeOrder(request: AddUserOrderModel): Observable<any> {
        let url = BaseURL + "/Order/AddOrder";
        return this.http.post(url, request);
    }
    getOrders(filter: UserOrderFilter) {
        const url = BaseURL + "/Order/user";
        let params = new HttpParams();

        Object.entries(filter).forEach(([key, value]) => {
            if (value !== null && value !== undefined && value !== '') {
                params = params.set(key, value.toString());
            }
        });

        return this.http.get(url, { params });
    }
    checkShipment(model: AddUserOrderModel) {
        const url = `${BaseURL}/Order/CheckService`;
        return this.http.post<ShippingCheckResponse>(url, model);
    }

}