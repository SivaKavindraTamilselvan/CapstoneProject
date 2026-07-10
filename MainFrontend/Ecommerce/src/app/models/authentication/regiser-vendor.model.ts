import { RegisterModel } from "./register-user.model";

export class RegisterVendorModel {
    constructor(
        public requestRegisterUserDTO: RegisterModel = new RegisterModel(),
        public contactPersonName: string = "",
        public companyEmail: string = "",
        public companyPhoneNumber: string = "",
        public vendorCompanyName: string = "",
        public gstNumber: string = ""
    ) { }
}