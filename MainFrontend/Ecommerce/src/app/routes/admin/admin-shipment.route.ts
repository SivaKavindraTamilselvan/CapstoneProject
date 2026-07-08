import { Routes } from "@angular/router";
import { AddCoupon } from "../../admin/admin-coupon/add-coupon/add-coupon";
import { CouponList } from "../../admin/admin-coupon/coupon-list/coupon-list";
import { CouponDetail } from "../../admin/admin-coupon/coupon-detail/coupon-detail";
import { ShipmentList } from "../../admin/admin-shipment/shipment-list/shipment-list";
import { AdminShipmentDetails } from "../../admin/admin-shipment/admin-shipment-details/admin-shipment-details";


export const AdminShipmentRoutes: Routes = [

    {
        path: 'shipments/list',
        component: ShipmentList,
        data: { status: null, title: 'Shipment List' }
    },
    {
        path: 'shipment-details/:id',
        component: AdminShipmentDetails
    },
];