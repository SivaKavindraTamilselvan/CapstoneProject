export class ProductAttributeModel{
    constructor(
        public productVariantAttributeId: number,
        public attributeName: string = "",
        public attributeValue: string = "",
        public isActive: boolean
    ){}
}