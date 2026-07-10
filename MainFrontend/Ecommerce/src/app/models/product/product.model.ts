import { ProductImageModel } from "./product-image.model"; 
import { ProductVariantModel } from "./product-variant.model"; 

export class ProductModel {
    constructor(
        public productId: number,
        public productName: string = "",
        public description: string = "",
        public productSubCategoryName: string = "",
        public productCategoryName: string = "",
        public vendorName: string = "",
        public productApprovalStatus: string = "",
        public productStatus: string = "",
        public mainProductSubCategoryAttributeName: string = "",
        public createdAt: string = "",
        public updatedAt: string | null,
        public isAvailableForSale: boolean | null = null,
        public validationIssues: string[],
        public productVariants: ProductVariantModel[],
        public productImages: ProductImageModel[]
    ) { }
}