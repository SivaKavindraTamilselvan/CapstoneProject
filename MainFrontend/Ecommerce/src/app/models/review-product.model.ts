export class ReviewProductModel{
    constructor(
        public productId : number = 0,
        public approvalStatusId : string = "",
        public remark : string = ""
    ){}
}