import { ProductImageModel } from "../../product/product-image.model";
import { UserProductVariantModel } from "./user-variant.model";

export class UserProductModel {
    constructor(
        public productId: number,
        public productName: string = "",
        public description: string = "",
        public productSubCategoryName: string = "",
        public productCategoryName: string = "",
        public vendorName: string = "",
        public mainProductSubCategoryAttributeName: string = "",
        public productVariants: UserProductVariantModel[],
        public productImages: ProductImageModel[]
    ) { }
}