import { HttpClient, HttpParams } from "@angular/common/http";
import { Injectable } from "@angular/core";
import { AddProductModel } from "../models/vendor/vendor-product/add-model/add-product.model";
import { BaseURL } from "../environment";
import { AddProductImageModel } from "../models/vendor/vendor-product/add-model/add-product-image";
import { VendorProductFilter } from "../models/vendor/vendor-product/filter/vendor-product.filter";
import { PagedResponse } from "../models/paged-response.model";
import { VendorProductModel } from "../models/vendor/vendor-product/response/vendor-product.model";
import { VendorProductVariantFilter } from "../models/vendor/vendor-product/filter/vendor.varaint.filter";
import { VendorProductVariantModel } from "../models/vendor/vendor-product/response/vendor-variant.model";
import { AddProductVariantModel } from "../models/vendor/vendor-product/add-product-variant.model";
import { AddProductVariantImageModel } from "../models/vendor/vendor-product/add-model/add-variant-image.model";

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
    getProductVariant(filter: VendorProductVariantFilter) {
        let url = BaseURL + "/VendorProduct/ProductVariant"
        let params = new HttpParams();

        Object.entries(filter).forEach(([key, value]) => {
            if (value !== null && value !== undefined && value !== '') {
                params = params.set(key, value.toString());
            }
        });
        return this.http.get<PagedResponse<VendorProductVariantModel>>(url, { params });
    }
    addProductVariant(productVariant: AddProductVariantModel) {
        let url = BaseURL + "/VendorProduct/AddProductVariant";
        return this.http.post(url, productVariant);
    }
    addProductVariantImage(productVariantImage: AddProductVariantImageModel) {
        let url = BaseURL + "/VendorProduct/AddProductVariantImage";
        return this.http.post(url, productVariantImage);
    }
}