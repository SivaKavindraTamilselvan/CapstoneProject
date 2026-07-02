export class AdminVendorModel{
    constructor(
        public vendorId : number = 0,
        public contactPersonName : string = "",
        public companyEmail : string = "",
        public companyPhoneNumber : string ="",
        public vendorCompanyName : string = "",
        public gstNumber : string = "",
        public approvalStatusId : number = 0,
        public approvalStatusName :string = "",
        public isActive : boolean =true,
        public reviewedByAdminId : number =0 ,
        public reviewAdminName : string = "",
        public createdAt : string = ""
    ){}
}