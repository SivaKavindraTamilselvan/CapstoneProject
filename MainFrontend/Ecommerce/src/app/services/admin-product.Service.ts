import { HttpClient, HttpParams } from "@angular/common/http";
import { Injectable } from "@angular/core";
import { Observable } from "rxjs";
import { BaseURL } from "../environment";
import { PagedResponse } from "../models/paged-response.model";
import { ProductModel } from "../models/product.model";
import { AdminProductFilter } from "../models/admin-product.filter";
import { ReviewProductRequestModel } from "../models/review-product.dto";

@Injectable({
    providedIn: "root"
})
export class AdminProductService {
    constructor(private http: HttpClient) {

    }

    getProducts(filter: AdminProductFilter): Observable<PagedResponse<ProductModel>> {
        let url = BaseURL + "/AdminProduct/all";
        let params = new HttpParams();

        Object.entries(filter).forEach(([key, value]) => {
            if (value !== null && value !== undefined && value !== '') {
                params = params.set(key, value.toString());
            }
        });

        return this.http.get<PagedResponse<ProductModel>>(url, { params });
    }
    reviewProduct(request: ReviewProductRequestModel) {
        let url = BaseURL + "/AdminProduct/ReviewProduct";
        return this.http.put(url, request);
    }
}