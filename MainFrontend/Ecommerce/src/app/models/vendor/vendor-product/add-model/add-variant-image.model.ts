export class AddProductVariantImageModel {
    constructor(
        public productVariantId: number = 0,
        public file: File | null = null,
        public displayOrderId: number = 1,
    ) { }
}