import { RegisterModel } from "./register-user.model";

export class RegisterAdminModel{
    constructor(public requestRegisterUserDTO : RegisterModel = new RegisterModel(),public adminRoleId : string = ""){

    }
}

export interface RequestSetPasswordDTO {
  token: string;
  newPassword: string;
}

export interface ResponseSetPasswordDTO {
  email: string;
  message: string;
}

export interface RequestResendInviteDTO {
  email: string;
}