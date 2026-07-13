export class AdminProductVariantFilter {
    constructor(
        public pageNumber: number = 1,
        public pageSize: number = 10,

        public sku: string ='',
        public VendorId: number | null = null,

        public productId: number | null = null,
        public searchTerm: string ='',
        public categoryId: number | null = null,
        public subCategoryId: number | null = null,
        public statusId: number | null = null,
        public approvalStatusId: number | null = null,

        public minPrice: number | null = null,
        public maxPrice: number | null = null,

        public minimuQuantityPerUser: number | null = null,
        public addedByVendorUserId: number | null = null,

        public isReturn: boolean | null = true,
        public isExchange: boolean | null = true,

        public mainProductSubCategoryAttributeId: number | null = null,

        public minAvailableQuantity: number | null = null,
        public minReservedQuantity: number | null = null,
        public maxAvailableQuantity: number | null = null,
        public maxReservedQuantity: number | null = null,

        public hasIssues: boolean | null = null,
        public isAvailableForSale: boolean | null = null
    ) { }
}