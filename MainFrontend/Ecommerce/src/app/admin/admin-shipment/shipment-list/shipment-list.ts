import { Component, computed, effect, signal } from '@angular/core';
import { Router } from '@angular/router';
import { AdminShipmentService } from '../../../services/admin-shipment.Service';
import { ShipmentFilter } from '../../../models/admin/admin-shipment/shipment.filter';
import { ShipmentModel } from '../../../models/admin/admin-shipment/admin-shipment.model';
import { DatePipe } from '@angular/common';
import { PagedResponse } from '../../../models/paged-response.model';
import { TableAction } from '../../../shared-components/data-table-component/table-actions.model';
import { Column } from '../../../shared-components/data-table-component/column.model';
import { BasePage } from '../../../shared-class/shares-page-class';
import { form, FormField, maxLength, min, pattern, required } from '@angular/forms/signals';
import { MobileCardComponent } from '../../../shared-components/mobile-card-component/mobile-card-component';
import { FilterComponent } from '../../../shared-components/filter-component/filter-component';
import { DataTableComponent } from '../../../shared-components/data-table-component/data-table-component';
import { PaginationComponent } from '../../../shared-components/pagination-component/pagination-component';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { UpdateAdminShipment } from '../update-admin-shipment/update-admin-shipment';

@Component({
  selector: 'app-shipment-list',
  imports: [MobileCardComponent, FilterComponent, DataTableComponent, PaginationComponent, FormField, ReactiveFormsModule, FormsModule, UpdateAdminShipment],
  providers: [DatePipe],
  templateUrl: './shipment-list.html',
  styleUrl: './shipment-list.css',
})
export class ShipmentList extends BasePage {

  shipments = signal<PagedResponse<ShipmentModel> | null>(null);
  shipmentStatusId = signal<number | null>(null);
  fromDate = signal<string>('');
  toDate = signal<string>('');
  shipmentFilter = signal(new ShipmentFilter());
  clearFilterValues(): void {
    this.shipmentStatusId.set(null);
    this.shipmentFilter.set(new ShipmentFilter());
    this.shipmentFilter.update(filter => ({
      ...filter,
      shipmentStatusId: null
    }));
  }

  totalPages = computed(() => this.shipments()?.totalPages ?? 1);
  constructor(private datePipe: DatePipe, private router: Router, private shipmentService: AdminShipmentService) {
    super();
    effect(() => {
      if (this.filterForm().invalid()) {
        this.filterErrorMessage.set('Please fix the validation errors.');
      } else {
        this.filterErrorMessage.set(null);
      }
    });
  }

  ngOnInit(): void {
    this.loadShipments();
  }

  loadShipments() {
    this.buildFilter();
    this.shipmentService.getShipment(this.shipmentFilter()).subscribe({
      next: (response: PagedResponse<ShipmentModel>) => {
        this.shipments.set(response);
        console.log(response);
      },
      error: (error) => {
        console.error(error);

        if (error.status === 404) {
          this.shipments.set({
            items: [],
            totalCount: 0,
            pageNumber: this.pageNumber(),
            pageSize: this.pageSize(),
            totalPages: 1
          });
        }
      }
    });
  }

  protected loadData(): void {
    this.loadShipments();
  }

  filterForm = form(this.shipmentFilter, (path) => {
    pattern(path.trackingNumber, /^[A-Za-z0-9-]*$/, { message: 'Tracking number can contain only letters, numbers, and hyphens.' });
    maxLength(path.trackingNumber, 50, { message: 'Tracking number cannot exceed 50 characters.' });
    maxLength(path.courierName, 50, { message: 'Courier name cannot exceed 50 characters.' });
    min(path.orderId, 1, { message: 'Order ID must be greater than 0.' });
    min(path.shipmentStatusId, 1, { message: 'Shipment status ID must be greater than 0.' });
  });

  private buildFilter() {
    this.shipmentFilter.update(filter => ({
      ...filter,
      pageNumber: this.pageNumber(),
      pageSize: this.pageSize(),
      shipmentStatusId: this.shipmentStatusId(),
    }));
  }

  onStatusChange(event: Event): void {
    const value = (event.target as HTMLSelectElement).value;

    if (value === '') {
      this.shipmentStatusId.set(null);
    } else {
      this.shipmentStatusId.set(Number(value));
    }
    this.shipmentFilter.update(filter => ({
      ...filter,
      shipmentStatusId: value === '' ? null : Number(value)
    }));
  }

  onFromDateInput(event: Event): void {
    const value = (event.target as HTMLInputElement).value;
    this.fromDate.set(value);
    this.shipmentFilter.update(filter => ({
      ...filter,
      fromDate: value
    }));
  }

  onToDateInput(event: Event): void {
    const value = (event.target as HTMLInputElement).value;
    this.toDate.set(value);
    this.shipmentFilter.update(filter => ({
      ...filter,
      toDate: value
    }));
  }

  showUpdateShipmentPopup = signal(false);
  selectedShipmentIdForUpdate = signal<number | null>(null);

  openUpdatePopup(shipmentId: number) {
    this.selectedShipmentIdForUpdate.set(shipmentId);
    this.showUpdateShipmentPopup.set(true);
  }

  closeUpdateShipmentPopup() {
    this.showUpdateShipmentPopup.set(false);
    this.selectedShipmentIdForUpdate.set(null);
  }

  onShipmentUpdated() {
    this.loadShipments();
  }

  actions: TableAction<ShipmentModel>[] = [
    { label: 'View', color: 'green', action: 'view' },
    { label: 'Update', color: 'blue', action: 'update' }
  ];

  columns: Column[] = [
    { key: 'shipmentId', header: 'Shipment ID' },
    { key: 'orderId', header: 'Order ID' },
    { key: 'shipperName', header: 'Courier' },
    { key: 'trackingNumber', header: 'Tracking', formatter: (value: string | null) => value ?? 'Tracking Number Not Created' },
    { key: 'currentStatus', header: 'Status' },
    { key: 'expectedDeliveryDate', header: 'Expected Date', formatter: (value: string) => this.datePipe.transform(value, 'dd/MM/yyyy') }
  ];
  mobileColumns = [...this.columns];


  handleAction(event: { type: string; row: ShipmentModel }) {
    switch (event.type) {
      case 'view':
        this.viewShipment(event.row.shipmentId);
        break;
      case 'update':
        this.openUpdatePopup(event.row.shipmentId);
        break;
    }
  }

  viewShipment(id: number): void {
    this.router.navigate(['/admin/shipment-details', id]);
  }
}