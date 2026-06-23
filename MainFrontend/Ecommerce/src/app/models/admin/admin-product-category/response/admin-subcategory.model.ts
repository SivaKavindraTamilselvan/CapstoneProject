export class AdminProductSubCategoryModel {
    constructor(
        public productSubCategoryId: number = 0,
        public productSubCategoryName: string | null = null,
        public productCategoryId: number = 0,
        public isActive: boolean = true,
        public commissionPercentage: number = 0,
        public addedByAdminId: number = 0,
        public addedUserName: string | null = null,
        public createdAt: string = ''
    ) {}
}