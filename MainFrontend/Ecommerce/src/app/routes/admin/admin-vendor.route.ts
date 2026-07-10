import { Routes } from "@angular/router";
import { VendorList } from "../../admin/admin-vendor/vendor-list/vendor-list";
import { VendorDetails } from "../../admin/admin-vendor/vendor-details/vendor-details";

export const AdminVendorRoutes: Routes = [
    {
        path: 'vendors/review',
        component: VendorList,
        data: { status: 1, title: 'Pending Review Vendor',includeIsDeleted:false }
    },
    {
        path: 'vendors/list',
        component: VendorList,
        data: { status: null, title: 'Vendor List', includeIsDeleted: false }
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