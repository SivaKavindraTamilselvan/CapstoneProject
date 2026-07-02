import { Component, signal } from '@angular/core';
import { VendorUserModel } from '../../../models/vendor/vendor-user/response-vendor-user.model';
import { VendorUserService } from '../../../services/vendor-user.Service';
import { ActivatedRoute } from '@angular/router';
import { DatePipe } from '@angular/common';

@Component({
  selector: 'app-vendor-user-details',
  imports: [DatePipe],
  templateUrl: './vendor-user-details.html',
  styleUrl: './vendor-user-details.css',
})
export class VendorUserDetails {
  adminUser = signal(new VendorUserModel());
  errorMessage = signal<string | null>(null);
  showActivatePopup = signal(false);
  constructor(private vendorUserService: VendorUserService, private route: ActivatedRoute) {

  }
  loadAdminUser(id: number) {
    this.vendorUserService.getAdminUserDetail(id).subscribe({
      next: (response: any) => {
        this.adminUser.set(response);
        console.log(response);
      },
      error: (error) => {
        if (error.status == 404) {
          this.errorMessage.set("Admin User Is Not Found");
        }
      }
    })
  }
  ngOnInit() {
    const adminUserId = Number(this.route.snapshot.paramMap.get('id'));

    if (adminUserId) {
      this.loadAdminUser(adminUserId);
    }
  }
}

