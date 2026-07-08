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
import { ShipmentUpdateModel } from '../../../models/admin/admin-shipment/update-shipment.model';

@Component({
  selector: 'app-shipment-list',
  imports: [MobileCardComponent, FilterComponent, DataTableComponent, PaginationComponent, FormField, ReactiveFormsModule, FormsModule],
  providers: [DatePipe],
  templateUrl: './shipment-list.html',
  styleUrl: './shipment-list.css',
})
export class ShipmentList extends BasePage {

  actions: TableAction<ShipmentModel>[] = [
    {
      label: 'View',
      color: 'green',
      action: 'view'
    },
    {
      label: 'Update',
      color: 'blue',
      action: 'update'
    }
  ];

  columns: Column[] = [
    {
      key: 'shipmentId',
      header: 'Shipment ID'
    },
    {
      key: 'orderId',
      header: 'Order ID'
    },
    {
      key: 'shipperName',
      header: 'Courier'
    },
    {
      key: 'trackingNumber',
      header: 'Tracking'
    },
    {
      key: 'currentStatus',
      header: 'Status'
    },
    {
      key: 'expectedDeliveryDate',
      header: 'Expected Date',
      formatter: (value: string) =>
        this.datePipe.transform(value, 'dd/MM/yyyy')
    }
  ];

  mobileColumns: Column[] = [
    {
      key: 'shipmentId',
      header: 'Shipment ID'
    },
    {
      key: 'orderId',
      header: 'Order ID'
    },
    {
      key: 'shipperName',
      header: 'Courier'
    },
    {
      key: 'trackingNumber',
      header: 'Tracking'
    },
    {
      key: 'currentStatus',
      header: 'Status'
    },
    {
      key: 'expectedDeliveryDate',
      header: 'Expected Date',
      formatter: (value: string) =>
        this.datePipe.transform(value, 'dd/MM/yyyy')
    }
  ];

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

  constructor(
    private datePipe: DatePipe,
    private router: Router,
    private shipmentService: AdminShipmentService
  ) {
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

  handleAction(event: { type: string; row: ShipmentModel }) {
    switch (event.type) {
      case 'view':
        this.viewShipment(event.row.shipmentId);
        break;
      case 'update':
        this.openDeletePopup(event.row.shipmentId);
        break;
    }
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

  viewShipment(id: number): void {
    this.router.navigate(['/admin/shipment-details', id]);
  }


    showActivatePopup = signal(false);
    successMessage = signal<string | null>(null);
    errorMessage = signal<string | null>(null);
    updateShipmentModel = signal(new ShipmentUpdateModel());
    selectedShipmentId = signal<number | null>(null);

  updateForm = form(this.updateShipmentModel, (path) => {
    required(path.remarks, { message: "Enter The Remarks" });
    required(path.location, { message: "Enter The Location" });
    required(path.shipmentStatusId, { message: "Enter The Shipment Status" });
  })

  updateShipment() {
    if (this.updateForm().invalid()) return;

    this.errorMessage.set(null);
    this.successMessage.set(null);

    if (this.updateForm().invalid()) {
      this.errorMessage.set("Enter proper details");
      return;
    }

    const request = {
      shipmentStatusId:  Number(this.updateShipmentModel().shipmentStatusId),
      remarks: this.updateShipmentModel().remarks,
      shipmentId: this.updateShipmentModel().shipmentId,
      location: this.updateShipmentModel().location,
    };

    this.shipmentService.updateShipment(request).subscribe({
      next: () => {
        this.successMessage.set("Shipment updated successfully. Closing in 3 seconds...");
        setTimeout(() => {
          this.closePopup();
          this.successMessage.set(null);
          this.loadShipments();
        }, 3000);
      },
      error: (error) => {
        this.successMessage.set(null);

        if (error.status === 400 && error.error?.errors) {
          const messages = Object.values(error.error.errors)
            .flat()
            .join(", ");

          this.errorMessage.set(messages);
        }
        else {
          this.errorMessage.set(
            error.error?.message ?? "Something went wrong. Please try again."
          );
        }
      }
    });
  }

  openDeletePopup(shipmentId: number) {
    this.selectedShipmentId.set(shipmentId);

    this.updateShipmentModel.update(model => ({
      ...model,
      shipmentId: shipmentId,
      remark: '',
      location: '',
      shipmentStatusId : ''
    }));

    this.showActivatePopup.set(true);
  }

  closeUpdatePopup() {
    this.showActivatePopup.set(false);
    this.selectedShipmentId.set(null);
    this.updateShipmentModel.set(new ShipmentUpdateModel());
    this.errorMessage.set(null);
  }
}