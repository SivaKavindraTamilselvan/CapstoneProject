import { Component, effect, input, output, signal } from '@angular/core';
import { form, FormField, required } from '@angular/forms/signals';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { VendorInventoryService } from '../../../services/vendor-inventory.Service';
import { AddInventoryModel } from '../../../models/inventory/add-inventory.model';

@Component({
  selector: 'app-add-inventory-component',
  standalone: true,
  imports: [FormField, ReactiveFormsModule, FormsModule],
  templateUrl: './add-inventory-component.html',
  styleUrl: './add-inventory-component.css',
})
export class AddInventoryComponent {
  constructor(private vendorInventoryService: VendorInventoryService) {
    effect(() => {
      const id = this.addressId();
      if (id != null) {
        this.inventoryModel.update(i => ({ ...i, addressId: id }));
      }
    });
  }

  addressId = input<number | null>(null);

  closed = output<void>();
  added = output<void>();

  inventoryModel = signal(new AddInventoryModel());
  successMessage = signal<string | null>(null);
  errorMessage = signal<string | null>(null);
  progress = signal(false);

  addForm = form(this.inventoryModel, (path) => {
    required(path.addressId, { message: 'Choose The Address' });
    required(path.availableQuantity, { message: 'Enter the available quantity' });
    required(path.reservedQuantity, { message: 'Enter the reserved quantity' });
    required(path.productVariantId, { message: 'Choose the product variant' });
  });

  handleAddInventory() {
    this.errorMessage.set(null);
    this.successMessage.set(null);

    if (this.addForm().invalid()) {
      this.errorMessage.set("Enter proper details");
      return;
    }

    this.progress.set(true);
    this.vendorInventoryService.addInventory(this.inventoryModel()).subscribe({
      next: () => {
        this.progress.set(false);
        this.successMessage.set("Inventory added successfully");
        setTimeout(() => {
          this.successMessage.set(null);
          this.added.emit();
          this.close();
        }, 3000);
      },
      error: (error) => {
        this.progress.set(false);
        this.successMessage.set(null);

        if (error.status === 400 && error.error?.errors) {
          const messages = Object.values(error.error.errors).flat().join(", ");
          this.errorMessage.set(messages);
        }
        else if (error.status === 500) {
          this.errorMessage.set("Inventory already exists for the selected product variant and address.");
        }
        else {
          this.errorMessage.set(error.error?.message ?? "Something went wrong. Please try again.");
        }
      }
    });
  }

  close() {
    this.inventoryModel.set(new AddInventoryModel());
    this.errorMessage.set(null);
    this.successMessage.set(null);
    this.closed.emit();
  }
}