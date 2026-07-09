import { Component, effect, input, output, signal } from '@angular/core';
import { UpdateProductStatus } from '../../../models/vendor/vendor-product/add-model/update-product-status.model';
import { VendorProductService } from '../../../services/vendor-product.Service';
import { form, FormField, max, min, required } from '@angular/forms/signals';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';

@Component({
  selector: 'app-update-product-component',
  imports: [ReactiveFormsModule,FormsModule],
  templateUrl: './update-product-component.html',
  styleUrl: './update-product-component.css',
})
export class UpdateProductComponent {
  constructor(private vendorProductService: VendorProductService) {
    effect(() => {
      const id = this.productId();
      if (id != null) {
        this.updateProductModel.set(new UpdateProductStatus(id, 0));
      }
    });
  }

  productId = input<number | null>(null);

  closed = output<void>();
  updated = output<void>();

  updateProductModel = signal(new UpdateProductStatus());
  successMessage = signal<string | null>(null);
  errorMessage = signal<string | null>(null);
  progress = signal(false);

  productStatusOptions = [
    { id: 1, label: 'Draft' },
    { id: 2, label: 'Active' },
    { id: 3, label: 'Temporarily Not Available' },
    { id: 4, label: 'Archived' },
  ];
  updateForm = form(this.updateProductModel, (path) => {
    required(path.productId, { message: "Enter The Approval Status" });
    required(path.productStatusId, { message: 'Enter The Approval Status' });
    min(path.productStatusId, 1, { message: 'Select valid approval status' });
    max(path.productStatusId, 4, { message: 'Select valid approval status' });
  });

  onUpdateStatusChange(event: Event): void {
    const value = Number((event.target as HTMLSelectElement).value);
    this.updateProductModel.update(model => ({ ...model, productStatusId: value }));
  }

  handleUpdate() {
    this.errorMessage.set(null);
    this.successMessage.set(null);

    if (this.updateForm().invalid()) {
      this.errorMessage.set("Enter proper details");
      return;
    }

    this.progress.set(true);
    const request = {
      productId: this.updateProductModel().productId,
      productStatusId: Number(this.updateProductModel().productStatusId),
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
  close() {
    this.updateProductModel.set(new UpdateProductStatus());
    this.errorMessage.set(null);
    this.successMessage.set(null);
    this.closed.emit();
  }
}
