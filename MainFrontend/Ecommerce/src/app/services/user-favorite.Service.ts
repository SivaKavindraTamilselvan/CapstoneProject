import { HttpClient } from "@angular/common/http";
import { Injectable } from "@angular/core";
import { Observable } from "rxjs";
import { CartItemModel } from "../models/user/cart/user-cart.models";
import { BaseURL } from "../environment";
import { AddCartItemModel } from "../models/user/cart/add-cart,model";
import { RemoveCartItemModel } from "../models/user/cart/remove-cart.model";
import { UpdateCartItemModel } from "../models/user/cart/update-cart.model";
import { FavoriteItemModel } from "../models/user/favorites/user-favorite.model";
import { AddFavoriteItemModel } from "../models/user/favorites/add-favorite.model";
import { RemoveFavoriteItemModel } from "../models/user/favorites/remove-favorite.model";

@Injectable({
    providedIn: "root"
})
export class UserFavoriteService {
    constructor(private http: HttpClient) {

    }
    getFavoriteItems(): Observable<FavoriteItemModel[]> {
        let url = BaseURL + "/Favorite/GetFavoriteByUserId";
        return this.http.get<FavoriteItemModel[]>(url, {});
    }

    addToFavorite(requst: AddFavoriteItemModel): Observable<any> {
        let url = BaseURL + "/Favorite/AddToFavorite";
        return this.http.post(url, requst);
    }

    removeFromFavorite(request: RemoveFavoriteItemModel): Observable<any> {
        let url = BaseURL + "/Favorite/DeleteFavoriteItem";
        return this.http.put(url, request);
    }
}
