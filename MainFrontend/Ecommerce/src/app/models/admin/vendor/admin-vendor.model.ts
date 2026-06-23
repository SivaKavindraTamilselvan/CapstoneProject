export class AdminVendorModel{
    constructor(
        public vendorId : number,
        public contactPersonName : string = "",
        public companyEmail : string = "",
        public companyPhoneNumber : string ="",
        public vendorCompanyName : string = "",
        public gstNumber : string = "",
        public approvalStatusId : number,
        public approvalStatusName :string = "",
        public isActive : boolean,
        public reviewedByAdminId : number,
        public reviewAdminName : string = ""
    ){}
}