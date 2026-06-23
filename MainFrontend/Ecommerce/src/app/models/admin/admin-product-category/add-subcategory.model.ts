export class AddProductSubCategoryModel {
    constructor(
        public productSubCategoryName: string = "",
        public commissionPercentage: number = 0,
        public productCategoryId: number = 0
    ) {}
}