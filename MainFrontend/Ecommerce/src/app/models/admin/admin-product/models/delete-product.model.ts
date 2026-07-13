export class AdminDeleteProductModel {
    constructor(
        public productId: number = 0,
        public remark: string = ""
    ) { }
}

export class AdminDeleteVariantModel {
    constructor(
        public productVariantId: number = 0,
        public remark: string = ""
    ) { }
}