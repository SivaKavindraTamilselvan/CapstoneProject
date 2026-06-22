export class ProductCategoryModel {
    constructor(
        public productCategoryId: number,
        public productCategoryName: string | null,
        public isActive: boolean = true,
        public addedByAdminId: number,
        public addedUserName: string | null,
        public createdAt: Date
    ) {}
}