export class UpdateProductVariantStatus {
    constructor(
        public productVariantId: number | null = null,
        public productStatusId: number |null = null
    ) { }
}