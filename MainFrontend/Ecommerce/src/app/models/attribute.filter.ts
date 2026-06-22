export class AttributeFilter {
    constructor(
        public attributeName: string | null = null,
        public status: boolean | null = null,
        public addedByAdminId: number | null = null
    ) {}
}