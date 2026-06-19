export class ReviewVendorModel{
    constructor(
        public vendorId : number = 0,
        public approvalStatusId : string = "",
        public remark : string = ""
    ){}
}