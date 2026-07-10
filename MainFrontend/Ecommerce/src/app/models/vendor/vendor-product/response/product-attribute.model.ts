export class VendorProductAttributeModel{
    constructor(
        public productVariantAttributeId: number,
        public attributeName: string = "",
        public attributeValue: string = "",
        public isActive: boolean
    ){}
}