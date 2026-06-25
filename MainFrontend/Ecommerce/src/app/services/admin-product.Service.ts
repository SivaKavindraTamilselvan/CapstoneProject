import { HttpClient, HttpParams } from "@angular/common/http";
import { Injectable } from "@angular/core";
import { map, Observable } from "rxjs";
import { BaseURL } from "../environment";
import { PagedResponse } from "../models/paged-response.model";
import { ProductModel } from "../models/product/product.model";
import { AdminProductFilter } from "../models/admin/admin-product/filter/admin-product.filter";
import { ReviewProductRequestModel } from "../models/product/review-product.dto";
import { AdminDeleteProductModel } from "../models/admin/admin-product/models/delete-product.model";
import { AdminAttributeModel } from "../models/admin/admin-product-category/response/admin-attribute.model";

@Injectable({
    providedIn: "root"
})
export class AdminProductService {
    constructor(private http: HttpClient) {

    }

    getProducts(filter: AdminProductFilter): Observable<PagedResponse<ProductModel>> {
        let url = BaseURL + "/AdminProduct/all?includeIsDeleted=false";
        let params = new HttpParams();

        Object.entries(filter).forEach(([key, value]) => {
            if (value !== null && value !== undefined && value !== '') {
                params = params.set(key, value.toString());
            }
        });

        return this.http.get<PagedResponse<ProductModel>>(url, { params });
    }
    getReviewProducts(filter: AdminProductFilter): Observable<PagedResponse<ProductModel>> {
        let url = BaseURL + "/AdminProduct/all?ProductApprovalStatusId=2";
        let params = new HttpParams();

        Object.entries(filter).forEach(([key, value]) => {
            if (value !== null && value !== undefined && value !== '') {
                params = params.set(key, value.toString());
            }
        });

        return this.http.get<PagedResponse<ProductModel>>(url, { params });
    }
    getDeletedProducts(filter: AdminProductFilter): Observable<PagedResponse<ProductModel>> {
        let url = BaseURL + "/AdminProduct/all?ProductApprovalStatusId=6";
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
        return this.http.post(url, request);
    }
    deleteProduct(request: AdminDeleteProductModel) {
        let url = BaseURL + "/AdminProduct/DeleteProduct";
        return this.http.patch(url, request);
    }
    getProductCategory() {
        let url = BaseURL + "/admin/product-categories";
        return this.http.get(url);
    }
    getSubCategory(category: number) {
        let url = `${BaseURL}/admin/product-categories/subcategories?ProductCategoryId=${category}`;
        return this.http.get(url);
    }
    getAttribute() {
        let url = BaseURL + "/admin/product-attributes";
        let params = new HttpParams();
        return this.http.get<PagedResponse<AdminAttributeModel>>(url, { params });
    }
    getProductDetails(productId : number){
        let url = `${BaseURL}/AdminProduct/${productId}`;
        return this.http.get<ProductModel>(url,{});
    }
}