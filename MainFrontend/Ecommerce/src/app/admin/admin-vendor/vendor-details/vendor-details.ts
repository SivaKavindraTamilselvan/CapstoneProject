import { Component, signal } from '@angular/core';
import { AdminVendorService } from '../../../services/admin-vendor.Service';
import { ActivatedRoute } from '@angular/router';
import { AdminVendorModel } from '../../../models/admin/vendor/admin-vendor.model';

@Component({
  selector: 'app-vendor-details',
  imports: [],
  templateUrl: './vendor-details.html',
  styleUrl: './vendor-details.css',
})
export class VendorDetails {
  vendor = signal(new AdminVendorModel());
  errorMessage = signal<string | null>(null);
  constructor(private adminVendorService : AdminVendorService,private route : ActivatedRoute){

  }
  ngOnInit(){
    const vendorId = Number(this.route.snapshot.paramMap.get('id'));
    if (vendorId) {
      this.loadVendor(vendorId);
    }
  }
  loadVendor(vendorId : number){
    this.adminVendorService.getVendorDetails(vendorId).subscribe({
      next : (response:any)=>{
        this.vendor.set(response);
        console.log(response);
      },
      error : (error)=>{
        if (error.status == 404) {
          this.errorMessage.set("Admin User Is Not Found");
        }
      }
    })
  }
}
