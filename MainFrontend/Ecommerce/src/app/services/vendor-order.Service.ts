import { HttpClient, HttpParams } from "@angular/common/http";
import { Injectable } from "@angular/core";
import { AddUserOrderModel, ProductReviewSummary } from "../models/user/order/place-order.model";
import { Observable } from "rxjs";
import { BaseURL } from "../environment";
import { UserCouponModel } from "../models/user/coupon/user-coupon.model";
import { PagedResponse } from "../models/paged-response.model";
import { AdminUserFilter } from "../models/admin/admin-user/admin-user.filter";
import { AdminOrderFilter } from "../models/admin/admin-orders/get-order.filter";
import { RequestVendorReturnFilter, VendorOrderFilter } from "../models/vendor/vendor-order/vendor-order.filter";
import { OrderItemSummaryModel } from "../models/admin/admin-orders/get-items.model";
import { ReviewReturnModel, ReviewReturnProductModel } from "../models/vendor/vendor-return/return.model";
import { ReviewProductModel } from "../models/product/review-product.model";
import { OrderItemModel } from "../models/admin/admin-orders/get-orderitem.model";

@Injectable({
    providedIn: "root"
})
export class VendorOrderService {
    constructor(private http: HttpClient) {

    }

    getOrders(filter: VendorOrderFilter) {
        const url = BaseURL + "/Order/vendor";
        let params = new HttpParams();

        Object.entries(filter).forEach(([key, value]) => {
            if (value !== null && value !== undefined && value !== '') {
                params = params.set(key, value.toString());
            }
        });

        return this.http.get<PagedResponse<OrderItemSummaryModel>>(url, { params });
    }

    updateOrder(orderId: number) {
        const url = BaseURL + `/Order/UpdateOrderStatus?orderitemid=${orderId}`;
        return this.http.put(url, {});
    }

    getReturnOrder(filter: RequestVendorReturnFilter) {
        const url = BaseURL + "/Vendor/returns";
        let params = new HttpParams();

        Object.entries(filter).forEach(([key, value]) => {
            if (value !== null && value !== undefined && value !== '') {
                params = params.set(key, value.toString());
            }
        });

        return this.http.get<PagedResponse<OrderItemSummaryModel>>(url, { params });
    }

    reviewReturn(model: ReviewReturnModel) {
        const url = BaseURL + "/Vendor/ReviewReturnProductByVendor";
        return this.http.put(url, model);
    }


    reviewReturnProduct(model: ReviewReturnProductModel) {
        const url = BaseURL + "/Vendor/ReviewReturnOriginalProductByVendor";
        return this.http.post(url, model);
    }

    acceptReturnProduct(returnId: number) {
        const url = BaseURL + `/Vendor/CreateReturnRefund/${returnId}`;
        return this.http.post(url, {});
    }

    getOrdersDetails(id: number) {
        const url = BaseURL + `/Order/GetUserOrderItemsById/${id}`;
        return this.http.get<OrderItemModel>(url, {});
    }
   
}