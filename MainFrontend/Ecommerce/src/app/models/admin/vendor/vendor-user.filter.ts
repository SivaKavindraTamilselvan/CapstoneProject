export class AdminVendorUserFilter {
  constructor(
    public vendorId: number | null = null,
    public vendorRoleId: number | null = null,
    public isActive: boolean | null = null,
    public pageNumber: number = 1,
    public pageSize: number = 10
  ) {}
}