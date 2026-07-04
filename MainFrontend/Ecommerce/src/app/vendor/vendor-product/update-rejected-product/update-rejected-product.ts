import { Component, computed, signal } from '@angular/core';
import { VendorProductService } from '../../../services/vendor-product.Service';
import { PagedResponse } from '../../../models/paged-response.model';
import { VendorProductModel } from '../../../models/vendor/vendor-product/response/vendor-product.model';
import { VendorProductFilter } from '../../../models/vendor/vendor-product/filter/vendor-product.filter';
import { AdminProductCategoryModel } from '../../../models/admin/admin-product-category/response/admin-category';
import { AdminProductSubCategoryModel } from '../../../models/admin/admin-product-category/response/admin-subcategory.model';
import { UpdateRejectedProductModel } from '../../../models/vendor/vendor-product/add-model/update-rejected-product.model';
import { form, FormField, min, required } from '@angular/forms/signals';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { AdminMappedAttributeModel } from '../../../models/admin/admin-product-category/response/admin-mapped.model';
import { MappedAttributeFilter } from '../../../models/admin/admin-product-category/filter-models/mapped-attribute.filter';

@Component({
  selector: 'app-update-rejected-product',
  imports: [FormField, ReactiveFormsModule, FormsModule],
  templateUrl: './update-rejected-product.html',
  styleUrl: './update-rejected-product.css',
})
export class UpdateRejectedProduct {
  products = signal<PagedResponse<VendorProductModel> | null>(null);
  searchTerm = signal<string>('');
  productCategoryId = signal<number | null>(null);
  productSubCategoryId = signal<number | null>(null);
  productApprovalStatusId = signal<number | null>(null);
  productStatusId = signal<number | null>(null);
  minPrice = signal<number | null>(null);
  maxPrice = signal<number | null>(null);
  hasIssues = signal<boolean | null>(null);
  isAvailableForSale = signal<boolean | null>(null);
  productName = signal<string>('');
  addedByVendorUserId = signal<number | null>(null);
  minAvailableQuantity = signal<number | null>(null);
  maxAvailableQuantity = signal<number | null>(null);
  minReservedQuantity = signal<number | null>(null);
  maxReservedQuantity = signal<number | null>(null);
  mainProductSubCategoryAttributeId = signal<number | null>(null);
  showUpdatePopup = signal(false);

  pageNumber = signal<number>(1);
  pageSize = signal<number>(10);
  totalPages = computed(() => this.products()?.totalPages ?? 1);

  filterPanelOpen = signal<boolean>(false);
  categories = signal<AdminProductCategoryModel[]>([]);
  subCategories = signal<AdminProductSubCategoryModel[]>([]);

  filtererrorMessage = signal<string | null>(null);
  filterapplied = signal(false);

  updateProductModel = signal(new UpdateRejectedProductModel());
  successMessage = signal<string | null>(null);
  errorMessage = signal<string | null>(null);
  updateCategoryId = signal<number | null>(null);
  updateSubCategoryId = signal<number | null>(null);

  updateSubCategories = signal<AdminProductSubCategoryModel[]>([]);
  allAttributes = signal<AdminMappedAttributeModel[]>([]);
  filteredAttributes = signal<AdminMappedAttributeModel[]>([]);


  approvalStatusOptions = [
    { id: 1, label: 'Pending' },
    { id: 2, label: 'Vendor Approved' },
    { id: 3, label: 'Vendor Rejected' },
    { id: 4, label: 'Admin Approved' },
    { id: 5, label: 'Admin Rejected' },
    { id: 6, label: 'Deleted By Admin' },
  ];

  productStatusOptions = [
    { id: 1, label: 'Draft' },
    { id: 2, label: 'Active' },
    { id: 3, label: 'Temporarily Not Available' },
    { id: 4, label: 'Archived' },
  ];
  constructor(private vendorProductservice: VendorProductService) {

  }
  ngOnInit(): void {
    this.loadRejectedProduct();
    this.loadCategories();
  }
  loadRejectedProduct() {
    this.vendorProductservice.getProduct(this.buildFilter()).subscribe({
      next: (response: any) => {
        console.log(response);
        this.products.set(response);
      },
      error: (error) => {
        console.error(error);
      }
    })
  }
  private buildFilter(): VendorProductFilter {
    return {
      productName: this.productName() || null,
      productCategoryId: this.productCategoryId(),
      productSubCategoryId: this.productSubCategoryId(),
      productApprovalStatusId: 5,
      productStatusId: this.productStatusId(),
      addedByVendorUserId: this.addedByVendorUserId(),
      minPrice: this.minPrice(),
      maxPrice: this.maxPrice(),
      searchTerm: this.searchTerm() || null,
      hasIssues: this.hasIssues(),
      isAvailableForSale: this.isAvailableForSale(),
      minAvailableQuantity: this.minAvailableQuantity(),
      maxAvailableQuantity: this.maxAvailableQuantity(),
      minReservedQuantity: this.minReservedQuantity(),
      maxReservedQuantity: this.maxReservedQuantity(),
      mainProductSubCategoryAttributeId: this.mainProductSubCategoryAttributeId(),
      pageNumber: this.pageNumber(),
      pageSize: this.pageSize(),
    }
  }
  toggleFilterPanel(): void {
    const wasOpen = this.filterPanelOpen();
    this.filterPanelOpen.update((open) => !open);
    if (wasOpen && !this.filterapplied()) {
      this.resetFilters();
    }
  }

  closeFilterPanel(): void {
    this.filterPanelOpen.set(false);
  }

  applyFilters(): void {
    if (this.filtererrorMessage()) {
      return;
    }
    this.filterapplied.set(true);
    this.pageNumber.set(1);
    this.loadRejectedProduct();
    this.closeFilterPanel();
  }


  resetFilters(): void {
     this.filtererrorMessage.set("");
    this.filterapplied.set(false);
    this.productName.set('');
    this.searchTerm.set('');
    this.productCategoryId.set(null);
    this.productSubCategoryId.set(null);
    this.productApprovalStatusId.set(null);
    this.productStatusId.set(null);
    this.addedByVendorUserId.set(null);
    this.minPrice.set(null);
    this.maxPrice.set(null);
    this.hasIssues.set(null);
    this.isAvailableForSale.set(null);
    this.minAvailableQuantity.set(null);
    this.maxAvailableQuantity.set(null);
    this.minReservedQuantity.set(null);
    this.maxReservedQuantity.set(null);
    this.mainProductSubCategoryAttributeId.set(null);
    this.pageNumber.set(1);
    this.loadRejectedProduct();
    this.closeFilterPanel();
  }

  goToPage(page: number): void {
    if (page < 1 || page > this.totalPages()) return;
    this.pageNumber.set(page);
    this.loadRejectedProduct();
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
    this.loadRejectedProduct();
  }
  onSearchTermInput(event: Event): void {
    this.searchTerm.set((event.target as HTMLInputElement).value);
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

  loadCategories(): void {
    this.vendorProductservice.getProductCategory().subscribe({
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
      this.vendorProductservice.getSubCategory(id).subscribe({
        next: (res: any) => this.subCategories.set(res.items ?? res),
        error: (err) => console.log(err)
      });
    }
  }
  onSubcategoryChange(event: Event): void {
    const v = (event.target as HTMLSelectElement).value;
    this.productSubCategoryId.set(v ? Number(v) : null);
  }
  updateRejectedForm = form(this.updateProductModel, (path) => {
    required(path.productName, {
      message: 'Product name is required'
    });

    required(path.description, {
      message: 'Description is required'
    });

    required(path.productSubCategoryId, {
      message: 'Sub category is required'
    });

    min(path.productSubCategoryId, 1, {
      message: 'Invalid sub category'
    });

    required(path.mainProductSubCategoryAttributeId, {
      message: 'Main attribute is required'
    });

    min(path.mainProductSubCategoryAttributeId, 1, {
      message: 'Invalid attribute'
    });
  });
  loadAttributes(subCategoryId: number, product?: VendorProductModel): void {

    const request = new MappedAttributeFilter();
    request.productSubCategoryId = subCategoryId;
    request.status = true;

    this.vendorProductservice.getmappedAttribute(subCategoryId).subscribe({
      next: (res: any) => {

        const data = res.items ?? res;

        this.allAttributes.set(data);

        console.log("ALL ATTRIBUTES:", data);
        const attribute = data.find(
          (x: AdminMappedAttributeModel) =>
            x.attributeName.trim().toLowerCase() ===
            product?.mainProductSubCategoryAttributeName?.trim().toLowerCase()
        );

        console.log("MATCHED:", attribute);

        this.updateProductModel.set(
          new UpdateRejectedProductModel(
            product!.productId,
            product!.productName,
            product!.description,
            subCategoryId,
            attribute?.productSubCategoryAttributeId ?? 0
          )
        );

        this.showUpdatePopup.set(true);

      },
      error: err => console.log(err)
    });
  }
  openUpdatePopup(product: VendorProductModel) {

    const category = this.categories().find(
      c => c.productCategoryName === product.productCategoryName
    );


    if (!category) {
      return;
    }

    this.updateCategoryId.set(Number(category.productCategoryId));
    this.vendorProductservice.getSubCategory(category.productCategoryId).subscribe({
      next: (res: any) => {

        this.updateSubCategories.set(res.items ?? res);

        const subCategory = this.updateSubCategories().find(
          s => s.productSubCategoryName === product.productSubCategoryName
        );

        if (!subCategory) {
          return;
        }
        this.loadAttributes(subCategory.productSubCategoryId, product);

      }
    });
  }
  closeUpdatePopup() {

    this.showUpdatePopup.set(false);

    this.updateProductModel.set(
      new UpdateRejectedProductModel()
    );

    this.errorMessage.set(null);
  }
  updateRejectedProduct() {

    if (this.updateRejectedForm().invalid()) {
      this.errorMessage.set("Enter valid details");
      return;
    }

    this.vendorProductservice
      .updateRejectedProduct(this.updateProductModel())
      .subscribe({

        next: () => {

          this.successMessage.set("Product updated successfully");

          this.closeUpdatePopup();

          this.loadRejectedProduct();
        },

        error: err => {

          this.errorMessage.set(
            err.error?.message ?? "Failed to update product"
          );

        }

      });

  }
  onUpdateCategoryChange(event: Event) {

    const id = Number((event.target as HTMLSelectElement).value);

    this.updateCategoryId.set(id);

    this.vendorProductservice.getSubCategory(id).subscribe({
      next: (res: any) => {

        this.updateSubCategories.set(res.items ?? res);

        this.updateProductModel.update(model => ({
          ...model,
          productSubCategoryId: 0,
          mainProductSubCategoryAttributeId: 0
        }));

        this.filteredAttributes.set([]);
      }
    });
  }
  onUpdateSubCategoryChange(event: Event) {

    const id = Number((event.target as HTMLSelectElement).value);

    this.updateProductModel.update(model => ({
      ...model,
      productSubCategoryId: id,
      mainProductSubCategoryAttributeId: 0
    }));

    this.filteredAttributes.set(
      this.allAttributes().filter(
        x => x.productSubCategoryId === id
      )
    );
  }
  onUpdateAttributeChange(event: Event) {

    const id = Number((event.target as HTMLSelectElement).value);

    this.updateProductModel.update(model => ({
      ...model,
      mainProductSubCategoryAttributeId: id
    }));
  }
}
