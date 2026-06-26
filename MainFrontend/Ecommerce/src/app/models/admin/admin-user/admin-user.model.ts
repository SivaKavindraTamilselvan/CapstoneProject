export class AdminUserModel {
    constructor(
        public adminUserId: number = 0,
        public userId: number = 0,
        public adminRoleId: number = 0,
        public adminRoleName: string = "",
        public firstName: string = "",
        public lastName: string = "",
        public email: string = "",
        public phoneNumber: string = "",
        public isActive: boolean = true,
        public createdAt: string = "") { }
}