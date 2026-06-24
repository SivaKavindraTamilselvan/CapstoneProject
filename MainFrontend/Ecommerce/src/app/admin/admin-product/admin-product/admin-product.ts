import { Component, OnInit, computed, signal } from '@angular/core';
import { PagedResponse } from '../../../models/paged-response.model';
import { ProductModel } from '../../../models/product/product.model';
import { Router } from '@angular/router';
import { AdminProductService } from '../../../services/admin-product.Service';
import { AdminProductFilter } from '../../../models/admin/admin-product/filter/admin-product.filter';
import { AdminDeleteProductModel } from '../../../models/admin/admin-product/models/delete-product.model';
import { form, FormField, required } from '@angular/forms/signals';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { AdminProductCategoryModel } from '../../../models/admin/admin-product-category/response/admin-category';
import { AdminProductSubCategoryModel } from '../../../models/admin/admin-product-category/response/admin-subcategory.model';

@Component({
  selector: 'app-admin-product',
  imports: [FormField, FormsModule, ReactiveFormsModule],
  templateUrl: './admin-product.html',
  styleUrl: './admin-product.css',
})
export class AdminProduct implements OnInit {
  products = signal<PagedResponse<ProductModel> | null>(null);

  searchTerm = signal<string>('');
  vendorId = signal<number | null>(null);
  addedId = signal<number | null>(null);
  productCategoryId = signal<number | null>(null);
  productSubCategoryId = signal<number | null>(null);
  productApprovalStatusId = signal<number | null>(null);
  productStatusId = signal<number | null>(null);
  minPrice = signal<number | null>(null);
  maxPrice = signal<number | null>(null);
  minAvailableQuantity = signal<number | null>(null);
  maxAvailableQuantity = signal<number | null>(null);
  minReservedQuantity = signal<number | null>(null);
  maxReservedQuantity = signal<number | null>(null);
  hasIssues = signal<boolean | null>(null);
  isAvailableForSale = signal<boolean | null>(null);

  pageNumber = signal<number>(1);
  pageSize = signal<number>(10);
  filterPanelOpen = signal<boolean>(false);

  showActivatePopup = signal(false);
  successMessage = signal<string | null>(null);
  errorMessage = signal<string | null>(null);
  deleteProductModel = signal(new AdminDeleteProductModel());
  selectedProductId = signal<number | null>(null);

  categories = signal<AdminProductCategoryModel[]>([]);
  subCategories = signal<AdminProductSubCategoryModel[]>([]);

  toggleFilterPanel(): void {
    this.filterPanelOpen.update((open) => !open);
  }

  closeFilterPanel(): void {
    this.filterPanelOpen.set(false);
  }

  totalPages = computed(() => this.products()?.totalPages ?? 1);

  approvalStatusOptions = [
    { id: 1, label: 'Pending' },
    { id: 2, label: 'Vendor Approved' },
    { id: 3, label: 'Vendor Rejected' },
    { id: 4, label: 'Admin Approved' },
    { id: 5, label: 'Admin Rejected' }
  ];

  productStatusOptions = [
    { id: 1, label: 'Draft' },
    { id: 2, label: 'Active' },
    { id: 3, label: 'Temporarily Not Available' },
    { id: 4, label: 'Archived' },
  ];

  constructor(private route: Router, private adminProductService: AdminProductService) { }

  ngOnInit(): void {
    this.loadProduct();
    this.loadCategories();
  }

  private buildFilter(): AdminProductFilter {
    return {
      addedId: this.addedId(),
      pageNumber: this.pageNumber(),
      pageSize: this.pageSize(),
      searchTerm: this.searchTerm() || null,
      vendorId: this.vendorId(),
      productCategoryId: this.productCategoryId(),
      productSubCategoryId: this.productSubCategoryId(),
      productApprovalStatusId: this.productApprovalStatusId(),
      productStatusId: this.productStatusId(),
      minPrice: this.minPrice(),
      maxPrice: this.maxPrice(),
      hasIssues: this.hasIssues(),
      isAvailableForSale: this.isAvailableForSale(),
      minAvailableQuantity: this.minAvailableQuantity(),
      maxAvailableQuantity: this.maxAvailableQuantity(),
      minReservedQuantity: this.minReservedQuantity(),
      maxReservedQuantity: this.maxReservedQuantity()
    };
  }

  loadProduct(): void {
    this.adminProductService.getProducts(this.buildFilter()).subscribe({
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
    this.productApprovalStatusId.set(null);
    this.productStatusId.set(null);
    this.minPrice.set(null);
    this.maxPrice.set(null);
    this.hasIssues.set(null);
    this.addedId.set(null);
    this.isAvailableForSale.set(null);
    this.minAvailableQuantity.set(null);
    this.maxAvailableQuantity.set(null);
    this.minReservedQuantity.set(null);
    this.maxReservedQuantity.set(null);
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
  onAdminIdInput(event: Event): void {
    const v = (event.target as HTMLInputElement).value;
    this.addedId.set(v ? Number(v) : null);
  }

  onApprovalStatusChange(event: Event): void {
    const v = (event.target as HTMLSelectElement).value;
    this.productApprovalStatusId.set(v ? Number(v) : null);
  }

  onProductStatusChange(event: Event): void {
    const v = (event.target as HTMLSelectElement).value;
    this.productStatusId.set(v ? Number(v) : null);
  }

  onMinAvailableInput(event: Event): void {
    const v = (event.target as HTMLInputElement).value;
    this.minAvailableQuantity.set(v ? Number(v) : null);
  }

  onMaxAvailableInput(event: Event): void {
    const v = (event.target as HTMLInputElement).value;
    this.maxAvailableQuantity.set(v ? Number(v) : null);
  }

  onMinReservedInput(event: Event): void {
    const v = (event.target as HTMLInputElement).value;
    this.minReservedQuantity.set(v ? Number(v) : null);
  }

  onMaxReservedInput(event: Event): void {
    const v = (event.target as HTMLInputElement).value;
    this.maxReservedQuantity.set(v ? Number(v) : null);
  }

  onMinPriceInput(event: Event): void {
    const v = (event.target as HTMLInputElement).value;
    this.minPrice.set(v ? Number(v) : null);
  }

  onMaxPriceInput(event: Event): void {
    const v = (event.target as HTMLInputElement).value;
    this.maxPrice.set(v ? Number(v) : null);
  }

  onHasIssuesChange(event: Event): void {
    this.hasIssues.set((event.target as HTMLInputElement).checked || null);
  }

  onAvailableForSaleChange(event: Event): void {
    this.isAvailableForSale.set((event.target as HTMLInputElement).checked || null);
  }
  deleteForm = form(this.deleteProductModel, (path) => {
    required(path.remark, { message: "Enter The Remarks" });
  })

  handleDelete() {
    this.errorMessage.set(null);
    this.successMessage.set(null);

    if (this.deleteForm().invalid()) {
      this.errorMessage.set("Enter proper details");
      return;
    }

    const request = {
      productId: this.deleteProductModel().productId,
      remark: this.deleteProductModel().remark
    };

    this.adminProductService.deleteProduct(request).subscribe({
      next: () => {
        this.successMessage.set("Product deleted successfully. Closing in 3 seconds...");
        setTimeout(() => {
          this.closePopup();
          this.successMessage.set(null);
          this.loadProduct();
        }, 3000);
      },
      error: (error) => {
        this.successMessage.set(null);

        if (error.status === 400 && error.error?.errors) {
          const messages = Object.values(error.error.errors)
            .flat()
            .join(", ");

          this.errorMessage.set(messages);
        }
        else {
          this.errorMessage.set(
            error.error?.message ?? "Something went wrong. Please try again."
          );
        }
      }
    });
  }

  openDeletePopup(productId: number) {
    this.selectedProductId.set(productId);

    this.deleteProductModel.update(model => ({
      ...model,
      productId: productId,
      remark: ''
    }));

    this.showActivatePopup.set(true);
  }

  closePopup() {
    this.showActivatePopup.set(false);
    this.selectedProductId.set(null);
    this.deleteProductModel.set(new AdminDeleteProductModel());
    this.errorMessage.set(null);
  }
  loadCategories(): void {
    this.adminProductService.getProductCategory().subscribe({
      next: (res: any) => {
        this.categories.set(res.items ?? res);
        console.log(this.categories);
      },
      error: (err) => console.log(err)
    });
  }

  onCategoryChange(event: Event): void {
    const v = (event.target as HTMLSelectElement).value;
    const id = v ? Number(v) : null;
    this.productCategoryId.set(id);
    this.productSubCategoryId.set(null);         // reset subcategory when category changes
    this.subCategories.set([]);
    if (id) {
      this.adminProductService.getSubCategory(id).subscribe({
        next: (res: any) => this.subCategories.set(res.items ?? res),
        error: (err) => console.log(err)
      });
    }
  }
  onSubcategoryChange(event: Event): void {
    const v = (event.target as HTMLSelectElement).value;
    this.productSubCategoryId.set(v ? Number(v) : null);
  }
}