export class ReviewVendorModel{
    constructor(
        public vendorId : number = 0,
        public approvalStatusId: number | null = null,
        public remark : string = ""
    ){}
}