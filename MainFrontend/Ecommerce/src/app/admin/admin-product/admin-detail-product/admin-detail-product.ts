import { Component, signal } from '@angular/core';
import { AdminProductService } from '../../../services/admin-product.Service';
import { ProductModel } from '../../../models/product/product.model';
import { ActivatedRoute } from '@angular/router';
import { CommonModule, DatePipe, DecimalPipe } from '@angular/common';

@Component({
  selector: 'app-admin-detail-product',
  imports: [DatePipe, DecimalPipe,CommonModule],
  templateUrl: './admin-detail-product.html',
  styleUrl: './admin-detail-product.css',
})
export class AdminDetailProduct {
  productModel = signal<ProductModel | null>(null);
  currentImageIndex = signal(0);


  constructor(private adminProductService: AdminProductService, private route: ActivatedRoute) {

  }
  ngOnInit(): void {
    const productId = Number(this.route.snapshot.paramMap.get('id'));

    if (productId) {
      this.loadProductDetails(productId);
    }
  }
  loadProductDetails(productId: number) {
    this.adminProductService.getProductDetails(productId).subscribe({
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
}
