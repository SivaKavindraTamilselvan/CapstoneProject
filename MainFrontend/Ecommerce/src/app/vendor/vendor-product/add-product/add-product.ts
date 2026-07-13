import { Component, signal } from '@angular/core';
import { VendorProductService } from '../../../services/vendor-product.Service';
import { AddProductModel } from '../../../models/vendor/vendor-product/add-model/add-product.model';
import { AddProductImageModel } from '../../../models/vendor/vendor-product/add-model/add-product-image';
import { form, FormField, maxLength, min, required } from '@angular/forms/signals';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { AdminProductCategoryModel } from '../../../models/admin/admin-product-category/response/admin-category';
import { AdminProductSubCategoryModel } from '../../../models/admin/admin-product-category/response/admin-subcategory.model';
import { AdminAttributeModel } from '../../../models/admin/admin-product-category/response/admin-attribute.model';
import { AdminMappedAttributeModel } from '../../../models/admin/admin-product-category/response/admin-mapped.model';

@Component({
  selector: 'app-add-product',
  imports: [FormField, ReactiveFormsModule, FormsModule],
  templateUrl: './add-product.html',
  styleUrl: './add-product.css',
})
export class AddProduct {
  errorMessage = signal<string | null>(null);
  successMessage = signal<string | null>(null);
  product = signal(new AddProductModel());
  productImages = signal<AddProductImageModel[]>([]);
  imagePreviews = signal<string[]>([]);
  productCategoryId = signal<number | null>(null);
  attributeId = signal<number | null>(null);
  productSubCategoryId = signal<number | null>(null);
  categories = signal<AdminProductCategoryModel[]>([]);
  attributes = signal<AdminMappedAttributeModel[]>([]);
  subCategories = signal<AdminProductSubCategoryModel[]>([]);

  loading = signal(false);

  // Fixed image angle types instead of a running sequence number
  imageAngles: { label: string; value: number }[] = [
    { label: 'Front', value: 1 },
    { label: 'Back', value: 2 },
    { label: 'Left', value: 3 },
    { label: 'Right', value: 4 },
  ];

   scrollToTop(): void {
    window.scrollTo({
      top: 0,
      left: 0,
      behavior: 'smooth'
    });
  }

  constructor(private vendorProductService: VendorProductService) { }

  ngOnInit() {
    this.scrollToTop();
    this.loadCategories();
    this.loadAttributes();
  }

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

    Array.from(input.files).forEach((file) => {
      const reader = new FileReader();

      reader.onload = () => {
        const imageModel = new AddProductImageModel();
        imageModel.imageUrl = file.name;
        imageModel.displayOrderId = this.nextAvailableAngle();
        imageModel.isMainImage = this.productImages().length === 0;

        this.productImages.update((imgs) => [...imgs, imageModel]);
        this.imagePreviews.update((previews) => [...previews, reader.result as string]);
      };

      reader.readAsDataURL(file);
    });

    input.value = '';
  }

  private nextAvailableAngle(): number {
    const usedAngles = this.productImages().map((img) => img.displayOrderId);
    const free = this.imageAngles.find((a) => !usedAngles.includes(a.value));
    return free ? free.value : 1;
  }

  onAngleChange(index: number, event: Event): void {
    const value = Number((event.target as HTMLSelectElement).value);
    this.productImages.update((imgs) =>
      imgs.map((img, i) => (i === index ? { ...img, displayOrderId: value } : img))
    );
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
    this.productImages.update((imgs) => imgs.filter((_, i) => i !== index));
    this.imagePreviews.update((previews) => previews.filter((_, i) => i !== index));

    this.productImages.update((imgs) => {
      if (imgs.length > 0 && !imgs.some((i) => i.isMainImage)) {
        return imgs.map((img, i) => ({ ...img, isMainImage: i === 0 }));
      }
      return imgs;
    });
  }

  addProduct(): void {
    this.errorMessage.set(null);
    this.successMessage.set(null);

    if (this.addForm().invalid()) {
      this.errorMessage.set("Enter proper details");
      this.scrollToTop();
      return;
    }

    if (this.productImages().length === 0) {
      this.errorMessage.set(
        'Please upload at least one product image.'
      );
      return;
    }

    this.loading.set(true);

    this.vendorProductService.addProduct(this.product()).subscribe({
      next: (response: any) => {
        const productId =
          response.productId ??
          response.data?.productId ??
          response.id;

        if (!productId) {
          this.errorMessage.set('Product added, but product ID was not returned.');
          this.loading.set(false);
          return;
        }

        this.addProductImages(productId);
        this.scrollToTop();
      },
      error: (error) => {
        if (error.status === 400 && error.error?.errors) {
          const messages = Object.values(error.error.errors)
            .flat()
            .join(', ');

          this.errorMessage.set(messages);
        }
        else if (error.error?.message) {
          this.errorMessage.set(error.error.message);
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
        else {
          this.errorMessage.set('Failed to add product.');
        }
        this.loading.set(false);
      }
    });
  }

  private addProductImages(productId: number): void {
    const images = this.productImages().map((img) => {
      img.productId = productId;
      return img;
    });

    if (images.length === 0) {
      this.successMessage.set('Product added successfully');
      this.loading.set(false);
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
            this.loading.set(false);
            this.resetForm();
          }
        },
        error: (error) => {

          if (error.error?.message) {
            this.errorMessage.set(error.error.message);
          }
          else if (error.status === 0) {
            this.errorMessage.set(
              'Images could not be uploaded. Please check your connection.'
            );
          }
          else {
            this.errorMessage.set(
              'Product was added, but one or more images failed to upload.'
            );
          }
          this.loading.set(false);
        }
      });
    });
  }

  resetForm(): void {
    this.errorMessage.set(null);
    this.successMessage.set(null);

    this.product.set(new AddProductModel());
    this.productImages.set([]);
    this.imagePreviews.set([]);
    this.productCategoryId.set(null);
    this.productSubCategoryId.set(null);
    this.attributeId.set(null);
    this.subCategories.set([]);

    this.addForm().reset();
    this.scrollToTop();
  }

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
    this.product.update(product => ({
      ...product,
      productSubCategoryId: value
    }));
    this.productSubCategoryId.set(value);
    this.loadAttributes();
  }

  onAttributeChange(event: Event): void {
    const value = Number((event.target as HTMLSelectElement).value);
    this.product.update(product => ({
      ...product,
      mainProductSubCategoryAttributeId: value
    }));
    this.attributeId.set(value);
  }
}