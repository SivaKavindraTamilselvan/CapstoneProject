import { Component, effect, input, output, signal, untracked } from '@angular/core';
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
  imports: [FormField, ReactiveFormsModule, FormsModule],
  templateUrl: './update-rejected-product-component.html',
  styleUrl: './update-rejected-product-component.css',
})
export class UpdateRejectedProductComponent {
  productCategoryId = signal<number | null>(null);
  productSubCategoryId = signal<number | null>(null);
  categories = signal<AdminProductCategoryModel[]>([]);
  subCategories = signal<AdminProductSubCategoryModel[]>([]);
  attributes = signal<AdminMappedAttributeModel[]>([]);


  constructor(private vendorProductService: VendorProductService) {
    effect(() => {
      const p = this.product();
      if (!p) return;
      untracked(() => {
        this.updateRejectedProductModel.set(
          new UpdateRejectedProductModel(
            p.productId,
            p.productName,
            p.description,
            p.productSubCategoryId,
            p.mainProductSubCategoryAttributeId
          )
        );
        this.loadCategories();
      });
    });
  }

  product = input<VendorProductModel | null>(null);

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

  attributeId = signal<number | null>(null);


loadCategories(): void {
    this.errorMessage.set(null);
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

  loadAttributes(): void {
    this.errorMessage.set(null);
    const id = this.productSubCategoryId();
    if (id == null) {
      return;
    }
    this.vendorProductService.getmappedAttribute(id).subscribe({
      next: (res: any) => {
        this.attributes.set(res.items ?? res);
        console.log(res);
      },
      error: (error) => {

        console.error(error);

        if (error.status === 0) {
          this.errorMessage.set(
            'Unable to load attributes. Check your internet connection.'
          );
        }
        else {
          this.errorMessage.set(
            'Failed to load attributes.'
          );
        }
      }
    });
  }

  onCategoryChange(event: Event): void {
    const v = (event.target as HTMLSelectElement).value;
    const id = v ? Number(v) : null;
    this.productCategoryId.set(id);
    this.productSubCategoryId.set(null);
    this.subCategories.set([]);
    if (id) {
      this.errorMessage.set(null);
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
    const value = Number((event.target as HTMLSelectElement).value);
    this.updateRejectedProductModel.update(product => ({
      ...product,
      productSubCategoryId: value
    }));
    this.productSubCategoryId.set(value);
    this.loadAttributes();
  }

  onAttributeChange(event: Event): void {
    const value = Number((event.target as HTMLSelectElement).value);
    this.updateRejectedProductModel.update(product => ({
      ...product,
      mainProductSubCategoryAttributeId: value
    }));
    this.attributeId.set(value);
  }

  updateRejectedProduct() {
    this.errorMessage.set(null);
    this.successMessage.set(null);

    if (this.updateRejectedForm().invalid()) {
      this.errorMessage.set('Enter valid details');
      return;
    }

    this.progress.set(true);
    this.vendorProductService.updateRejectedProduct(this.updateRejectedProductModel()).subscribe({
      next: () => {
        this.progress.set(false);
        this.successMessage.set('Product updated successfully');
        setTimeout(() => {
          this.successMessage.set(null);
          this.updated.emit();
          this.close();
        }, 2000);
      },
      error: err => {
        this.progress.set(false);
        this.errorMessage.set(err.error?.message ?? 'Failed to update product');
      }
    });
  }

  close() {
    this.updateRejectedProductModel.set(new UpdateRejectedProductModel());
    this.updateCategoryId.set(null);
    this.updateSubCategories.set([]);
    this.allAttributes.set([]);
    this.filteredAttributes.set([]);
    this.errorMessage.set(null);
    this.successMessage.set(null);
    this.closed.emit();
  }
}