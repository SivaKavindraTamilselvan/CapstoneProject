import { Component, computed, effect, signal } from '@angular/core';
import { PagedResponse } from '../../../models/paged-response.model';
import { VendorProductModel } from '../../../models/vendor/vendor-product/response/vendor-product.model';
import { ActivatedRoute, Router } from '@angular/router';
import { AdminProductService } from '../../../services/admin-product.Service';
import { VendorProductFilter } from '../../../models/vendor/vendor-product/filter/vendor-product.filter';
import { VendorProductService } from '../../../services/vendor-product.Service';
import { AdminProductCategoryModel } from '../../../models/admin/admin-product-category/response/admin-category';
import { AdminProductSubCategoryModel } from '../../../models/admin/admin-product-category/response/admin-subcategory.model';
import { AdminDeleteProductModel } from '../../../models/admin/admin-product/models/delete-product.model';
import { form, FormField, max, min, pattern, required } from '@angular/forms/signals';
import { PaginationComponent } from '../../../shared-components/pagination-component/pagination-component';
import { FilterComponent } from '../../../shared-components/filter-component/filter-component';
import { DataTableComponent } from '../../../shared-components/data-table-component/data-table-component';
import { MobileCardComponent } from '../../../shared-components/mobile-card-component/mobile-card-component';
import { Column } from '../../../shared-components/data-table-component/column.model';
import { TableAction } from '../../../shared-components/data-table-component/table-actions.model';
import { UpdateProductStatus } from '../../../models/vendor/vendor-product/add-model/update-product-status.model';
import { BasePage } from '../../../shared-class/shares-page-class';
import { ReviewProductModel } from '../../../models/product/review-product.model';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { UpdateRejectedProductModel } from '../../../models/vendor/vendor-product/add-model/update-rejected-product.model';
import { MappedAttributeFilter } from '../../../models/admin/admin-product-category/filter-models/mapped-attribute.filter';
import { AdminMappedAttributeModel } from '../../../models/admin/admin-product-category/response/admin-mapped.model';
import { AdminProductFilter } from '../../../models/admin/admin-product/filter/admin-product.filter';

@Component({
  selector: 'app-product-list',
  imports: [PaginationComponent, FilterComponent, DataTableComponent, MobileCardComponent, FormField, ReactiveFormsModule, FormsModule],
  templateUrl: './product-list.html',
  styleUrl: './product-list.css',
})
export class ProductList extends BasePage {
  actions = computed<TableAction<VendorProductModel>[]>(() => {
    if (this.pageTitle() == 'Update Product') {
      return [
        {
          label: 'View',
          color: 'green',
          action: 'view'
        },
        {
          label: 'Update',
          color: 'blue',
          action: 'update'
        },
      ];
    }
    if (this.status() === 1) {
      return [
        {
          label: 'View',
          color: 'green',
          action: 'view'
        },
        {
          label: 'Review',
          color: 'gray',
          action: 'review'
        },
      ];
    }
    if (this.status() === 6) {
      return [
        {
          label: 'View',
          color: 'green',
          action: 'view'
        },
        {
          label: 'Update',
          color: 'blue',
          action: 'update-rejected'
        },
      ];
    }
    if (this.deleted() == true) {
      return [
        {
          label: 'View',
          color: 'green',
          action: 'view'
        },

      ];
    }

    return [
      {
        label: 'View',
        color: 'green',
        action: 'view'
      },
      {
        label: 'Delete',
        color: 'red',
        action: 'delete',
      },
    ];
  });

  columns: Column[] = [
    {
      key: 'productId',
      header: 'ID'
    },
    {
      key: 'productName',
      header: 'Name'
    },
    {
      key: 'productCategoryName',
      header: 'Category'
    },
    {
      key: 'productSubCategoryName',
      header: 'SubCategory'
    },
    {
      key: 'productApprovalStatus',
      header: 'Approval',
    },
    {
      key: 'productStatus',
      header: 'Status'
    },

  ];

  mobileColumns: Column[] = [
    {
      key: 'productName',
      header: 'Name'
    },
    {
      key: 'productCategoryName',
      header: 'Category'
    },
    {
      key: 'productSubCategoryName',
      header: 'Sub Category'
    },
    {
      key: 'productApprovalStatus',
      header: 'Approval',
    },
    {
      key: 'productStatus',
      header: 'Status'
    },
  ];

  handleAction(event: { type: string; row: VendorProductModel }) {
    switch (event.type) {
      case 'view':
        this.viewProduct(event.row.productId);
        break;
      case 'delete':
        this.openDeletePopup(event.row.productId);
        break;
      case 'review':
        this.openReviewPopup(event.row.productId);
        break;
      case 'update':
        this.openUpdatePopup(event.row.productId);
        break;
      case 'update-rejected':
        this.openUpdateRejectedPopup(event.row);
        break;
    }
  }


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



  showActivatePopup = signal(false);
  successMessage = signal<string | null>(null);
  errorMessage = signal<string | null>(null);
  deleteProductModel = signal(new AdminDeleteProductModel());
  selectedProductId = signal<number | null>(null);

  categories = signal<AdminProductCategoryModel[]>([]);
  subCategories = signal<AdminProductSubCategoryModel[]>([]);

  filtererrorMessage = signal<string | null>(null);
  progress = signal(false);
  filterapplied = signal(false);

  deleteerrorMessage = signal<string | null>(null);
  deletesuccessMessage = signal<string | null>(null);


  totalPages = computed(() => this.products()?.totalPages ?? 1);

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

  adminProductFilter = signal(new VendorProductFilter());

  constructor(private router: ActivatedRoute, private route: Router, private vendorProductService: VendorProductService) {
    super();
    effect(() => {
      if (this.filterForm().invalid()) {
        this.filterErrorMessage.set('Please fix the validation errors.');
      } else {
        this.filterErrorMessage.set(null);
      }
    });
  }

  protected loadData(): void {
    this.loadProduct();
  }

  clearFilterValues(): void {
    this.productStatusId.set(null);
    this.productCategoryId.set(null);
    this.productSubCategoryId.set(null);;
    this.hasIssues.set(null);
    this.draftstatus.set(null);
    this.isAvailableForSale.set(null);
    this.subCategories.set([]);
    this.adminProductFilter.set(new VendorProductFilter());
    this.adminProductFilter.update(model => ({
      ...model,
      productStatusId: null,
      hasIssues: null,
      isAvailableForSale: null,
      productCategoryId: null,
      productSubCategoryId: null
    }));
  }
  draftstatus = signal<number | null>(null);

  status = signal<number | null>(null);
  deleted = signal<boolean | null>(null);
  pageTitle = signal<string | null>(null);

  ngOnInit(): void {
    this.router.data.subscribe(data => {
      this.status.set(data['status']);
      this.deleted.set(data['deleted']);
      this.pageTitle.set(data['title']);
      this.loadProduct();
      this.loadCategories();
    });
  }

  private buildFilters() {
    this.adminProductFilter.update(filter => ({
      ...filter,
      productApprovalStatusId: this.status() == null && this.deleted() == false ? this.draftstatus() : this.status(),
      includeIsDeleted: this.deleted(),
      pageNumber: this.pageNumber(),
      pageSize: this.pageSize(),
      searchTerm: filter.searchTerm.trim().toLowerCase(),
    }));
  }

  filterForm = form(this.adminProductFilter, (path) => {
    min(path.addedByVendorUserId, 1, { message: 'ID cannot be negative or 0.' });
    min(path.productCategoryId, 1, { message: 'ID cannot be negative or 0.' });
    min(path.productSubCategoryId, 1, { message: 'ID cannot be negative or 0.' });
    min(path.productApprovalStatusId, 1, { message: 'ID cannot be negative or 0.' });
    min(path.productStatusId, 1, { message: 'ID cannot be negative or 0.' });
    min(path.minPrice, 0, { message: 'Minimum price cannot be negative or 0.' });
    min(path.maxPrice, 0, { message: 'Maximum price cannot be negative or 0.' });
    min(path.minAvailableQuantity, 0, { message: 'Minimum available quantity cannot be negative.' });
    min(path.maxAvailableQuantity, 0, { message: 'Maximum available quantity cannot be negative.' });
    min(path.minReservedQuantity, 0, { message: 'Minimum reserved quantity cannot be negative.' });
    min(path.maxReservedQuantity, 0, { message: 'Maximum reserved quantity cannot be negative.' });
  });

  loadProduct(): void {
    this.buildFilters();
    this.vendorProductService.getProduct(this.adminProductFilter()).subscribe({
      next: (response: any) => {
        this.products.set(response);
      },
      error: (error) => {
        console.log(error);
        if (error.status === 404) {
          this.products.set({
            items: [],
            pageNumber: 1,
            pageSize: 10,
            totalCount: 0,
            totalPages: 0
          });
        }
        else if (error.status === 0) {
          this.errorMessage.set(
            'Unable to connect to the server. Please check your internet connection.'
          );
        }
        else if (error.status >= 500) {
          this.errorMessage.set(
            'Something went wrong on the server. Please try again later.'
          );
        }
        else if (error.error?.message) {
          this.errorMessage.set(error.error.message);
        }
        else {
          this.errorMessage.set('Failed to load products.');
        }
      },
    });
  }


  onApprovalStatusChange(event: Event): void {
    const v = (event.target as HTMLSelectElement).value;
    this.productApprovalStatusId.set(v ? Number(v) : null);
    this.draftstatus.set(v ? Number(v) : null);
  }

  onProductStatusChange(event: Event): void {
    const v = (event.target as HTMLSelectElement).value;
    this.productStatusId.set(v ? Number(v) : null);
    this.adminProductFilter.update(model => ({
      ...model,
      productStatusId: v ? Number(v) : null
    }));
  }

  onHasIssuesChange(event: Event): void {
    const checked = (event.target as HTMLInputElement).checked;
    this.hasIssues.set(checked);
    this.adminProductFilter.update(model => ({
      ...model,
      hasIssues: checked
    }));
  }

  onAvailableForSaleChange(event: Event): void {
    const checked = (event.target as HTMLInputElement).checked;

    this.isAvailableForSale.set(checked ? true : null);

    this.adminProductFilter.update(model => ({
      ...model,
      isAvailableForSale: checked
    }));
  }

  loadCategories(): void {
    this.vendorProductService.getProductCategory().subscribe({
      next: (res: any) => {
        this.categories.set(res.items ?? res);
      },
      error: (error) => {
        if (error.status === 0) {
          this.errorMessage.set(
            'Unable to load categories. Check your internet connection.'
          );
        }
        else {
          this.errorMessage.set(
            'Failed to load product categories.'
          );
        }
      }
    });
  }

  onCategoryChange(event: Event): void {
    const v = (event.target as HTMLSelectElement).value;
    const id = v ? Number(v) : null;
    this.adminProductFilter.update(model => ({
      ...model,
      productCategoryId: id,
      productSubCategoryId: null
    }));

    this.productCategoryId.set(id);
    this.productSubCategoryId.set(null);
    this.subCategories.set([]);
    if (id) {
      this.vendorProductService.getSubCategory(id).subscribe({
        next: (res: any) => this.subCategories.set(res.items ?? res),
        error: (error) => {
          if (error.status === 0) {
            this.errorMessage.set(
              'Unable to load subcategories. Check your internet connection.'
            );
          }
          else {
            this.errorMessage.set(
              'Failed to load product subcategories.'
            );
          }
        }
      });
    }
  }
  onSubcategoryChange(event: Event): void {
    const v = (event.target as HTMLSelectElement).value;
    this.productSubCategoryId.set(v ? Number(v) : null);
     this.adminProductFilter.update(model => ({
      ...model,
      productSubCategoryId: v ? Number(v) : null
    }));
  }

  openDeletePopup(productId: number) {
    this.selectedProductId.set(productId);
    this.showActivatePopup.set(true);
  }

  closeDeletePopup() {
    this.showActivatePopup.set(false);
    this.selectedProductId.set(null);
    this.errorMessage.set(null);
  }

  deleteProduct() {
    const id = this.selectedProductId();
    if (id == null) {
      return;
    }
    var request = new UpdateProductStatus();
    request.productId = id;
    request.productStatusId = 4;
    this.vendorProductService.deleteProduct(request).subscribe({
      next: (response: any) => {
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
    })
  }

  reviewProductModel = signal(new ReviewProductModel());
  viewProduct(productId: number) {
    this.route.navigate(['/vendor/products', productId]);
  }

  reviewerrorMessage = signal<string | null>(null);

  reviewForm = form(this.reviewProductModel, (path) => {
    required(path.approvalStatusId, { message: "Enter The Approval Status" });
    required(path.remark, { message: "Enter The Remarks" });
    pattern(path.approvalStatusId, /^[23]$/, { message: "Select valid approval status" })
  });

  openReviewPopup(productId: number) {
    this.selectedProductId.set(productId);

    this.reviewProductModel.set(
      new ReviewProductModel(productId, "", "")
    );

    this.showReviewPopup.set(true);
  }

  showReviewPopup = signal(false);
  showUpdatePopup = signal(false);
  showUpdateRejectedPopup = signal(false);



  closeReviewPopup() {
    this.showReviewPopup.set(false);
    this.selectedProductId.set(null);
    this.reviewProductModel.set(new ReviewProductModel());
    this.reviewerrorMessage.set(null);
    this.reviewForm().reset();
  }
  handleReview() {
    this.reviewerrorMessage.set(null);
    this.successMessage.set(null);
    if (this.reviewForm().invalid()) {
      this.reviewerrorMessage.set("Enter proper details");
      return;
    }
    const request = {
      productId: this.reviewProductModel().productId,
      approvalStatusId: Number(this.reviewProductModel().approvalStatusId),
      remark: this.reviewProductModel().remark
    };
    this.vendorProductService.reviewProduct(request).subscribe({
      next: () => {
        this.successMessage.set("Product reviewed successfully");
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

          this.reviewerrorMessage.set(messages);
        }
        else {
          this.reviewerrorMessage.set(
            error.error?.message ?? "Something went wrong. Please try again."
          );
        }
      }
    });
  }

  openUpdatePopup(productId: number) {
    this.selectedProductId.set(productId);

    this.updateProductModel.set(
      new UpdateProductStatus(productId, 0)
    );

    this.showUpdatePopup.set(true);
  }

  closeUpdatePopup() {
    this.showUpdatePopup.set(false);
    this.selectedProductId.set(null);
    this.updateProductModel.set(new UpdateProductStatus());
    this.updateerrorMessage.set(null);
  }
  updateProductModel = signal(new UpdateProductStatus());


  updateForm = form(this.updateProductModel, (path) => {
    required(path.productId, { message: "Enter The Approval Status" });
    required(path.productStatusId, { message: 'Enter The Approval Status' });
    min(path.productStatusId, 1, { message: 'Select valid approval status' });
    max(path.productStatusId, 4, { message: 'Select valid approval status' });
  });



  updateerrorMessage = signal<string | null>(null);

  handleUpdate() {
    this.updateerrorMessage.set(null);
    this.successMessage.set(null);
    if (this.updateForm().invalid()) {
      this.updateerrorMessage.set("Enter proper details");
      return;
    }
    const request = {
      productId: this.updateProductModel().productId,
      productStatusId: Number(this.updateProductModel().productStatusId),
    };
    this.vendorProductService.updateProduct(request).subscribe({
      next: () => {
        this.successMessage.set("Product updated successfully");
        setTimeout(() => {
          this.closeUpdatePopup();
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

          this.updateerrorMessage.set(messages);
        }
        else {
          this.updateerrorMessage.set(
            error.error?.message ?? "Something went wrong. Please try again."
          );
        }
      }
    });
  }
  onUpdateStatusChange(event: Event): void {
    const value = Number((event.target as HTMLSelectElement).value);

    this.updateProductModel.update((model) => ({
      ...model,
      productStatusId: value
    }));
  }
  updateRejectedProductModel = signal(new UpdateRejectedProductModel());

  updateCategoryId = signal<number | null>(null);
  updateSubCategoryId = signal<number | null>(null);
  updateSubCategories = signal<AdminProductSubCategoryModel[]>([]);
  updateRejectedForm = form(this.updateRejectedProductModel, (path) => {
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

  openUpdateRejectedPopup(product: VendorProductModel) {
    this.showUpdateRejectedPopup.set(true);


    const category = this.categories().find(
      c => c.productCategoryName === product.productCategoryName
    );


    if (!category) {
      return;
    }

    this.updateCategoryId.set(Number(category.productCategoryId));
    this.vendorProductService.getSubCategory(category.productCategoryId).subscribe({
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
  closeUpdateRejectedPopup() {

    this.showUpdateRejectedPopup.set(false);

    this.updateRejectedProductModel.set(
      new UpdateRejectedProductModel()
    );

    this.errorMessage.set(null);
  }
  updateRejectedProduct() {

    if (this.updateRejectedForm().invalid()) {
      this.errorMessage.set("Enter valid details");
      return;
    }

    this.vendorProductService
      .updateRejectedProduct(this.updateRejectedProductModel())
      .subscribe({

        next: () => {

          this.successMessage.set("Product updated successfully");

          this.closeUpdateRejectedPopup();

          this.loadProduct();
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

    this.vendorProductService.getSubCategory(id).subscribe({
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

  allAttributes = signal<AdminMappedAttributeModel[]>([]);
  filteredAttributes = signal<AdminMappedAttributeModel[]>([]);

  loadAttributes(subCategoryId: number, product?: VendorProductModel): void {

    const request = new MappedAttributeFilter();
    request.productSubCategoryId = subCategoryId;
    request.status = true;

    this.vendorProductService.getmappedAttribute(subCategoryId).subscribe({
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

        this.updateRejectedProductModel.set(
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
}
