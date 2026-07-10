import { Routes } from "@angular/router";
import { AddCoupon } from "../../admin/admin-coupon/add-coupon/add-coupon";
import { CouponList } from "../../admin/admin-coupon/coupon-list/coupon-list";
import { CouponDetail } from "../../admin/admin-coupon/coupon-detail/coupon-detail";


export const AdminCouponRoutes: Routes = [
    {
        path: 'coupon/add',
        component: AddCoupon
    },
    {
        path: 'coupon/list',
        component: CouponList,
        data: { status: null, title: 'Coupon List' }
    },
    {
        path: 'coupons/:id',
        component: CouponDetail
    },
    {
        path: 'coupon/list?status=active',
        component: CouponList,
        data: { status: true, title: 'Active Coupons' }
    },
    {
        path: 'coupon/list?status=inactive',
        component: CouponList,
        data: { status: false, title: 'Inactive Coupons' }
    },
];