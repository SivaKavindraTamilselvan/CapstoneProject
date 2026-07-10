export class FavoriteItemModel {
    constructor(
        public favoritesItemsId: number = 0,
        public favoritesId: number = 0,
        public productName: string = '',
        public description: string = '',
        public productVariantId: number = 0,
        public productId: number = 0,
        public sku: string = '',
        public price: number = 0,
        public mainImageUrl : string = '',
    ) { }

}