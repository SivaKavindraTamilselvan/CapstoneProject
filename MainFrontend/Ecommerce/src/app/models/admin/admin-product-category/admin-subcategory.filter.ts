export class AdminProductSubCategoryFilter {
    constructor(
        public pageNumber: number = 1,
        public pageSize: number = 10,
        public productSubCategoryId: number | null = null,
        public productCategoryId: number | null = null,
        public status: boolean | null = null,
        public minimumCommissionPercentage: number | null = null,
        public maximumCommissionPercentage: number | null = null,
        public addedByAdminId: number | null = null
    ) {}
}