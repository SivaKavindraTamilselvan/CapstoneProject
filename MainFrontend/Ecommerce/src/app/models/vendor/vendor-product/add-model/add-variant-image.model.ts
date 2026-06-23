export class AddProductVariantImageModel {
    constructor(
        public productVariantId: number = 0,
        public imageUrl: string = "",
        public displayOrderId: number = 1,
    ) { }
}