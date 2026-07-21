import { Component, input, output, signal } from '@angular/core';
import { VendorProductService } from '../../../services/vendor-product.Service';
import { UpdateProductStatus } from '../../../models/vendor/vendor-product/add-model/update-product-status.model';
import { FormField } from '@angular/forms/signals';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { UpdateProductVariantStatus } from '../../../models/vendor/vendor-product/add-model/update-variant-status.model';

@Component({
  selector: 'app-delete-product-component',
  imports: [],
  templateUrl: './delete-product-componentd.html',
  styleUrl: './delete-product-componentd.css',
})
export class DeleteProductComponent {
  constructor(private vendorProductService: VendorProductService) { }

  productId = input<number | null>(null);

  mode = input<'product' | 'variant'>('product');

  closed = output<void>();
  deleted = output<void>();

  successMessage = signal<string | null>(null);
  errorMessage = signal<string | null>(null);
  progress = signal(false);

  delete() {
    if (this.mode() === 'product') {
      this.deleteProduct();
    } else {
      this.deleteProductVariant();
    }
  }

  deleteProduct() {
    const id = this.productId();
    if (id == null) return;

    this.progress.set(true);
    this.errorMessage.set(null);
    this.successMessage.set(null);

    const request = new UpdateProductStatus();
    request.productId = id;
    request.productStatusId = 4;
    this.vendorProductService.deleteProduct(request).subscribe({
      next: () => {
        this.successMessage.set("Product deleted successfully. Closing in 3 seconds...");
        setTimeout(() => {
          this.successMessage.set(null);
          this.deleted.emit();
          this.close();
          this.progress.set(false);
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

  deleteProductVariant() {
    const id = this.productId();
    if (id == null) return;

    this.progress.set(true);
    this.errorMessage.set(null);
    this.successMessage.set(null);

    const request = new UpdateProductVariantStatus();
    request.productVariantId = id;
    request.productStatusId = 4;
    this.vendorProductService.updateProductVariant(request).subscribe({
      next: () => {
        this.progress.set(false);
        this.successMessage.set("Product Variant deleted successfully. Closing in 3 seconds...");
        setTimeout(() => {
          this.successMessage.set(null);
          this.deleted.emit();
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
  close() {
    this.errorMessage.set(null);
    this.successMessage.set(null);
    this.closed.emit();
  }
}
