export interface AdminProductFilter {
  pageNumber: number;
  pageSize: number;
  searchTerm?: string | null;
  vendorId?: number | null;
  productCategoryId?: number | null;
  productSubCategoryId?: number | null;
  productApprovalStatusId?: number | null;
  productStatusId?: number | null;
  minPrice?: number | null;
  maxPrice?: number | null;
  hasIssues?: boolean | null;
  isAvailableForSale?: boolean | null;
}