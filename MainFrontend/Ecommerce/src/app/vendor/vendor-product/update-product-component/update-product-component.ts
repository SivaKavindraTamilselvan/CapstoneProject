import { Component, effect, input, output, signal } from '@angular/core';
import { UpdateProductStatus } from '../../../models/vendor/vendor-product/add-model/update-product-status.model';
import { VendorProductService } from '../../../services/vendor-product.Service';
import { form, FormField, max, min, required } from '@angular/forms/signals';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { UpdateProductVariantStatus } from '../../../models/vendor/vendor-product/add-model/update-variant-status.model';

@Component({
  selector: 'app-update-product-component',
  imports: [ReactiveFormsModule, FormsModule],
  templateUrl: './update-product-component.html',
  styleUrl: './update-product-component.css',
})
export class UpdateProductComponent {
  constructor(private vendorProductService: VendorProductService) {
    effect(() => {
      const id = this.productId();
      if (id == null) return;

      if (this.mode() === 'product') {
        this.productModel.set(new UpdateProductStatus(id, 0));
      } else {
        this.variantModel.set(new UpdateProductVariantStatus(id, 0));
      }
    });
  }

  productModel = signal(new UpdateProductStatus());
  variantModel = signal(new UpdateProductVariantStatus());

  productId = input<number | null>(null);
  mode = input<'product' | 'variant'>('product');

  closed = output<void>();
  updated = output<void>();

  successMessage = signal<string | null>(null);
  errorMessage = signal<string | null>(null);
  progress = signal(false);



  productStatusOptions = [
    { id: 1, label: 'Draft' },
    { id: 2, label: 'Active' },
    { id: 3, label: 'Temporarily Not Available' },
  ];

  productForm = form(this.productModel, path => {
    required(path.productId, { message: "Enter The Approval Status" });
    required(path.productStatusId, { message: 'Enter The Approval Status' });
    min(path.productStatusId, 1, { message: 'Select valid approval status' });
    max(path.productStatusId, 3, { message: 'Select valid approval status' });
  });

  variantForm = form(this.variantModel, path => {
    required(path.productVariantId, { message: "Enter The Approval Status" });
    required(path.productStatusId, { message: 'Enter The Approval Status' });
    min(path.productStatusId, 1, { message: 'Select valid approval status' });
    max(path.productStatusId, 3, { message: 'Select valid approval status' });
  });

  update() {
    if (this.mode() === 'product') {
      this.updateProduct();
    } else {
      this.updateVariant();
    }
  }

  onUpdateStatusChange(event: Event): void {
    const value = Number((event.target as HTMLSelectElement).value);

    if (this.mode() === 'product') {
      this.productModel.update(model => ({
        ...model,
        productStatusId: value
      }));
    }
    else {
      this.variantModel.update(model => ({
        ...model,
        productStatusId: value
      }));
    }
  }

  updateProduct() {
    this.errorMessage.set(null);
    this.successMessage.set(null);

    if (this.productForm().invalid()) {
      this.errorMessage.set("Enter proper details");
      return;
    }
    this.progress.set(true);
    const request = {
      productId: this.productModel().productId,
      productStatusId: Number(this.productModel().productStatusId),
    };
    this.vendorProductService.updateProduct(request).subscribe({
      next: () => {
        this.progress.set(false);
        this.successMessage.set("Product updated successfully");
        setTimeout(() => {
          this.successMessage.set(null);
          this.updated.emit();
          this.close();
        }, 3000);
      },
      error: (error) => {
        this.progress.set(false);
        this.successMessage.set(null);

        if (error.status === 400 && error.error?.errors) {
          const messages = Object.values(error.error.errors).flat().join(", ");
          this.errorMessage.set(messages);
        } else {
          this.errorMessage.set(error.error?.message ?? "Something went wrong. Please try again.");
        }
      }
    });
  }
  updateVariant() {
    this.errorMessage.set(null);
    this.successMessage.set(null);

    if (this.variantForm().invalid()) {
      this.errorMessage.set("Enter proper details");
      return;
    }

    this.progress.set(true);

    const request = {
      productVariantId: this.variantModel().productVariantId,
      productStatusId: Number(this.variantModel().productStatusId),
    };

    this.vendorProductService.updateProductVariant(request).subscribe({
      next: () => {
        this.progress.set(false);
        this.successMessage.set("Product Variant updated successfully");

        setTimeout(() => {
          this.successMessage.set(null);
          this.updated.emit();
          this.close();
        }, 3000);
      },
      error: (error) => {
        this.progress.set(false);
        this.successMessage.set(null);

        if (error.status === 400 && error.error?.errors) {
          const messages = Object.values(error.error.errors).flat().join(", ");
          this.errorMessage.set(messages);
        } else {
          this.errorMessage.set(
            error.error?.message ?? "Something went wrong. Please try again."
          );
        }
      }
    });
  }
  close() {
    this.productModel.set(new UpdateProductStatus());
    this.variantModel.set(new UpdateProductVariantStatus());
    this.errorMessage.set(null);
    this.successMessage.set(null);
    this.closed.emit();
  }
}
