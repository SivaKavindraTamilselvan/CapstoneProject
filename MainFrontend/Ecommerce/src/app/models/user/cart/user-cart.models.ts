export class CartItemModel {
    constructor(
        public cartItemsId: number = 0,
        public cartId: number = 0,
        public productName: string = '',
        public description: string = '',
        public productVariantId: number = 0,
         public productId: number = 0,
        public qunatity: number = 0,
        public sku: string = '',
        public price: number = 0,
        public mainImageUrl : string = '',
    ) { }

}