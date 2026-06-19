import { Component, signal } from '@angular/core';
import { Router } from '@angular/router';
import { AdminVendorService } from '../../../services/admin-vendor.Service';
import { PagedResponse } from '../../../models/paged-response.model';
import { AdminVendorModel } from '../../../models/admin-vendor.model';

@Component({
  selector: 'app-vendor-list',
  imports: [],
  templateUrl: './vendor-list.html',
  styleUrl: './vendor-list.css',
})
export class VendorList {
  vendors = signal<PagedResponse<AdminVendorModel> | null>(null);
  constructor(private route : Router,private adminVendorService : AdminVendorService){

  }
  ngOnInit():void{
    this.loadVendor();
  }
  loadVendor(){
    this.adminVendorService.getVendor().subscribe({
      next : (response : any)=>{
        this.vendors.set(response);
      },
      error : (error) =>{
        console.log(error);
      }
    })
  }
}
