import { Routes } from "@angular/router";

import { VendorOrderList } from "../../vendor/vendor-order/vendor-order-list/vendor-order-list";
import { VendorReturnOrder } from "../../vendor/vendor-return/vendor-return-order/vendor-return-order";
import { VendorOrderDetails } from "../../vendor/vendor-order/vendor-order-details/vendor-order-details";

export const VendorOrderRoute: Routes = [
    {
        path: 'orders/list',
        component: VendorOrderList,
        data: { status: null, order: null, title: 'All Orders' }
    },
    {
        path: 'orders/confirmed-orders',
        component: VendorOrderList,
        data: { status: 1, order: 2, title: 'OnGoing Orders' }
    },
    {
        path: 'orders/cancelled-orders',
        component: VendorOrderList,
        data: { status: 7, title: 'Cancelled Orders' }
    },
     {
        path: 'orders/:id',
        component: VendorOrderDetails
    },
    {
        path: 'returns/pending',
        component: VendorReturnOrder,
        data: { status: 1, title: 'Pending Return List' }
    },
    {
        path: 'returns/inspection',
        component: VendorReturnOrder,
        data: { status: 6, title: 'Product Inspection List' }
    },
    {
        path: 'returns',
        component: VendorReturnOrder,
        data: { status: null, title: 'Return List' }
    },
]