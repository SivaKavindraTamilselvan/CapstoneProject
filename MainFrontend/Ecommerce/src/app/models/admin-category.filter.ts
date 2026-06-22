export interface AdminProductCategoryFilter {
  pageNumber: number;
  pageSize: number;
  ProductCategoryId?: number | null;
  ProductCategoryName? : string | null;
  AddedByAdminId?: number | null;
  status?: boolean | null;
}