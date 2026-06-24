import { HttpClient, HttpParams } from "@angular/common/http";
import { Injectable } from "@angular/core";
import { AdminProductCategoryFilter } from "../models/admin/admin-product-category/filter-models/admin-category.filter";
import { BaseURL } from "../environment";
import { PagedResponse } from "../models/paged-response.model";
import { AdminProductCategoryModel } from "../models/admin/admin-product-category/response/admin-category"; 
import { AddProductCategoryModel } from "../models/admin/admin-product-category/add-models/add-category.model";
import { AdminProductSubCategoryFilter } from "../models/admin/admin-product-category/filter-models/admin-subcategory.filter";
import { AddProductSubCategoryModel } from "../models/admin/admin-product-category/add-models/add-subcategory.model";
import { AdminProductSubCategoryModel } from "../models/admin/admin-product-category/response/admin-subcategory.model";
import { AdminAttributeModel } from "../models/admin/admin-product-category/response/admin-attribute.model"; 
import { AddAttributeModel } from "../models/admin/admin-product-category/add-models/add-attribute.model";
import { AttributeFilter } from "../models/admin/admin-product-category/filter-models/attribute.filter";
import { MappedAttributeFilter } from "../models/admin/admin-product-category/filter-models/mapped-attribute.filter";
import { AdminMappedAttributeModel } from "../models/admin/admin-product-category/response/admin-mapped.model";
import { AddMapedAttributeModel } from "../models/admin/admin-product-category/add-models/add-mapped.model";

@Injectable({
    providedIn: "root"
})
export class AdminProductCategoryService {
    constructor(private http: HttpClient) {

    }
    getProductCategory(filter: AdminProductCategoryFilter) {
        let url = BaseURL + "/admin/product-categories";
        let params = new HttpParams();

        Object.entries(filter).forEach(([key, value]) => {
            if (value !== null && value !== undefined && value !== '') {
                params = params.set(key, value.toString());
            }
        });

        return this.http.get<PagedResponse<AdminProductCategoryModel>>(url, { params });
    }
    addCategory(categoryModel: AddProductCategoryModel) {
        let url = BaseURL + "/admin/product-categories";
        return this.http.post(url, categoryModel);
    }
    deactivateCategory(categoryId : number){
        let url = `${BaseURL}/admin/product-categories/${categoryId}/deactivate`;
        return this.http.patch(url,{});
    }
    activateCategory(categoryId : number){
        let url = `${BaseURL}/admin/product-categories/${categoryId}/activate`;
        return this.http.patch(url,{});
    }
    getProductSubCategory(filter: AdminProductSubCategoryFilter) {
        let url = BaseURL + "/admin/product-categories/subcategories";
        let params = new HttpParams();

        Object.entries(filter).forEach(([key, value]) => {
            if (value !== null && value !== undefined && value !== '') {
                params = params.set(key, value.toString());
            }
        });
        return this.http.get<PagedResponse<AdminProductSubCategoryModel>>(url, { params });
    }
    addSubCategory(categoryModel: AddProductSubCategoryModel) {
        let url = BaseURL + "/admin/product-categories/subcategories";
        return this.http.post(url, categoryModel);
    }
    getAttribute(filter: AttributeFilter) {
        let url = BaseURL + "/admin/product-attributes";
        let params = new HttpParams();

        Object.entries(filter).forEach(([key, value]) => {
            if (value !== null && value !== undefined && value !== '') {
                params = params.set(key, value.toString());
            }
        });
        return this.http.get<PagedResponse<AdminAttributeModel>>(url, { params });
    }
    addAttribute(attributeModel: AddAttributeModel) {
        let url = BaseURL + "/admin/product-attributes";
        return this.http.post(url, attributeModel);
    }
    getmappedAttribute(filter: MappedAttributeFilter) {
        let url = BaseURL + "/admin/product-attributes/subcategory-attributes";
        let params = new HttpParams();

        Object.entries(filter).forEach(([key, value]) => {
            if (value !== null && value !== undefined && value !== '') {
                params = params.set(key, value.toString());
            }
        });
        return this.http.get<PagedResponse<AdminMappedAttributeModel>>(url, { params });
    }
    addMappedAttribute(mappedAttributeModel : AddMapedAttributeModel){
        let url = BaseURL + "/admin/product-attributes/subcategory-attributes";
        return this.http.post(url, mappedAttributeModel);
    }
}