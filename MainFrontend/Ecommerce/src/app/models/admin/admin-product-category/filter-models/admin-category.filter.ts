export class AdminProductCategoryFilter {
  constructor(
    public pageNumber: number = 1,
    public pageSize: number = 10,
    public ProductCategoryId: number | null = null,
    public ProductCategoryName: string = '',
    public AddedByAdminId: number | null = null,
    public status: boolean | null = null,
  ) {}
}