import { Component, signal } from '@angular/core';
import { ProductModel } from '../../../models/product/product.model';
import { ActivatedRoute, Router } from '@angular/router';
import { VendorProductService } from '../../../services/vendor-product.Service';
import { CommonModule } from '@angular/common';
import { VendorProductModel } from '../../../models/vendor/vendor-product/response/vendor-product.model';

@Component({
  selector: 'app-vendor-product-details',
  imports: [CommonModule],
  templateUrl: './vendor-product-details.html',
  styleUrl: './vendor-product-details.css',
})
export class VendorProductDetails {
  productModel = signal<VendorProductModel | null>(null);
  currentImageIndex = signal(0);


  constructor(private vendorProductService: VendorProductService, private route: ActivatedRoute,private router : Router) {

  }
  ngOnInit(): void {
    const productId = Number(this.route.snapshot.paramMap.get('id'));

    if (productId) {
      this.loadProductDetails(productId);
    }
  }
  loadProductDetails(productId: number) {
    this.vendorProductService.getProductDetails(productId).subscribe({
      next: (response: any) => {
        console.log(response);
        this.productModel.set(response);
      },
      error: (error) => {
        console.error(error);
      }
    })
  }
  prevImage() {
    const total = this.productModel()?.productImages?.length ?? 0;
    this.currentImageIndex.update(i => (i - 1 + total) % total);
  }

  nextImage() {
    const total = this.productModel()?.productImages?.length ?? 0;
    this.currentImageIndex.update(i => (i + 1) % total);
  }
  addVariant() {
    const product = this.productModel();
    if (!product) return;

    this.router.navigate(['/vendor/products', product.productId, 'variants','add'] , {
      queryParams: {
        subCategoryId: product.productSubCategoryId,
        mainProductAttributeId: product.mainProductSubCategoryAttributeId,
      },
    });
  }
}

