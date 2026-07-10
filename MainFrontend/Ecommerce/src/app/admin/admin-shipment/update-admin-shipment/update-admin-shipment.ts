import { Component, effect, input, output, signal } from '@angular/core';
import { AdminShipmentService } from '../../../services/admin-shipment.Service';
import { ShipmentUpdateModel } from '../../../models/admin/admin-shipment/update-shipment.model';
import { form, FormField, required } from '@angular/forms/signals';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';

@Component({
  selector: 'app-update-admin-shipment',
  imports: [FormField,ReactiveFormsModule,FormsModule],
  templateUrl: './update-admin-shipment.html',
  styleUrl: './update-admin-shipment.css',
})
export class UpdateAdminShipment {
  constructor(private shipmentService: AdminShipmentService) {
    effect(() => {
      const id = this.shipmentId();
      if (id != null) {
        this.updateShipmentModel.update(model => ({
          ...model,
          shipmentId: id,
          remarks: '',
          location: '',
          shipmentStatusId: ''
        }));
      }
    });
  }

  progress = signal(false);

  shipmentId = input<number | null>(null);

  closed = output<void>();
  updated = output<void>();

  successMessage = signal<string | null>(null);
  errorMessage = signal<string | null>(null);
  updateShipmentModel = signal(new ShipmentUpdateModel());

  updateForm = form(this.updateShipmentModel, (path) => {
    required(path.remarks, { message: "Enter The Remarks" });
    required(path.location, { message: "Enter The Location" });
    required(path.shipmentStatusId, { message: "Enter The Shipment Status" });
  });

  updateShipment() {
    if (this.updateForm().invalid()) {
      this.errorMessage.set("Enter proper details");
      return;
    }

    this.errorMessage.set(null);
    this.successMessage.set(null);
    this.progress.set(true);

    const request = {
      shipmentStatusId: Number(this.updateShipmentModel().shipmentStatusId),
      remarks: this.updateShipmentModel().remarks,
      shipmentId: this.updateShipmentModel().shipmentId,
      location: this.updateShipmentModel().location,
    };

    this.shipmentService.updateShipment(request).subscribe({
      next: () => {
        this.successMessage.set("Shipment updated successfully. Closing in 3 seconds...");
        setTimeout(() => {
          this.successMessage.set(null);
          this.updated.emit();
          this.close();
        }, 3000);
      },
      error: (error) => {
        this.progress.set(false);
        this.successMessage.set(null);

        if (error.status === 400 && error.error?.errors) {
          const messages = Object.values(error.error.errors)
            .flat()
            .join(", ");
          this.errorMessage.set(messages);
        } else {
          this.errorMessage.set(
            error.error?.message ?? "Something went wrong. Please try again."
          );
        }
      }
    });
  }

  close() {
    this.updateShipmentModel.set(new ShipmentUpdateModel());
    this.errorMessage.set(null);
    this.successMessage.set(null);
    this.closed.emit();
  }
}
