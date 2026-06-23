import { ProductVariantModel } from "../../product-variant.model";
import { ProductImageModel } from "../../product-image.model";

export class VendorProductModel {
    constructor(
        public productId: number = 0,
        public productName: string = "",
        public description: string = "",
        public productSubCategoryName: string = "",
        public productCategoryName: string = "",
        public productApprovalStatus: string = "",
        public productStatus: string = "",
        public mainProductSubCategoryAttributeName: string = "",
        public addedByVendorUserId: number = 0,
        public addedByVendorUser: string = "",
        public createdAt: string = "",
        public updatedAt: string = "",
        public isAvailableForSale: boolean = false,
        public validationIssues: string[] = [],
        public productVariants: ProductVariantModel[] = [],
        public productImages: ProductImageModel[] = []
    ) {}
}