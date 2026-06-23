import { Component, signal } from '@angular/core';
import { VendorProductService } from '../../../services/vendor-product.Service';
import { AddProductModel } from '../../../models/vendor/vendor-product/add-model/add-product.model';
import { AddProductImageModel } from '../../../models/vendor/vendor-product/add-model/add-product-image';
import { form, FormField, maxLength, min, required } from '@angular/forms/signals';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';

@Component({
  selector: 'app-add-product',
  imports: [FormField,ReactiveFormsModule,FormsModule],
  templateUrl: './add-product.html',
  styleUrl: './add-product.css',
})
export class AddProduct {
  errorMessage = signal<string | null>(null);
  successMessage = signal<string | null>(null);

  product = signal(new AddProductModel());
  productImages = signal<AddProductImageModel[]>([]);

  constructor(private vendorProductService: VendorProductService) { }

  addForm = form(this.product, (path) => {
    required(path.productName, { message: 'Product name is required' });
    maxLength(path.productName, 100, {
      message: 'Product name must be 100 characters or fewer',
    });

    required(path.description, { message: 'Product description is required' });
    maxLength(path.description, 1000, {
      message: 'Description must be 1000 characters or fewer',
    });

    required(path.productSubCategoryId, { message: 'Sub-category is required' });
    min(path.productSubCategoryId, 1, {
      message: 'Sub-category ID must be greater than 0',
    });

    required(path.mainProductSubCategoryAttributeId, {
      message: 'Main attribute is required',
    });
    min(path.mainProductSubCategoryAttributeId, 1, {
      message: 'Attribute ID must be greater than 0',
    });
  });

  onImageSelected(event: Event): void {
    const input = event.target as HTMLInputElement;
    if (!input.files?.length) return;

    const startOrder = this.productImages().length + 1;

    Array.from(input.files).forEach((file, index) => {
      const reader = new FileReader();

      reader.onload = () => {
        const imageModel = new AddProductImageModel();
        imageModel.imageUrl = file.name;
        imageModel.displayOrderId = startOrder + index;
        imageModel.isMainImage = this.productImages().length === 0;

        this.productImages.update((imgs) => [...imgs, imageModel]);
      };

      reader.readAsDataURL(file);
    });

    input.value = '';
  }

  setMainImage(index: number): void {
    this.productImages.update((imgs) =>
      imgs.map((img, i) => ({
        ...img,
        isMainImage: i === index,
      }))
    );
  }

  removeImage(index: number): void {
    this.productImages.update((imgs) =>
      imgs
        .filter((_, i) => i !== index)
        .map((img, i) => ({
          ...img,
          displayOrderId: i + 1,
          isMainImage: i === 0,
        }))
    );
  }

  addProduct(): void {
    this.errorMessage.set(null);
    this.successMessage.set(null);

    this.vendorProductService.addProduct(this.product()).subscribe({
      next: (response: any) => {
        const productId =
          response.productId ??
          response.data?.productId ??
          response.id;

        if (!productId) {
          this.errorMessage.set('Product added, but product ID was not returned.');
          return;
        }

        this.addProductImages(productId);
      },
      error: (error) => {
        this.errorMessage.set(error.error?.message ?? 'Failed to add product');
      },
    });
  }

  private addProductImages(productId: number): void {
    const images = this.productImages().map((img) => {
      img.productId = productId;
      return img;
    });

    if (images.length === 0) {
      this.successMessage.set('Product added successfully');
      this.resetForm();
      return;
    }

    let uploadedCount = 0;

    images.forEach((image) => {
      this.vendorProductService.addProductImage(image).subscribe({
        next: () => {
          uploadedCount++;

          if (uploadedCount === images.length) {
            this.successMessage.set('Product and images added successfully');
            this.resetForm();
          }
        },
        error: (error) => {
          this.errorMessage.set(
            error.error?.message ?? 'Product added, but image upload failed'
          );
        },
      });
    });
  }

  resetForm(): void {
    this.product.set(new AddProductModel());
    this.productImages.set([]);
  }
}