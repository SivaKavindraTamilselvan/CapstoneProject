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
import { AddProductVariantModel } from "../models/vendor/vendor-product/add-model/add-product-variant.model";
import { AddProductVariantImageModel } from "../models/vendor/vendor-product/add-model/add-variant-image.model";
import { ReviewProductRequestModel } from "../models/product/review-product.dto";
import { Observable } from "rxjs";
import { ProductModel } from "../models/product/product.model";
import { UpdateProductStatus } from "../models/vendor/vendor-product/add-model/update-product-status.model";
import { UpdateRejectedProductModel } from "../models/vendor/vendor-product/add-model/update-rejected-product.model";
import { AdminAttributeModel } from "../models/admin/admin-product-category/response/admin-attribute.model";
import { MappedAttributeFilter } from "../models/admin/admin-product-category/filter-models/mapped-attribute.filter";
import { AdminMappedAttributeModel } from "../models/admin/admin-product-category/response/admin-mapped.model";
import { ReviewProductVariantRequestModel } from "../models/product/review-variant.dto";
import { UpdateProductVariantStatus } from "../models/vendor/vendor-product/add-model/update-variant-status.model";
import { UpdateRejectedProductVariantModel } from "../models/vendor/vendor-product/add-model/update-rejected-status.model";

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
    getDeletedProduct(filter: VendorProductFilter) {
        let url = BaseURL + "/VendorProduct?includeIsDeleted=true"
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
    getProductCategory() {
        let url = BaseURL + "/UserProductCategory/categories";
        return this.http.get(url);
    }
    getSubCategory(category: number) {
        let url = `${BaseURL}/UserProductCategory/vendor-categories/${category}/subcategories`;
        return this.http.get(url);
    }
    getReviewProducts(filter: VendorProductFilter) {
        let url = BaseURL + "/VendorProduct?includeIsDeleted=false&ProductApprovalStatusId=1"
        let params = new HttpParams();

        Object.entries(filter).forEach(([key, value]) => {
            if (value !== null && value !== undefined && value !== '') {
                params = params.set(key, value.toString());
            }
        });
        return this.http.get<PagedResponse<VendorProductModel>>(url, { params });
    }
    reviewProduct(request: ReviewProductRequestModel) {
        let url = BaseURL + "/Vendor/ReviewProductByVendor";
        return this.http.put(url, request);
    }
    reviewProductVariant(request: ReviewProductVariantRequestModel) {
        let url = BaseURL + "/Vendor/ReviewProductVariantByVendor";
        return this.http.put(url, request);
    }
    updateProduct(request: UpdateProductStatus) {
        let url = BaseURL + "/VendorProduct/UpdateProduct";
        return this.http.put(url, request);
    }
    deleteProduct(request: UpdateProductStatus) {
        let url = BaseURL + "/VendorProduct/UpdateProduct";
        return this.http.put(url, request);
    }
    updateProductVariant(request: UpdateProductVariantStatus) {
        let url = BaseURL + "/VendorProduct/UpdateProductVariant";
        return this.http.put(url, request);
    }
    updateRejectedProduct(request: UpdateRejectedProductModel) {
        let url = BaseURL + "/VendorProduct/UpdateProductDetails";
        return this.http.put(url, request);
    }
    updateRejectedVariant(request: UpdateRejectedProductVariantModel) {
        let url = BaseURL + "/VendorProduct/UpdateRejectedProductVariant";
        return this.http.put(url, request);
    }
    getProductDetails(productId: number) {
        let url = `${BaseURL}/VendorProduct/${productId}`;
        return this.http.get<ProductModel>(url, {});
    }
    getProductVariantDetails(productId: number) {
        let url = `${BaseURL}/VendorProduct/variant/${productId}`;
        return this.http.get<ProductModel>(url, {});
    }
    getmappedAttribute(id: number) {
        const url = BaseURL + `/VendorProduct/subcategory-attributes?ProductSubCategoryId=${id}`;
        return this.http.get<PagedResponse<AdminMappedAttributeModel>>(url);
    }
    getAttributes() {
        const url = BaseURL + `/VendorProduct/attributes`;
        return this.http.get(url);
    }

    deleteProductImage(id: number) {
        let url = BaseURL + `/VendorProduct/DeleteProductImage/${id}`
        return this.http.delete(url);
    }

    makeImageDefault(id: number) {
        let url = BaseURL + `/VendorProduct/UpdateProductImageAsDefault/${id}`
        return this.http.put(url,{});
    }

     uploadProductImage(formData: FormData): Observable<any> {
        return this.http.post(`${BaseURL}/VendorProduct/upload-image`, formData);
    }
    uploadProductVariantImage(formData: FormData): Observable<any> {
        return this.http.post(`${BaseURL}/VendorProduct/upload-variant-image`, formData);
    }
}