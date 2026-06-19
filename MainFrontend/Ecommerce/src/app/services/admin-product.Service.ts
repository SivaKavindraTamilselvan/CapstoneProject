import { HttpClient } from "@angular/common/http";
import { Injectable } from "@angular/core";
import { BaseURL } from "../environment";
import { PagedResponse } from "../models/paged-response.model";
import { ProductModel } from "../models/product.model";

@Injectable({
    providedIn: "root"
})
export class AdminProductService{
    constructor(private http: HttpClient)
    {

    }
    getProducts()
    {
        let url = BaseURL + "/AdminProduct/all";
        return this.http.get<PagedResponse<ProductModel>>(url);
    }
}