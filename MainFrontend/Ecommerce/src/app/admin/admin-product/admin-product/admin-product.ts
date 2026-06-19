import { Component, signal } from '@angular/core';
import { PagedResponse } from '../../../models/paged-response.model';
import { ProductModel } from '../../../models/product.model';
import { Router } from '@angular/router';
import { AdminProductService } from '../../../services/admin-product.Service';

@Component({
  selector: 'app-admin-product',
  imports: [],
  templateUrl: './admin-product.html',
  styleUrl: './admin-product.css',
})
export class AdminProduct {
  products = signal<PagedResponse<ProductModel> | null>(null);
  constructor(private route:Router, private adminProductService : AdminProductService)
  {

  }
  ngOnInit():void{
    this.loadProduct();
  }
  loadProduct()
  {
    this.adminProductService.getProducts().subscribe({
      next:(response:any)=>{
        this.products.set(response);
      },
      error :(error)=>{
        console.log(error);
      }
    })
  }
}
