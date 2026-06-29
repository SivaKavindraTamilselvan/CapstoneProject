import { Component, signal } from '@angular/core';
import { Router } from '@angular/router';
import { VendorProductService } from '../../../services/vendor-product.Service';
import { AddProductVariantModel } from '../../../models/vendor/vendor-product/add-model/add-product-variant.model';
import { AddProductVariantImageModel } from '../../../models/vendor/vendor-product/add-model/add-variant-image.model';
import { AddProductVariantAttributeModel } from '../../../models/vendor/vendor-product/add-model/add-variant-attribute.model';
import { form, FormField, min, required } from '@angular/forms/signals';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';

@Component({
  selector: 'app-add-variant',
  imports: [FormField, ReactiveFormsModule, FormsModule],
  templateUrl: './add-variant.html',
  styleUrl: './add-variant.css',
})
export class AddVariant {
  errorMessage = signal<string | null>(null);
  successMessage = signal<string | null>(null);

  productVariant = signal(new AddProductVariantModel());
  productVariantImages = signal<AddProductVariantImageModel[]>([]);

  constructor(
    private route: Router,
    private vendorProductService: VendorProductService
  ) {}

  addForm = form(this.productVariant, (path) => {
    required(path.productId, { message: 'Product is required' });
    min(path.productId, 1, { message: 'Product ID must be greater than 0' });

    required(path.price, { message: 'Price is required' });
    min(path.price, 1, { message: 'Price must be greater than 0' });

    required(path.weightInKgs, { message: 'Weight is required' });
    min(path.weightInKgs, 0.001, { message: 'Weight must be greater than 0' });

    required(path.lengthInCm, { message: 'Length is required' });
    min(path.lengthInCm, 0.1, { message: 'Length must be greater than 0' });

    required(path.widthInCm, { message: 'Width is required' });
    min(path.widthInCm, 0.1, { message: 'Width must be greater than 0' });

    required(path.heightInCm, { message: 'Height is required' });
    min(path.heightInCm, 0.1, { message: 'Height must be greater than 0' });

    required(path.minimuQuantityPerUser, {
      message: 'Minimum quantity is required',
    });
    min(path.minimuQuantityPerUser, 1, {
      message: 'Minimum quantity must be at least 1',
    });
  });

  // --- Attribute management ---
  // FIX: was spreading into 'productVariantAttribute' but reading from 'v.attributes'
  // Both the spread key and the read key must match the actual model property name.
  addAttribute(): void {
    this.productVariant.update((v) => ({
      ...v,
      productVariantAttribute: [...v.productVariantAttribute, new AddProductVariantAttributeModel()],
    }));
  }

  removeAttribute(index: number): void {
    this.productVariant.update((v) => ({
      ...v,
      productVariantAttribute: v.productVariantAttribute.filter((_, i) => i !== index),
    }));
  }

  updateAttributeId(index: number, event: Event): void {
    const value = Number((event.target as HTMLInputElement).value);
    this.productVariant.update((v) => {
      const updated = [...v.productVariantAttribute];
      updated[index] = { ...updated[index], productSubCategoryAttributeId: value };
      return { ...v, productVariantAttribute: updated };
    });
  }

  updateAttributeValue(index: number, event: Event): void {
    const value = (event.target as HTMLInputElement).value;
    this.productVariant.update((v) => {
      const updated = [...v.productVariantAttribute];
      updated[index] = { ...updated[index], attributeValue: value };
      return { ...v, productVariantAttribute: updated };
    });
  }

  // --- Image management ---
  onImageSelected(event: Event): void {
    const input = event.target as HTMLInputElement;
    if (!input.files?.length) return;

    const startOrder = this.productVariantImages().length + 1;

    Array.from(input.files).forEach((file, index) => {
      const reader = new FileReader();
      reader.onload = () => {
        const image = new AddProductVariantImageModel();
        image.imageUrl = file.name;
        image.displayOrderId = startOrder + index;
        this.productVariantImages.update((imgs) => [...imgs, image]);
      };
      reader.readAsDataURL(file); // FIX: was missing — onload never fired without this
    });

    input.value = '';
  }

  removeImage(index: number): void {
    this.productVariantImages.update((imgs) =>
      imgs
        .filter((_, i) => i !== index)
        .map((img, i) => ({ ...img, displayOrderId: i + 1 }))
    );
  }

  // --- Submit ---
  addVariant(): void {
    this.errorMessage.set(null);
    this.successMessage.set(null);

    this.vendorProductService.addProductVariant(this.productVariant()).subscribe({
      next: (response: any) => {
        const variantId =
          response.productVariantId ??
          response.data?.productVariantId ??
          response.id;

        if (!variantId) {
          this.errorMessage.set('Variant added, but variant ID was not returned.');
          return;
        }

        this.addVariantImages(variantId);
      },
      error: (error) => {
        this.errorMessage.set(error.error?.message ?? 'Failed to add variant');
      },
    });
  }

  private addVariantImages(variantId: number): void {
    const images = this.productVariantImages().map((img) => ({
      ...img,
      productVariantId: variantId,
    }));

    if (images.length === 0) {
      this.successMessage.set('Variant added successfully');
      this.resetForm();
      return;
    }

    let uploadedCount = 0;

    images.forEach((image) => {
      this.vendorProductService.addProductVariantImage(image).subscribe({
        next: () => {
          uploadedCount++;
          if (uploadedCount === images.length) {
            this.successMessage.set('Variant and images added successfully');
            this.resetForm();
          }
        },
        error: (error) => {
          this.errorMessage.set(
            error.error?.message ?? 'Variant added, but image upload failed'
          );
        },
      });
    });
  }

  resetForm(): void {
    this.productVariant.set(new AddProductVariantModel());
    this.productVariantImages.set([]);
  }

  onIsReturnChange(event: Event): void {
    const checked = (event.target as HTMLInputElement).checked;
    this.productVariant.update((v) => ({ ...v, isReturn: checked }));
  }

  onIsExchangeChange(event: Event): void {
    const checked = (event.target as HTMLInputElement).checked;
    this.productVariant.update((v) => ({ ...v, isExchange: checked }));
  }
}