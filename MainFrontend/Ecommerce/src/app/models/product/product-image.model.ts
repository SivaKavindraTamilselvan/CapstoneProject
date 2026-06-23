export class ProductImageModel{
    constructor(
        public productImageId: number,
        public imageUrl: string = "",
        public displayOrder: number,
        public isMainImage: boolean,
    ){}
}