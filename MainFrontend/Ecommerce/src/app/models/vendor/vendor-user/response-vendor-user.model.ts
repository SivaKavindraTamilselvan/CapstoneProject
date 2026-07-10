export class VendorUserModel {
    constructor(
        public vendorUserId: number = 0,
        public userId: number = 0,
        public vendorRoleId: number = 0,
        public vendorRoleName: string = "",
        public firstName: string = "",
        public lastName: string = "",
        public email: string = "",
        public phoneNumber: string = "",
        public isActive: boolean = true,
        public createdAt: string = "") { }
}