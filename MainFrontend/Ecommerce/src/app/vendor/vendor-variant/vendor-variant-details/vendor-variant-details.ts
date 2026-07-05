import { Component, signal } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { VendorProductService } from '../../../services/vendor-product.Service';
import { VendorProductVariantModel } from '../../../models/vendor/vendor-product/response/vendor-variant.model';
import { DatePipe, DecimalPipe, NgClass } from '@angular/common';

@Component({
  selector: 'app-vendor-variant-details',
  imports: [DatePipe,DecimalPipe,NgClass],
  templateUrl: './vendor-variant-details.html',
  styleUrl: './vendor-variant-details.css',
})
export class VendorVariantDetails {
    variantModel = signal<VendorProductVariantModel | null>(null);
    currentImageIndex = signal(0);

  constructor(private route: Router, private vendorProductService: VendorProductService, private router: ActivatedRoute) {

  }

  ngOnInit(): void {
    const productId = Number(this.router.snapshot.paramMap.get('id'));

    if (productId) {
      this.loadProductDetails(productId);
    }
  }

  loadProductDetails(productId: number) {
    this.vendorProductService.getProductVariantDetails(productId).subscribe({
      next: (response: any) => {
        console.log(response);
        this.variantModel.set(response);
      },
      error: (error) => {
        console.error(error);
      }
    })
  }
  prevImage() {
    const images = this.variantModel()?.productImages;
    if (!images?.length) return;

    this.currentImageIndex.update(i => (i === 0 ? images.length - 1 : i - 1));
  }

  nextImage() {
    const images = this.variantModel()?.productImages;
    if (!images?.length) return;

    this.currentImageIndex.update(i => (i === images.length - 1 ? 0 : i + 1));
  }

}
