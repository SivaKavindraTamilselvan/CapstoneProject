import { Component, signal } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { VendorProductService } from '../../../services/vendor-product.Service';
import { AddProductVariantModel } from '../../../models/vendor/vendor-product/add-model/add-product-variant.model';
import { AddProductVariantImageModel } from '../../../models/vendor/vendor-product/add-model/add-variant-image.model';
import { AddProductVariantAttributeModel } from '../../../models/vendor/vendor-product/add-model/add-variant-attribute.model';
import { form, FormField, min, required } from '@angular/forms/signals';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { AdminAttributeModel } from '../../../models/admin/admin-product-category/response/admin-attribute.model';
import { AdminMappedAttributeModel } from '../../../models/admin/admin-product-category/response/admin-mapped.model';

@Component({
  selector: 'app-add-variant',
  imports: [FormField, ReactiveFormsModule, FormsModule],
  templateUrl: './add-variant.html',
  styleUrl: './add-variant.css',
})
export class AddVariant {
  errorMessage = signal<string | null>(null);
  successMessage = signal<string | null>(null);
  loading = signal(false);

  subcategoryid = signal<number | null>(null);
  productid = signal<number | null>(null);
  attributes = signal<AdminMappedAttributeModel[]>([]);
  mainAttributeId = signal<number | null>(null);

  productVariant = signal(new AddProductVariantModel());
  productVariantImages = signal<AddProductVariantImageModel[]>([]);
  variantImagePreviews = signal<string[]>([]);

  // Fixed image angle types instead of a running sequence number
  imageAngles: { label: string; value: number }[] = [
    { label: 'Front', value: 1 },
    { label: 'Back', value: 2 },
    { label: 'Left', value: 3 },
    { label: 'Right', value: 4 },
  ];

  constructor(
    private route: Router,
    private router: ActivatedRoute,
    private vendorProductService: VendorProductService
  ) { }

  ngOnInit(): void {
    window.scroll(0, 0);
    const productId = Number(this.router.snapshot.paramMap.get('id'));
    const subCategoryId = Number(this.router.snapshot.queryParamMap.get('subCategoryId'));
    const mainProductAttributeId = Number(this.router.snapshot.queryParamMap.get('mainProductAttributeId'));
    this.subcategoryid.set(subCategoryId);
    this.productid.set(productId);
    this.loadAttributes();
    this.mainAttributeId.set(mainProductAttributeId);

    this.productVariant.update((v) => ({
      ...v,
      productId,
      subCategoryId,
      mainProductAttributeId,
    }));
  }

  addForm = form(this.productVariant, (path) => {

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

  scrollToTop(): void {
    window.scrollTo({
      top: 0,
      left: 0,
      behavior: 'smooth'
    });
  }

  loadAttributes(): void {
    this.errorMessage.set(null);
    const id = (this.subcategoryid());
    if (id == null) {
      this.scrollToTop();
      return;
    }
    this.vendorProductService.getmappedAttribute(id).subscribe({
      next: (res: any) => {
        this.attributes.set(res.items ?? res);
        console.log(this.attributes());
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

  onImageSelected(event: Event): void {
  const input = event.target as HTMLInputElement;
  if (!input.files?.length) return;

  Array.from(input.files).forEach((file) => {
    const reader = new FileReader();
    reader.onload = () => {
      const image = new AddProductVariantImageModel();
      image.file = file; // store the actual File, not file.name
      image.displayOrderId = this.nextAvailableAngle();

      this.productVariantImages.update((imgs) => [...imgs, image]);
      this.variantImagePreviews.update((previews) => [...previews, reader.result as string]);
    };
    reader.readAsDataURL(file);
  });

  input.value = '';
}

private nextAvailableAngle(): number {
  const usedAngles = this.productVariantImages().map((img) => img.displayOrderId);
  const free = this.imageAngles.find((a) => !usedAngles.includes(a.value));
  return free ? free.value : 1;
}

onAngleChange(index: number, event: Event): void {
  const value = Number((event.target as HTMLSelectElement).value);
  this.productVariantImages.update((imgs) =>
    imgs.map((img, i) => (i === index ? { ...img, displayOrderId: value } : img))
  );
}

removeImage(index: number): void {
  this.productVariantImages.update((imgs) => imgs.filter((_, i) => i !== index));
  this.variantImagePreviews.update((previews) => previews.filter((_, i) => i !== index));
}

private addVariantImages(variantId: number): void {
  const images = this.productVariantImages().map((img) => {
    img.productVariantId = variantId;
    return img;
  });

  if (images.length === 0) {
    this.successMessage.set('Variant added successfully');
    this.loading.set(false);
    this.resetForm();
    return;
  }

  let uploadedCount = 0;

  images.forEach((image) => {
    const formData = new FormData();
    formData.append('ProductVariantId', image.productVariantId.toString());
    formData.append('File', image.file as File);
    formData.append('DisplayOrderId', image.displayOrderId.toString());

    this.vendorProductService.uploadProductVariantImage(formData).subscribe({
      next: () => {
        uploadedCount++;

        if (uploadedCount === images.length) {
          this.successMessage.set('Variant and images added successfully');
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
            'Variant was added, but one or more images failed to upload.'
          );
        }
        this.loading.set(false);
      }
    });
  });
}

  addVariant(): void {
    this.errorMessage.set(null);
    this.successMessage.set(null);
    if (this.addForm().invalid()) {
      this.errorMessage.set('Enter proper details');
      this.scrollToTop();
      return;
    }

    this.loading.set(true);

    this.productVariant.update(v => ({
      ...v,
      productId: this.productid() ?? 0
    }));

    this.vendorProductService.addProductVariant(this.productVariant()).subscribe({
      next: (response: any) => {
        const variantId =
          response.productVariantId ??
          response.data?.productVariantId ??
          response.id;

        if (!variantId) {
          this.errorMessage.set('Variant added, but variant ID was not returned.');
          this.loading.set(false);
          return;
        }

        this.addVariantImages(variantId);
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

  

  resetForm(): void {
    this.errorMessage.set(null);
    //this.successMessage.set(null);

    const productId = this.productid() ?? 0;
    const subCategoryId = this.subcategoryid() ?? 0;
    const mainProductAttributeId = this.mainAttributeId();

    this.productVariant.set(new AddProductVariantModel());
    this.addForm().reset();

    this.productVariant.update((v) => ({
      ...v,
      productId : productId,
      subCategoryId,
      mainProductAttributeId,
    }));

    this.productVariantImages.set([]);
    this.variantImagePreviews.set([]);
    this.scrollToTop();
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