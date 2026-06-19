import { Component, signal } from '@angular/core';
import { AdminVendorService } from '../../../services/admin-vendor.Service';
import { PagedResponse } from '../../../models/paged-response.model';
import { AdminVendorModel } from '../../../models/admin-vendor.model';

@Component({
  selector: 'app-active-vendor',
  imports: [],
  templateUrl: './active-vendor.html',
  styleUrl: './active-vendor.css',
})
export class ActiveVendor {
  vendors = signal<PagedResponse<AdminVendorModel> | null>(null);
  constructor(private adminVednorServce: AdminVendorService) {

  }
  ngOnInit() {
    this.loadActiveVendor();
  }
  loadActiveVendor() {
    this.adminVednorServce.getActiveVendor().subscribe({
      next: (response: any) => {
        this.vendors.set(response);
      },
      error: (error) => {
        console.log(error);
        if (error.status === 404) {
          this.vendors.set({
            items: [],
            pageNumber: 1,
            pageSize: 10,
            totalCount: 0,
            totalPages: 0
          });
        }
        else {
          console.log(error);
        }
      }
    })
  }
}
