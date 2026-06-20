import { Component, signal, computed } from '@angular/core';
import { Route, Router } from '@angular/router';
import { PagedResponse } from '../../../models/paged-response.model';
import { AdminUserService } from '../../../services/admin-user.Service';
import { AdminUserModel } from '../../../models/admin-user.model';
import { AdminUserFilter } from '../../../models/admin-user.filter';

@Component({
  selector: 'app-admin-list',
  imports: [],
  templateUrl: './admin-list.html',
  styleUrl: './admin-list.css',
})
export class AdminList {
  adminUsers = signal<PagedResponse<AdminUserModel> | null>(null);
  pageNumber = signal<number>(1);
  pageSize = signal<number>(10);
  totalPages = computed(() => this.adminUsers()?.totalPages ?? 1);
  filterPanelOpen = signal<boolean>(false);
  adminRoleId = signal<number | null>(null);
  isActive = signal<boolean | null>(null);
  constructor(private route: Router, private adminUserService: AdminUserService) {

  }
  ngOnInit(): void {
    this.loadAdminUser();
  }
  loadAdminUser() {
    this.adminUserService.getAdminUser(this.buildFilter()).subscribe({
      next: (response: any) => {
        this.adminUsers.set(response);
      },
      error: (error) => {
        console.log(error);
      }
    })
  }
  goToPage(page: number): void {
    if (page < 1 || page > this.totalPages()) {
      return;
    }
    this.pageNumber.set(page);
    this.loadAdminUser();
  }
  nextPage(): void {
    this.goToPage(this.pageNumber() + 1);
  }
  previousPage(): void {
    this.goToPage(this.pageNumber() - 1);
  }
  toggleFilterPanel(): void {
    this.filterPanelOpen.update((open) => !open);
  }
  closeFilterPanel(): void {
    this.filterPanelOpen.set(false);
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
  onStatusChange(event: Event): void {
    const value = (event.target as HTMLSelectElement).value;
    if (value === '') {
      this.isActive.set(null);
    }
    else {
      this.isActive.set(value === 'true');
    }
  }
  applyFilter(): void {
    this.pageNumber.set(1);
    this.loadAdminUser();
    this.closeFilterPanel();
  }
  resetFilter(): void {
    this.adminRoleId.set(null);
    this.isActive.set(null);
    this.pageNumber.set(1);
    this.loadAdminUser();
  }
  private buildFilter(): AdminUserFilter {
    return {
      pageNumber: this.pageNumber(),
      pageSize: this.pageSize(),
      adminRoleId: this.adminRoleId(),
      isActive: this.isActive(),
    };
  }
  onPageSizeChange(event : Event) : void{
    const value = Number((event.target as HTMLInputElement).value);
    this.pageSize.set(value);
    this.pageNumber.set(1);
    this.loadAdminUser();
  }
}
