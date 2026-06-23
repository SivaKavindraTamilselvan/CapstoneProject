export class AddProductImageModel {
    constructor(
        public productId: number = 0,
        public imageUrl: string = "",
        public displayOrderId: number = 1,
        public isMainImage: boolean = false
    ) {}
}