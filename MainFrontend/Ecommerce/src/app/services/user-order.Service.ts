import { HttpClient } from "@angular/common/http";
import { Injectable } from "@angular/core";
import { AddUserOrderModel } from "../models/user/order/place-order.model";
import { Observable } from "rxjs";
import { BaseURL } from "../environment";

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
}