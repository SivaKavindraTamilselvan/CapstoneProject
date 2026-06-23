import { HttpClient, HttpParams } from "@angular/common/http";
import { Injectable } from "@angular/core";
import { AdminProductCategoryFilter } from "../models/admin/admin-product-category/admin-category.filter";
import { BaseURL } from "../environment";
import { PagedResponse } from "../models/paged-response.model";
import { ProductCategoryModel } from "../models/admin/admin-product-category/admin-category";
import { AddProductCategoryModel } from "../models/admin/admin-product-category/add-category.model";
import { AdminProductSubCategoryFilter } from "../models/admin/admin-product-category/admin-subcategory.filter";
import { AddProductSubCategoryModel } from "../models/admin/admin-product-category/add-subcategory.model";
import { AdminProductSubCategoryModel } from "../models/admin/admin-product-category/admin-subcategory.model";
import { AttributeModel } from "../models/admin/admin-product-category/admin-attribute.model";
import { AddAttributeModel } from "../models/admin/admin-product-category/add-attribute.model";
import { AttributeFilter } from "../models/admin/admin-product-category/attribute.filter";
import { MappedAttributeFilter } from "../models/admin/admin-product-category/mapped-attribute.filter";
import { AdminMappedAttributeModel } from "../models/admin/admin-product-category/admin-mapped.model";
import { AddMapedAttributeModel } from "../models/admin/admin-product-category/add-mapped.model";

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

        return this.http.get<PagedResponse<ProductCategoryModel>>(url, { params });
    }
    addCategory(categoryModel: AddProductCategoryModel) {
        let url = BaseURL + "/admin/product-categories";
        return this.http.post(url, categoryModel);
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
        return this.http.get<PagedResponse<AttributeModel>>(url, { params });
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