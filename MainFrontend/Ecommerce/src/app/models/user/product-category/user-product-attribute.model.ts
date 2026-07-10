export class UserProductAttributeModel{
    constructor(
        public productVariantAttributeId: number,
        public attributeName: string = "",
        public attributeValue: string = "",
    ){}
}