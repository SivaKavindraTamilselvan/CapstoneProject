import { HttpClient, HttpParams } from "@angular/common/http";
import { Injectable } from "@angular/core";
import { AdminProductCategoryFilter } from "../models/admin-category.filter";
import { BaseURL } from "../environment";
import { PagedResponse } from "../models/paged-response.model";
import { ProductCategoryModel } from "../models/admin-category";
import { AddProductCategoryModel } from "../models/add-category.model";

@Injectable({
    providedIn: "root"
})
export class AdminProductCategoryService {
    constructor(private http: HttpClient) {

    }
    getProductCategory(filter : AdminProductCategoryFilter){
        let url = BaseURL + "/admin/product-categories";
        let params = new HttpParams();

        Object.entries(filter).forEach(([key, value]) => {
            if (value !== null && value !== undefined && value !== '') {
                params = params.set(key, value.toString());
            }
        });

        return this.http.get<PagedResponse<ProductCategoryModel>>(url, { params });
    }
    addCategory(categoryModel : AddProductCategoryModel){
        let url = BaseURL + "/admin/product-categories";
        return this.http.post(url,categoryModel);
    }
}