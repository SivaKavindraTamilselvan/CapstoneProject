export class AddProductImageModel {
    constructor(
        public productId: number = 0,
        public file: File | null = null,
        public displayOrderId: number = 1,
        public isMainImage: boolean = false
    ) {}
}