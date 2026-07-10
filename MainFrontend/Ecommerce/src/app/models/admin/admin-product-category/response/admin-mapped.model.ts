export class AdminMappedAttributeModel {
    constructor(
        public productSubCategoryAttributeId: number = 0,
        public productSubCategoryId: number = 0,
        public productSubCategoryName: string = "",
        public isSubCategoryActive: boolean = false,
        public attributeMasterId: number = 0,
        public attributeName: string = "",
        public isAttributeActive: boolean = false,
        public isActive: boolean = false,
        public addedByAdminId: number = 0,
        public createdAt: Date = new Date()
    ) {}
}