import { HttpClient, HttpParams } from "@angular/common/http";
import { Injectable } from "@angular/core";
import { AdminProductCategoryFilter } from "../models/admin-category.filter";
import { BaseURL } from "../environment";
import { PagedResponse } from "../models/paged-response.model";
import { ProductCategoryModel } from "../models/admin-category";
import { AddProductCategoryModel } from "../models/add-category.model";
import { AdminProductSubCategoryFilter } from "../models/admin-subcategory.filter";
import { AddProductSubCategoryModel } from "../models/add-subcategory.model";
import { AdminProductSubCategoryModel } from "../models/admin-subcategory.model";
import { AttributeModel } from "../models/admin-attribute.model";
import { AddAttributeModel } from "../models/add-attribute.model";
import { AttributeFilter } from "../models/attribute.filter";

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
    getProductSubCategory(filter : AdminProductSubCategoryFilter){
        let url = BaseURL + "/admin/product-categories/subcategories";
        let params = new HttpParams();

        Object.entries(filter).forEach(([key, value]) => {
            if (value !== null && value !== undefined && value !== '') {
                params = params.set(key, value.toString());
            }
        });
        return this.http.get<PagedResponse<AdminProductSubCategoryModel>>(url, { params });
    }
    addSubCategory(categoryModel : AddProductSubCategoryModel){
        let url = BaseURL + "/admin/product-categories/subcategories";
        return this.http.post(url,categoryModel);
    }
    getAttribute(filter : AttributeFilter){
        let url = BaseURL + "/admin/product-attributes";
        let params = new HttpParams();

        Object.entries(filter).forEach(([key, value]) => {
            if (value !== null && value !== undefined && value !== '') {
                params = params.set(key, value.toString());
            }
        });
        return this.http.get<PagedResponse<AttributeModel>>(url, { params });
    }
    addAttribute(attributeModel : AddAttributeModel){
        let url = BaseURL + "/admin/product-attributes";
        return this.http.post(url,attributeModel);
    }
}