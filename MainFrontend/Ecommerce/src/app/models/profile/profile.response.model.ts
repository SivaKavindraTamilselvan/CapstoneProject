export class ProfileResponseModel {
    constructor(
        public userId: number = 0,
        public firstName: string = '',
        public lastName: string = '',
        public email: string = '',
        public phoneNumber: string = '',
        public isActive: boolean = true,
        public createdAt: Date | null = null,
    ) { }
}