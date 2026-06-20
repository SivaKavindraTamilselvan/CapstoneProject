import { Component } from '@angular/core';
import { AdminVendorService } from '../../../services/admin-vendor.Service';

@Component({
  selector: 'app-delete-vendor',
  imports: [],
  templateUrl: './delete-vendor.html',
  styleUrl: './delete-vendor.css',
})
export class DeleteVendor {
  constructor(private adminVendorService : AdminVendorService)
  {

  }

}
