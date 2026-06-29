export class ReviewProductVariantRequestModel{
    constructor(
        public productVariantId : number = 0,
        public approvalStatusId: number = 0,
        public remark : string = ""
    ){}
}