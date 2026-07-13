import { Component, signal, computed, effect } from '@angular/core';
import { UserProductService } from '../../services/user-product.Service';
import { UserProductModel } from '../../models/user/product/user-product.model';
import { PagedResponse } from '../../models/paged-response.model';
import { UserProductFilter } from '../../models/user/product/user-product.filter';
import { ActivatedRoute, Route, Router } from '@angular/router';
import { UserProductCategoryModel } from '../../models/user/product-category/user-product-category.model';
import { UserSubProductCategoryModel } from '../../models/user/product-category/user-sub-category.model';
import { ProductReviews } from './product-reviews/product-reviews';

@Component({
  selector: 'app-user-product',
  imports: [ProductReviews],
  templateUrl: './user-product.html',
  styleUrl: './user-product.css',
})
export class UserProduct {
  constructor(private userProductService: UserProductService, private router: ActivatedRoute, private route: Router) {
    effect(() => {
      const search = this.userProductService.navbarSearchTerm();

      this.searchTerm.set(search);
      this.pageNumber.set(1);
      this.loadProduct();
    });
  }
  products = signal<PagedResponse<UserProductModel> | null>(null);
  searchTerm = signal<string>('');
  productCategoryId = signal<number | null>(null);
  productSubCategoryId = signal<number | null>(null);
  minPrice = signal<number | null>(null);
  maxPrice = signal<number | null>(null);
  draftMinPrice = signal<number | null>(null);
  draftMaxPrice = signal<number | null>(null);
  draftproductCategoryId = signal<number | null>(null);
  draftproductSubCategoryId = signal<number | null>(null);
  pageNumber = signal<number>(1);
  pageSize = signal<number>(10);
  filterPanelOpen = signal<boolean>(false);
  totalPages = computed(() => this.products()?.totalPages ?? 1);

  categories = signal<UserProductCategoryModel[]>([]);
  subCategories = signal<UserSubProductCategoryModel[]>([]);

  ngOnInit(): void {

    this.router.paramMap.subscribe(params => {
      const subCategoryId = params.get('subCategoryId');

      this.productSubCategoryId.set(
        subCategoryId ? Number(subCategoryId) : null
      );

      this.pageNumber.set(1);
      this.loadProduct();
    });

    this.loadCategories();
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
        console.log(this.products());
      },
      error: (error) => {
        console.log(error);
      },
    });
  }
  applyFilters(): void {
    this.productCategoryId.set(this.draftproductCategoryId());
    this.productSubCategoryId.set(this.draftproductSubCategoryId());
    this.minPrice.set(this.draftMinPrice());
    this.maxPrice.set(this.draftMaxPrice());
    this.pageNumber.set(1);
    this.loadProduct();
    this.closeFilterPanel();
  }

  resetFilters(): void {
    this.searchTerm.set('');
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

  onMinPriceInput(event: Event): void {
    const v = (event.target as HTMLInputElement).value;
    this.draftMinPrice.set(v ? Number(v) : null);
  }

  onMaxPriceInput(event: Event): void {
    const v = (event.target as HTMLInputElement).value;
    this.draftMaxPrice.set(v ? Number(v) : null);
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

  goToProductDetails(productId: any) {
    this.route.navigate(['/user/product-details', productId]);
  }

  loadCategories(): void {
    this.userProductService.getProductCategory().subscribe({
      next: (res: any) => {
        this.categories.set(res.items ?? res);
      },
      error: (err) => console.log(err)
    });
  }

  onCategoryChange(event: Event): void {
    const v = (event.target as HTMLSelectElement).value;
    const id = v ? Number(v) : null;
    this.draftproductCategoryId.set(id);
    this.draftproductSubCategoryId.set(null);         // reset subcategory when category changes
    this.subCategories.set([]);
    if (id) {
      this.userProductService.getSubCategory(id).subscribe({
        next: (res: any) => this.subCategories.set(res.items ?? res),
        error: (err) => console.log(err)
      });
    }
  }
  onSubcategoryChange(event: Event): void {
    const v = (event.target as HTMLSelectElement).value;
    this.draftproductSubCategoryId.set(v ? Number(v) : null);
  }
}
