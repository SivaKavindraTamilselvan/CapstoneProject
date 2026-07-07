import { Routes } from '@angular/router';
import { CategoryList } from '../admin/admin-category/category-list/category-list';
import { Subcategorylist } from '../admin/admin-subcategory/subcategorylist/subcategorylist';
import { InactiveSubCategory } from '../admin/admin-subcategory/inactive-sub-category/inactive-sub-category';
import { ActiveSubCategory } from '../admin/admin-subcategory/active-sub-category/active-sub-category';
import { AttributeList } from '../admin/admin-attribute/attribute-list/attribute-list';
import { ActiveAttribute } from '../admin/admin-attribute/active-attribute/active-attribute';
import { InactiveAttribute } from '../admin/admin-attribute/inactive-attribute/inactive-attribute';
import { MappedAttributeList } from '../admin/admin-mapped-attribute/mapped-attribute-list/mapped-attribute-list';
import { ActiveMappedAttribute } from '../admin/admin-mapped-attribute/active-mapped-attribute/active-mapped-attribute';
import { InactiveMappedAttribute } from '../admin/admin-mapped-attribute/inactive-mapped-attribute/inactive-mapped-attribute';
import { AddCoupon } from '../admin/admin-coupon/add-coupon/add-coupon';
import { GetAdminOrders } from '../admin/admin-orders/get-admin-orders/get-admin-orders';
import { ShipmentList } from '../admin/admin-shipment/shipment-list/shipment-list';
import { UpdateShipment } from '../admin/admin-shipment/update-shipment/update-shipment';
import { CouponList } from '../admin/admin-coupon/coupon-list/coupon-list';
import { CouponDetail } from '../admin/admin-coupon/coupon-detail/coupon-detail';
import { ActiveCoupon } from '../admin/admin-coupon/active-coupon/active-coupon';
import { DeactiveCoupon } from '../admin/admin-coupon/deactive-coupon/deactive-coupon';
import { AdminNotificationList } from '../notification/admin/admin-notification-list/admin-notification-list';
import { AdminProfilePage } from '../admin/admin-profile-page/admin-profile-page';
import { AdminOrderDetails } from '../admin/admin-orders/admin-order-details/admin-order-details';
import { AdminShipmentDetails } from '../admin/admin-shipment/admin-shipment-details/admin-shipment-details';
import { AdminUserRoutes } from './admin/admin-user.route';
import { AdminVendorRoutes } from './admin/admin-vendor.route';
import { AdminProductRoutes } from './admin/admin-product.route';
import { AdminCategoryRoutes } from './admin/admin-category.route';
import { AdminAttributeRoutes } from './admin/admin-attribute.route';
import { AdminCouponRoutes } from './admin/admin-coupon.route';
import { AdminOrderRoutes } from './admin/admin-order.router';

export const AdminRoutes: Routes = [
    
   
    
   
   
    {
        path: 'shipments/list',
        component: ShipmentList
    },
    {
        path: 'shipments/update',
        component: UpdateShipment
    },
    {
        path: 'notifications',
        component: AdminNotificationList
    },
    {
        path: 'profile',
        component: AdminProfilePage
    },
    {
        path: 'order/:id',
        component: AdminOrderDetails
    },
     {
        path: 'shipment-details/:id',
        component: AdminShipmentDetails
    },
    ...AdminUserRoutes,
    ...AdminVendorRoutes,
    ...AdminProductRoutes,
    ...AdminCategoryRoutes,
    ...AdminAttributeRoutes,
    ...AdminCouponRoutes,
    ...AdminOrderRoutes,
];