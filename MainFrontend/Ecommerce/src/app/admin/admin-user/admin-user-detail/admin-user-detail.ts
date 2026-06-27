import { Component, signal } from '@angular/core';
import { AdminUserService } from '../../../services/admin-user.Service';
import { AdminUserModel } from '../../../models/admin/admin-user/admin-user.model';
import { ActivatedRoute } from '@angular/router';
import { DatePipe } from '@angular/common';

@Component({
  selector: 'app-admin-user-detail',
  imports: [DatePipe],
  templateUrl: './admin-user-detail.html',
  styleUrl: './admin-user-detail.css',
})
export class AdminUserDetail {
  adminUser = signal(new AdminUserModel());
  errorMessage = signal<string | null>(null);
  constructor(private adminUserService: AdminUserService, private route: ActivatedRoute) {

  }
  loadAdminUser(id: number) {
    this.adminUserService.getAdminUserDetail(id).subscribe({
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
