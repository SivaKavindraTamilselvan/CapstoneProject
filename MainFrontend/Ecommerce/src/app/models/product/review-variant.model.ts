export class ReviewProductVariantModel{
    constructor(
        public productVariantId : number = 0,
        public approvalStatusId : string = "",
        public remark : string = ""
    ){}
}