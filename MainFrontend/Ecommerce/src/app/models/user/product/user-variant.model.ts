import { ProductImageModel } from "../../product/product-image.model";
import { UserProductAttributeModel } from "../product-category/user-product-attribute.model";

export class UserProductVariantModel {
    constructor(
        public productVariantId: number,
        public sku: string = "",
        public price: number,
        public minimumQuantityPerUser: number,
        public availableQuantity: number,
        public reservedQuantity: number,
        public isReturn: boolean,
        public isExchange: boolean,
        public isAvailableForSale: boolean,
        public weightInKgs:number,
        public lengthInCm : number,
        public widthInCm :number,
        public heightInCm:number,
        public attributes: UserProductAttributeModel[],
        public productImages: ProductImageModel[]
    ) { }
}