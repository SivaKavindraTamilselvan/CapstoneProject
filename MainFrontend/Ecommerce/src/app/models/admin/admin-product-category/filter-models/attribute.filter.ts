export class AttributeFilter {
    constructor(
        public attributeName: string ='',
        public status: boolean | null = null,
        public addedByAdminId: number | null = null,
        public pageNumber : number = 1,
        public pageSize : number = 10,
    ) {}
}