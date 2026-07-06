export class AdminOrderFilter {
  constructor(
    public pageNumber: number = 1,
    public pageSize: number = 10,
    public orderNumber: string ='',
    public fromDate: string | null = null,
    public toDate: string | null = null,
    public orderStatusId: number | null = null,
    public minAmount: number | null = null,
    public maxAmount: number | null = null,
    public userId: number | null = null,
    public vendorId: number | null = null
  ) {}
}