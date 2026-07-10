export class AddAddressModel {
  constructor(
    public contactName: string = '',
    public contactPhoneNumber: string = '',
    public addressLine: string = '',
    public landMark: string = '',
    public city: string = '',
    public state: string = '',
    public pinCode: string = '',
    public isDefault: boolean = false
  ) {}
}