export class AdminUserModel {
    constructor(
        public adminUserId: number, 
        public userId: number, 
        public adminRoleId: number, 
        public adminRoleName: string = "", 
        public firstName: string = "", 
        public lastName: string = "", 
        public email: string = "", 
        public phoneNumber: string = "", 
        public isActive: boolean, 
        public createdAt: string = "") {}
}