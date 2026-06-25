export class AddressModel {
    constructor(
        public addressId: number = 0,
        public contactName: string = '',
        public contactPhoneNumber: string = '',
        public addressLine: string = '',
        public landMark: string = '',
        public city: string = '',
        public state: string = '',
        public pinCode: string = '',
        public isDefault: boolean = false,
        public isActive: boolean = false,
        public createdAt : Date,
        public updateAt : Date
    ) { }
}