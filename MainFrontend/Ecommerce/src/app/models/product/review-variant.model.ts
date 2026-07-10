export class ReviewProductVariantModel{
    constructor(
        public productVariantId : number = 0,
        public approvalStatusId : number | null = null,
        public remark : string = ""
    ){}
}