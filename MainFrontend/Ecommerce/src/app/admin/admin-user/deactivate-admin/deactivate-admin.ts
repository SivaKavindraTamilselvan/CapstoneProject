import { Component, signal, computed } from '@angular/core';
import { AdminUserService } from '../../../services/admin-user.Service';
import { Router } from '@angular/router';
import { PagedResponse } from '../../../models/paged-response.model';
import { AdminUserModel } from '../../../models/admin-user.model';
import { AdminUserFilter } from '../../../models/admin-user.filter';

@Component({
  selector: 'app-deactivate-admin',
  imports: [],
  templateUrl: './deactivate-admin.html',
  styleUrl: './deactivate-admin.css',
})
export class DeactivateAdmin {
  adminUsers = signal<PagedResponse<AdminUserModel> | null>(null);
  showActivatePopup = signal(false);
  selectedAdminId = signal<number | null>(null);
  pageNumber = signal<number>(1);
  pageSize = signal<number>(10);
  totalPages = computed(() => this.adminUsers()?.totalPages ?? 1);
  adminRoleId = signal<number | null>(null);
  filterPanelOpen = signal<boolean>(false);
  constructor(private route: Router, private adimUserService: AdminUserService) {

  }
  ngOnInit(): void {
    this.loadDeactiveAdminUser();
  }
  loadDeactiveAdminUser() {
    this.adimUserService.getDeactiveAdminUser(this.buildFilter()).subscribe({
      next: (response: any) => {
        this.adminUsers.set(response);
      },
      error: (error) => {
        console.log(error);
      }
    })
  }
  activateAdmin() {
    const id = this.selectedAdminId();
    if (id == null) {
      return;
    }
    this.adimUserService.activateAdminUser(id).subscribe({
      next: (response: any) => {
        this.closePopup();
        this.loadDeactiveAdminUser();
      },
      error: (error) => {
        console.log(error);
      }
    })
  }
  confirmActivate(id: number) {
    this.selectedAdminId.set(id);
    this.showActivatePopup.set(true);
  }
  closePopup() {
    this.showActivatePopup.set(false);
    this.selectedAdminId.set(null);
  }
  onPageSizeChange(event: Event): void {
    const value = Number((event.target as HTMLInputElement).value);
    this.pageNumber.set(1);
    this.pageSize.set(value);
    this.loadDeactiveAdminUser();
  }
  goToPage(page: number): void {
    if (page < 1 || page > this.totalPages()) {
      return;
    }
    this.pageNumber.set(page);
    this.loadDeactiveAdminUser();
  }
  nextPage(): void {
    this.goToPage(this.pageNumber() + 1);
  }
  previousPage(): void {
    this.goToPage(this.pageNumber() - 1);
  }
  adminRoleOption = [
    { id: 1, label: 'Overall Admin' },
    { id: 2, label: 'Vendor Admin' },
    { id: 3, label: 'Product Admin' },
    { id: 4, label: 'Order Admin' },
    { id: 5, label: 'Coupons Logistic Admin' },
    { id: 6, label: 'Return Admin' },
    { id: 7, label: 'Refund Admin' },
    { id: 8, label: 'Exchange Admin' },
    { id: 9, label: 'Payment Admin' }
  ]
  onAdminRoleChange(event: Event): void {
    const v = (event.target as HTMLSelectElement).value;
    this.adminRoleId.set(v ? Number(v) : null);
  }
  toggleFilterPanel(): void {
    this.filterPanelOpen.update((open) => !open);
  }
  closeFilterPanel(): void {
    this.filterPanelOpen.set(false);
  }
  applyFilter(): void {
    this.pageNumber.set(1);
    this.loadDeactiveAdminUser();
    this.closeFilterPanel();
  }
  resetFilter(): void {
    this.adminRoleId.set(null);
    this.pageNumber.set(1);
    this.loadDeactiveAdminUser();
  }
  private buildFilter(): AdminUserFilter {
    return {
      pageNumber: this.pageNumber(),
      pageSize: this.pageSize(),
      adminRoleId: this.adminRoleId(),
      isActive : false
    };
  }
}
