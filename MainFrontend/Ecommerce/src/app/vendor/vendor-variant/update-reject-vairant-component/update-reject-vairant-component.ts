import { Component, effect, input, output, signal } from '@angular/core';
import { VendorProductService } from '../../../services/vendor-product.Service';
import { UpdateRejectedProductVariantModel } from '../../../models/vendor/vendor-product/add-model/update-rejected-status.model';
import { VendorProductVariantModel } from '../../../models/vendor/vendor-product/response/vendor-variant.model';
import { form, FormField, min, required } from '@angular/forms/signals';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';

@Component({
  selector: 'app-update-reject-vairant-component',
  imports: [FormField,ReactiveFormsModule,FormsModule],
  templateUrl: './update-reject-vairant-component.html',
  styleUrl: './update-reject-vairant-component.css',
})
export class UpdateRejectVairantComponent {

  constructor(private vendorProductService: VendorProductService) {
    effect(() => {
      const v = this.variant();
      if (v) {
        this.updateModel.set(
          new UpdateRejectedProductVariantModel(
            v.productVariantId,
            v.price,
            v.weightInKgs,
            v.lengthInCm,
            v.widthInCm,
            v.heightInCm,
            v.minimuQuantityPerUser,
            v.isReturn,
            v.isExchange
          )
        );
      }
    });
  }

  variant = input<VendorProductVariantModel | null>(null);

  closed = output<void>();
  updated = output<void>();

  updateModel = signal(new UpdateRejectedProductVariantModel());
  successMessage = signal<string | null>(null);
  errorMessage = signal<string | null>(null);
  progress = signal(false);

  updateForm = form(this.updateModel, (path) => {
    required(path.productVariantId, { message: 'Variant is required' });
    required(path.price, { message: 'Price is required' });
    min(path.price, 0, { message: 'Price cannot be negative' });
    required(path.weightInKgs, { message: 'Weight is required' });
    min(path.weightInKgs, 0, { message: 'Weight cannot be negative' });
    required(path.lengthInCm, { message: 'Length is required' });
    min(path.lengthInCm, 0, { message: 'Length cannot be negative' });
    required(path.widthInCm, { message: 'Width is required' });
    min(path.widthInCm, 0, { message: 'Width cannot be negative' });
    required(path.heightInCm, { message: 'Height is required' });
    min(path.heightInCm, 0, { message: 'Height cannot be negative' });
    required(path.minimuQuantityPerUser, { message: 'Minimum quantity per user is required' });
    min(path.minimuQuantityPerUser, 1, { message: 'Minimum quantity per user must be at least 1' });
  });

  onToggleChange(field: 'isReturn' | 'isExchange', event: Event) {
    const checked = (event.target as HTMLInputElement).checked;
    this.updateModel.update(model => ({ ...model, [field]: checked }));
  }

  updateRejectedVariant() {
    this.errorMessage.set(null);
    this.successMessage.set(null);

    if (this.updateForm().invalid()) {
      this.errorMessage.set("Enter valid details");
      return;
    }

    this.progress.set(true);
    this.vendorProductService.updateRejectedVariant(this.updateModel()).subscribe({
      next: () => {
        
        this.successMessage.set("Variant updated successfully");
        setTimeout(() => {
          this.progress.set(false);
          this.successMessage.set(null);
          this.updated.emit();
          this.close();
        }, 2000);
      },
      error: (err) => {
        this.progress.set(false);
        this.successMessage.set(null);

        if (err.status === 400 && err.error?.errors) {
          const messages = Object.values(err.error.errors).flat().join(", ");
          this.errorMessage.set(messages);
        } else {
          this.errorMessage.set(err.error?.message ?? "Failed to update variant");
        }
      }
    });
  }

  close() {
    this.updateModel.set(new UpdateRejectedProductVariantModel());
    this.errorMessage.set(null);
    this.successMessage.set(null);
    this.closed.emit();
  }
}
