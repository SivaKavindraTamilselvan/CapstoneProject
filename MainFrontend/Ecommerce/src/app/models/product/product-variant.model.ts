import { ProductAttributeModel } from "../admin/admin-product-category/product-attribute.model";
import { ProductImageModel } from "./product-image.model";

export class ProductVariantModel {
    constructor(
        public productVariantId: number,
        public sku: string = "",
        public price: number,
        public weightInKgs: number,
        public lengthInCm: number,
        public widthInCm: number,
        public heightInCm: number,
        public minimumQuantityPerUser: number,
        public productApprovalStatus: string = "",
        public productVariantStatus: string = "",
        public availableQuantity: number,
        public reservedQuantity: number,
        public isReturn: boolean,
        public isExchange: boolean,
        public isAvailableForSale: boolean,
        public createdAt: string = "",
        public updatedAt: string | null,
        public validationIssues: string[],
        public attributes: ProductAttributeModel[],
        public productImages: ProductImageModel[] 
    ) {}
}