import { RegisterModel } from "./register-user.model";

export class RegisterVendorUserModel{
    constructor(public requestRegisterUserDTO : RegisterModel = new RegisterModel(),public vendorRoleId : string = ""){

    }
}