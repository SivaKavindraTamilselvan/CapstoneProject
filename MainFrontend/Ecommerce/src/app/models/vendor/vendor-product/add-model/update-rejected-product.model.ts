export class UpdateRejectedProductModel{
    constructor(
        public productId : number = 0,
        public productName : string = '',
        public description : string = '',
        public productSubCategoryId : number = 0,
        public mainProductSubCategoryAttributeId : number = 0
    ){}
}