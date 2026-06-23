export class VendorProductFilter {
    constructor(
        public productName: string | null = null,
        public productCategoryId: number | null = null,
        public productSubCategoryId: number | null = null,
        public productApprovalStatusId: number | null = null,
        public productStatusId: number | null = null,
        public addedByVendorUserId: number | null = null,
        public minPrice: number | null = null,
        public maxPrice: number | null = null,
        public searchTerm: string | null = null,
        public hasIssues: boolean | null = null,
        public isAvailableForSale: boolean | null = null,
        public minAvailableQuantity: number | null = null,
        public maxAvailableQuantity: number | null = null,
        public minReservedQuantity: number | null = null,
        public maxReservedQuantity: number | null = null,
        public mainProductSubCategoryAttributeId: number | null = null,
        public pageNumber: number = 1,
        public pageSize: number = 10
    ) {}
}