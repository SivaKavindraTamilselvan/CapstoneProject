import { Component, effect, input, output, signal } from '@angular/core';
import { VendorProductService } from '../../../services/vendor-product.Service';
import { VendorProductModel } from '../../../models/vendor/vendor-product/response/vendor-product.model';
import { AdminProductCategoryModel } from '../../../models/admin/admin-product-category/response/admin-category';
import { UpdateRejectedProductModel } from '../../../models/vendor/vendor-product/add-model/update-rejected-product.model';
import { AdminProductSubCategoryModel } from '../../../models/admin/admin-product-category/response/admin-subcategory.model';
import { AdminMappedAttributeModel } from '../../../models/admin/admin-product-category/response/admin-mapped.model';
import { form, FormField, min, required } from '@angular/forms/signals';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';

@Component({
  selector: 'app-update-rejected-product-component',
  imports: [FormField,ReactiveFormsModule,FormsModule],
  templateUrl: './update-rejected-product-component.html',
  styleUrl: './update-rejected-product-component.css',
})
export class UpdateRejectedProductComponent {
  constructor(private vendorProductService: VendorProductService) {
    effect(() => {
      const p = this.product();
      const cats = this.categories();
      if (p && cats.length) {
        this.initializeFromProduct(p, cats);
      }
    });
  }

  product = input<VendorProductModel | null>(null);
  categories = input<AdminProductCategoryModel[]>([]);

  closed = output<void>();
  updated = output<void>();

  updateRejectedProductModel = signal(new UpdateRejectedProductModel());
  successMessage = signal<string | null>(null);
  errorMessage = signal<string | null>(null);
  progress = signal(false);
  updateCategoryId = signal<number | null>(null);
  updateSubCategories = signal<AdminProductSubCategoryModel[]>([]);
  allAttributes = signal<AdminMappedAttributeModel[]>([]);
  filteredAttributes = signal<AdminMappedAttributeModel[]>([]);

  updateRejectedForm = form(this.updateRejectedProductModel, (path) => {
    required(path.productName, { message: 'Product name is required' });
    required(path.description, { message: 'Description is required' });
    required(path.productSubCategoryId, { message: 'Sub category is required' });
    min(path.productSubCategoryId, 1, { message: 'Invalid sub category' });
    required(path.mainProductSubCategoryAttributeId, { message: 'Main attribute is required' });
    min(path.mainProductSubCategoryAttributeId, 1, { message: 'Invalid attribute' });
  });
  private initializeFromProduct(product: VendorProductModel, categories: AdminProductCategoryModel[]) {
    const category = categories.find(c => c.productCategoryName === product.productCategoryName);
    if (!category) return;

    this.updateCategoryId.set(Number(category.productCategoryId));

    this.vendorProductService.getSubCategory(category.productCategoryId).subscribe({
      next: (res: any) => {
        this.updateSubCategories.set(res.items ?? res);

        const subCategory = this.updateSubCategories().find(
          s => s.productSubCategoryName === product.productSubCategoryName
        );
        if (!subCategory) return;

        this.loadAttributes(subCategory.productSubCategoryId, product);
      }
    });
  }
  private loadAttributes(subCategoryId: number, product: VendorProductModel): void {
    this.vendorProductService.getmappedAttribute(subCategoryId).subscribe({
      next: (res: any) => {
        const data = res.items ?? res;
        this.allAttributes.set(data);
        this.filteredAttributes.set(data);

        const attribute = data.find(
          (x: AdminMappedAttributeModel) =>
            x.attributeName.trim().toLowerCase() ===
            product.mainProductSubCategoryAttributeName?.trim().toLowerCase()
        );

        this.updateRejectedProductModel.set(
          new UpdateRejectedProductModel(
            product.productId,
            product.productName,
            product.description,
            subCategoryId,
            attribute?.productSubCategoryAttributeId ?? 0
          )
        );
      },
      error: err => console.log(err)
    });
  }
  onUpdateCategoryChange(event: Event) {
    const id = Number((event.target as HTMLSelectElement).value);
    this.updateCategoryId.set(id);

    this.vendorProductService.getSubCategory(id).subscribe({
      next: (res: any) => {
        this.updateSubCategories.set(res.items ?? res);
        this.updateRejectedProductModel.update(model => ({
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

    this.updateRejectedProductModel.update(model => ({
      ...model,
      productSubCategoryId: id,
      mainProductSubCategoryAttributeId: 0
    }));

    this.filteredAttributes.set(
      this.allAttributes().filter(x => x.productSubCategoryId === id)
    );
  }

  onUpdateAttributeChange(event: Event) {
    const id = Number((event.target as HTMLSelectElement).value);
    this.updateRejectedProductModel.update(model => ({
      ...model,
      mainProductSubCategoryAttributeId: id
    }));
  }
  updateRejectedProduct() {
    this.errorMessage.set(null);
    this.successMessage.set(null);

    if (this.updateRejectedForm().invalid()) {
      this.errorMessage.set("Enter valid details");
      return;
    }

    this.progress.set(true);
    this.vendorProductService.updateRejectedProduct(this.updateRejectedProductModel()).subscribe({
      next: () => {
        this.progress.set(false);
        this.successMessage.set("Product updated successfully");
        setTimeout(() => {
          this.successMessage.set(null);
          this.updated.emit();
          this.close();
        }, 2000);
      },
      error: err => {
        this.progress.set(false);
        this.errorMessage.set(err.error?.message ?? "Failed to update product");
      }
    });
  }
  close() {
    this.updateRejectedProductModel.set(new UpdateRejectedProductModel());
    this.errorMessage.set(null);
    this.successMessage.set(null);
    this.closed.emit();
  }
}
