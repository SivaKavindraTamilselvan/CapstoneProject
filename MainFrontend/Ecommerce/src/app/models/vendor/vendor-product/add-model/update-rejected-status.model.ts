export class UpdateRejectedProductVariantModel {
    constructor(
        public productVariantId: number = 0,
        public productVariantStatusId: number = 0,
        public price: number = 0,
        public weightInKgs: number = 0,
        public lengthInCm: number = 0,
        public widthInCm: number = 0,
        public heightInCm: number = 0,
        public minimuQuantityPerUser: number = 0,
        public isReturn: boolean = true,
        public isExchange: boolean = true
    ) { }
}