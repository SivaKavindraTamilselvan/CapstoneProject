export class AddInventoryModel{
    constructor(
        public productVariantId : number = 0,
        public addressId : number =0,
        public availableQuantity : number = 0,
        public reservedQuantity : number = 0
    ){}
}