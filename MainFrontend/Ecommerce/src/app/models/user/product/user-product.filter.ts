export interface UserProductFilter {
  pageNumber: number;
  pageSize: number;
  searchTerm?: string | null;
  productCategoryId?: number | null;
  productSubCategoryId?: number | null;
  minPrice?: number | null;
  maxPrice?: number | null;
}