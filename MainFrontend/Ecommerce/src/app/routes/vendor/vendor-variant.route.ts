import { Routes } from "@angular/router";
import { AddVariant } from "../../vendor/vendor-variant/add-variant/add-variant";
import { VendorVariantDetails } from "../../vendor/vendor-variant/vendor-variant-details/vendor-variant-details";
import { VariantList } from "../../vendor/vendor-variant/variant-list/variant-list";

export const VendorProductVariantRoute: Routes = [
    {
        path: 'products/:id/variants/add',
        component: AddVariant
    },
    {
        path: 'products/variants/list',
        component: VariantList,
        data: { status: null, deleted: false, update: false, title: 'Product Variant List' }
    },
    {
        path: 'products/deleted-variants/list',
        component: VariantList,
        data: { status: null, deleted: true, title: 'Deleted Variant List' }
    },
    {
        path: 'products/variants/review',
        component: VariantList,
        data: { status: 1, deleted: false, title: 'Review Variant List' }
    },
    {
        path: 'products/variants/update',
        component: VariantList,
        data: { status: null, deleted: false, update: true, title: 'Update Variant' }
    },
    {
        path: 'products/variants/update-rejected',
        component: VariantList,
        data: { status: 5, deleted: false, title: 'Rejected Variant List' }
    },
    {
        path: 'products/variant/:id',
        component: VendorVariantDetails
    },
]