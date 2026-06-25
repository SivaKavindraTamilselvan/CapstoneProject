export class AddressFilter{
    constructor(
        public contactPhoneNumber: string = '',    
        public city: string = '',
        public state: string = '',
        public pinCode: string = '',
        public isActive: boolean | null = null,
        public pageNumber: number,
        public pageSize: number,
    ){}
}