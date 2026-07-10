import { RegisterModel } from "./register-user.model";

export class RegisterAdminModel{
    constructor(public requestRegisterUserDTO : RegisterModel = new RegisterModel(),public adminRoleId : string = ""){

    }
}