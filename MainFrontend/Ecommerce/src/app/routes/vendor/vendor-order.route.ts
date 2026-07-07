import { Routes } from "@angular/router";

import { VendorOrderList } from "../../vendor/vendor-order/vendor-order-list/vendor-order-list";

export const VendorOrderRoute: Routes = [
    {
        path: 'orders/list',
        component: VendorOrderList,
        data: { status: null, order: null, title: 'Update Product' }
    },
    {
        path: 'orders/confirmed-orders',
        component: VendorOrderList,
        data: { status: 1, order: 2, title: 'Update Product' }
    },
]