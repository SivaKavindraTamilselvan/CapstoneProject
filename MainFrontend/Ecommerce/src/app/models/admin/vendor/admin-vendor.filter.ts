export class AdminVendorFilter {
  constructor(
    public contactPersonName: string = '',
    public companyEmail: string = '',
    public companyPhoneNumber: string = '',
    public vendorCompanyName: string = '',
    public gstNumber: string = '',
    public approvalStatusId: number | null = null,
    public isActive: boolean | null = null,
    public reviewedByAdminId: number | null = null,
    public pageNumber: number = 1,
    public pageSize: number = 10
  ) {}
}