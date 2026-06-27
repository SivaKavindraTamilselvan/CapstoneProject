import { Component, signal, computed } from '@angular/core';
import { UserProductService } from '../../services/user-product.Service'; 
import { UserProductModel } from '../../models/user/product/user-product.model'; 
import { PagedResponse } from '../../models/paged-response.model'; 
import { UserProductFilter } from '../../models/user/product/user-product.filter'; 

@Component({
  selector: 'app-user-product',
  imports: [],
  templateUrl: './user-product.html',
  styleUrl: './user-product.css',
})
export class UserProduct {
  constructor(private userProductService: UserProductService) {

  }
  products = signal<PagedResponse<UserProductModel> | null>(null);
  searchTerm = signal<string>('');
  vendorId = signal<number | null>(null);
  productCategoryId = signal<number | null>(null);
  productSubCategoryId = signal<number | null>(null);
  minPrice = signal<number | null>(null);
  maxPrice = signal<number | null>(null);
  pageNumber = signal<number>(1);
  pageSize = signal<number>(10);
  filterPanelOpen = signal<boolean>(false);
  totalPages = computed(() => this.products()?.totalPages ?? 1);
  ngOnInit(): void {
    this.loadProduct();
  }


  toggleFilterPanel(): void {
    this.filterPanelOpen.update((open) => !open);
  }

  closeFilterPanel(): void {
    this.filterPanelOpen.set(false);
  }
  private buildFilter(): UserProductFilter {
    return {
      pageNumber: this.pageNumber(),
      pageSize: this.pageSize(),
      searchTerm: this.searchTerm() || null,
      productCategoryId: this.productCategoryId(),
      productSubCategoryId: this.productSubCategoryId(),
      minPrice: this.minPrice(),
      maxPrice: this.maxPrice(),
    };
  }

  loadProduct(): void {
    this.userProductService.getProduct(this.buildFilter()).subscribe({
      next: (response: any) => {
        this.products.set(response);
      },
      error: (error) => {
        console.log(error);
      },
    });
  }
  applyFilters(): void {
    this.pageNumber.set(1);
    this.loadProduct();
    this.closeFilterPanel();
  }

  resetFilters(): void {
    this.searchTerm.set('');
    this.vendorId.set(null);
    this.productCategoryId.set(null);
    this.productSubCategoryId.set(null);

    this.minPrice.set(null);
    this.maxPrice.set(null);

    this.pageNumber.set(1);
    this.loadProduct();
    this.closeFilterPanel();
  }

  goToPage(page: number): void {
    if (page < 1 || page > this.totalPages()) return;
    this.pageNumber.set(page);
    this.loadProduct();
  }

  nextPage(): void {
    this.goToPage(this.pageNumber() + 1);
  }

  previousPage(): void {
    this.goToPage(this.pageNumber() - 1);
  }

  onPageSizeChange(event: Event): void {
    const value = Number((event.target as HTMLSelectElement).value);
    this.pageSize.set(value);
    this.pageNumber.set(1);
    this.loadProduct();
  }

  onSearchTermInput(event: Event): void {
    this.searchTerm.set((event.target as HTMLInputElement).value);
  }

  onVendorIdInput(event: Event): void {
    const v = (event.target as HTMLInputElement).value;
    this.vendorId.set(v ? Number(v) : null);
  }

  onMinPriceInput(event: Event): void {
    const v = (event.target as HTMLInputElement).value;
    this.minPrice.set(v ? Number(v) : null);
  }

  onMaxPriceInput(event: Event): void {
    const v = (event.target as HTMLInputElement).value;
    this.maxPrice.set(v ? Number(v) : null);
  }
  getMainImage(product: any): string | null {
    if (product.productImages?.length) {
      const main = product.productImages.find((img: any) => img.isMainImage);
      return main?.imageUrl ?? product.productImages[0].imageUrl;
    }
    const variantWithImages = product.productVariants?.find((v: any) => v.productImages?.length);
    return variantWithImages?.productImages[0]?.imageUrl ?? null;
  }

  getDisplayPrice(product: any): string {
    const prices = product.productVariants?.map((v: any) => v.price) ?? [];
    if (!prices.length) return 'N/A';
    const min = Math.min(...prices);
    const max = Math.max(...prices);
    const format = (n: number) => '₹' + n.toLocaleString('en-IN');
    return min === max ? format(min) : `${format(min)} - ${format(max)}`;
  }
}
