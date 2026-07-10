export class ReviewProductModel{
    constructor(
        public productId : number = 0,
        public approvalStatusId : number | null = null,
        public remark : string = ""
    ){}
}