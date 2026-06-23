import { HttpClient, HttpParams } from "@angular/common/http";
import { Injectable } from "@angular/core";
import { BaseURL } from "../environment";
import { UserProductModel } from "../models/user-product.model";
import { PagedResponse } from "../models/paged-response.model";
import { UserProductFilter } from "../models/user/product/user-product.filter";
import { Observable } from "rxjs";

@Injectable({
    providedIn: "root"
})
export class UserProductService {
    constructor(private http: HttpClient) {

    }
    getProduct(filter: UserProductFilter): Observable<PagedResponse<UserProductModel>> {
        let url = BaseURL + "/UserProduct/available";
        let params = new HttpParams();
        Object.entries(filter).forEach(([key, value]) => {
            if (value !== null && value !== undefined && value !== '') {
                params = params.set(key, value.toString());
            }
        });
        return this.http.get<PagedResponse<UserProductModel>>(url, { params });
    }
    getProductCategory() {
        let url = BaseURL + "/UserProductCategory/categories";
        return this.http.get(url);
    }
    getSubCategory(category: number) {
        let url = `${BaseURL}/UserProductCategory/categories/${category}/subcategories`; 
        return this.http.get(url);
    }
}