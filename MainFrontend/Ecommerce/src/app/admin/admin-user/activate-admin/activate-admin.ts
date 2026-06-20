import { Component, signal } from '@angular/core';
import { AdminUserService } from '../../../services/admin-user.Service'; 
import { Router } from '@angular/router';
import { PagedResponse } from '../../../models/paged-response.model'; 
import { AdminUserModel } from '../../../models/admin-user.model'; 

@Component({
  selector: 'app-activate-admin',
  imports: [],
  templateUrl: './activate-admin.html',
  styleUrl: './activate-admin.css',
})
export class ActivateAdmin {
  adminUsers = signal<PagedResponse<AdminUserModel> | null>(null);
  showDeactivatePopup = signal(false);
  selectedAdminId = signal<number | null>(null);
  constructor(private route: Router, private adminUserService: AdminUserService) {

  }
  ngOnInit(): void {
    this.loadActiveAdminUser();
  }
  loadActiveAdminUser() {
    this.adminUserService.getActiveAdminUser().subscribe({
      next: (response: any) => {
        this.adminUsers.set(response);
      },
      error: (error) => {
        console.log(error);
      }
    })

  }
  deactivateAdmin() {
    const id = this.selectedAdminId();
    if (id == null) {
      return;
    }
    this.adminUserService.deactivateAdminUser(id).subscribe({
      next: (response: any) => {
        this.loadActiveAdminUser();
        this.closePopup();
      },
      error: (error) => {
        console.log(error);
      }
    })
  }
  confirmDeactivate(id: number) {
    this.selectedAdminId.set(id);
    this.showDeactivatePopup.set(true);
  }
  closePopup() {
    this.showDeactivatePopup.set(false);
    this.selectedAdminId.set(null);
  }
}
