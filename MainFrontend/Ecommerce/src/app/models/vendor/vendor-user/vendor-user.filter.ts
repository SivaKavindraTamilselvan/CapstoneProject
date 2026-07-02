export class VendorUserFilter {
  constructor(
    public vendorRoleId: number | null = null,
    public status: boolean | null = null,
    public email : string = '',
    public phoneNumber : string ='',
    public pageNumber: number = 1,
    public pageSize: number = 10
  ) {}
}