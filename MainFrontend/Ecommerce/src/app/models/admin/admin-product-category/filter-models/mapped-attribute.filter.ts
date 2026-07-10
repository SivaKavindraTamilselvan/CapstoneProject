export class MappedAttributeFilter {
    constructor(
        public productSubCategoryId: number | null = null,
        public status: boolean | null = null,
        public attributeMasterId: number | null = null,
        public addedByAdminId: number | null = null,
        public pageNumber: number = 1,
        public pageSize: number = 10
    ) {}
}