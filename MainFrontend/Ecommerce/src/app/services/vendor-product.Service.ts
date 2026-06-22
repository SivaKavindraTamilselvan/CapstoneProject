import { HttpClient, HttpParams } from "@angular/common/http";
import { Injectable } from "@angular/core";
import { AddProductModel } from "../models/add-product.model";
import { BaseURL } from "../environment";
import { AddProductImageModel } from "../models/add-product-image";
import { VendorProductFilter } from "../models/vendor-product.filter";
import { PagedResponse } from "../models/paged-response.model";
import { VendorProductModel } from "../models/vendor-product.model";

@Injectable({
    providedIn: "root"
})
export class VendorProductService {
    constructor(private http: HttpClient) {

    }
    addProduct(productModel: AddProductModel) {
        let url = BaseURL + "/VendorProduct/AddProduct";
        return this.http.post(url, productModel);
    }
    addProductImage(productImageModel: AddProductImageModel) {
        let url = BaseURL + "/VendorProduct/AddProductImage";
        return this.http.post(url, productImageModel);
    }
    getProduct(filter: VendorProductFilter) {
        let url = BaseURL + "/VendorProduct"
        let params = new HttpParams();

        Object.entries(filter).forEach(([key, value]) => {
            if (value !== null && value !== undefined && value !== '') {
                params = params.set(key, value.toString());
            }
        });
        return this.http.get<PagedResponse<VendorProductModel>>(url, { params });
    }
}