import { HttpClient } from "@angular/common/http";
import { Injectable } from "@angular/core";
import { Observable } from "rxjs";
import { CartItemModel } from "../models/user/cart/user-cart.models";
import { BaseURL } from "../environment";
import { AddCartItemModel } from "../models/user/cart/add-cart,model";
import { RemoveCartItemModel } from "../models/user/cart/remove-cart.model";
import { UpdateCartItemModel } from "../models/user/cart/update-cart.model";

@Injectable({
    providedIn: "root"
})
export class UserCartService {
    constructor(private http: HttpClient) {

    }
    getCartItems(): Observable<CartItemModel[]> {
        let url = BaseURL + "/Cart/GetCartByUserId";
        return this.http.get<CartItemModel[]>(url, {});
    }

    addToCart(requst: AddCartItemModel): Observable<any> {
        let url = BaseURL + "/Cart/AddToCart";
        return this.http.post(url, requst);
    }

    removeFromCart(request: RemoveCartItemModel): Observable<any> {
        let url = BaseURL + "/Cart/DeleteCartItem";
        return this.http.put(url, request);
    }

    removeAllFromCart(): Observable<any> {
        let url = BaseURL + "/Cart/DeleteAllCartItem";
        return this.http.put(url,{});
    }

    updateQuantity(request: UpdateCartItemModel): Observable<any> {
        let url = BaseURL + "/Cart/UpdateToCart";
        return this.http.put(url, request);
    }
}
