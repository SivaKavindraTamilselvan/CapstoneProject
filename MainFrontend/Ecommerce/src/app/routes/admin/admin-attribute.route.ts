import { Routes } from "@angular/router";
import { RegisterAdmin } from "../../admin/admin-user/register-admin/register-admin";
import { AdminList } from "../../admin/admin-user/admin-list/admin-list";
import { AdminUserDetail } from "../../admin/admin-user/admin-user-detail/admin-user-detail";
import { AdminProduct } from "../../admin/admin-product/admin-product/admin-product";
import { AdminDetailProduct } from "../../admin/admin-product/admin-detail-product/admin-detail-product";
import { VariantList } from "../../admin/admin-product/variant-list/variant-list";
import { CategoryList } from "../../admin/admin-category/category-list/category-list";
import { Subcategorylist } from "../../admin/admin-subcategory/subcategorylist/subcategorylist";
import { AttributeList } from "../../admin/admin-attribute/attribute-list/attribute-list";
import { MappedAttributeList } from "../../admin/admin-mapped-attribute/mapped-attribute-list/mapped-attribute-list";


export const AdminAttributeRoutes: Routes = [
    {
        path: 'product-attribute/list',
        component: AttributeList,
        data: { status: null, title: 'Attribute List' }
    },
    {
        path: 'product-attribute/list?status=active',
        component: AttributeList,
        data: { status: true, title: 'Active Attribute List' }
    },
    {
        path: 'product-attribute/list?status=inactive',
        component: AttributeList,
        data: { status: false, title: 'Inactive attribute List' }
    },
    {
        path: 'attribute-mapping/list',
        component: MappedAttributeList,
        data: { status: null, title: 'Mapped Attribute List' }
    },
    {
        path: 'attribute-mapping/list?status=active',
        component: MappedAttributeList,
        data: { status: true, title: 'Active Mapped Attribute' }
    },
    {
        path: 'attribute-mapping/list?status=inactive',
        component: MappedAttributeList,
        data: { status: false, title: 'Inactive Mapped Attribute' }
    },
];