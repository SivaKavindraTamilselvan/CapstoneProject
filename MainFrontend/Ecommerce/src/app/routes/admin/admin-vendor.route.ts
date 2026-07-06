import { Routes } from "@angular/router";
import { ReviewVendor } from "../../admin/admin-vendor/review-vendor/review-vendor";
import { VendorList } from "../../admin/admin-vendor/vendor-list/vendor-list";
import { DeleteVendor } from "../../admin/admin-vendor/delete-vendor/delete-vendor";
import { VendorDetails } from "../../admin/admin-vendor/vendor-details/vendor-details";

export const AdminVendorRoutes: Routes = [
    {
        path: 'vendors/review',
        component: ReviewVendor
    },
    {
        path: 'vendors/list',
        component: VendorList,
        data: { status: null, title: 'Vendor List' }
    },
    {
        path: 'vendors/delete',
        component: VendorList,
        data: { status: 4, title: 'Deleted Vendor' }
    },
    {
        path: 'vendors/:id',
        component: VendorDetails
    },
];