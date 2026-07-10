import { Component, effect, input, output, signal } from '@angular/core';
import { VendorInventoryService } from '../../../services/vendor-inventory.Service';
import { UpdateInventoryModel } from '../../../models/inventory/update-inventory.model';
import { form, FormField, required } from '@angular/forms/signals';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';

@Component({
  selector: 'app-update-ineventory-component',
  imports: [FormsModule,ReactiveFormsModule,FormField],
  templateUrl: './update-ineventory-component.html',
  styleUrl: './update-ineventory-component.css',
})
export class UpdateIneventoryComponent {
    constructor(private inventoryService: VendorInventoryService) {
    effect(() => {
      const id = this.inventoryId();
      if (id != null) {
        this.updateModel.update(m => ({ ...m, inventoryId: id }));
      }
    });
  }

  inventoryId = input<number | null>(null);

  closed = output<void>();
  updated = output<void>();

  updateModel = signal(new UpdateInventoryModel());
  successMessage = signal<string | null>(null);
  errorMessage = signal<string | null>(null);
  progress = signal(false);

  updateForm = form(this.updateModel, (path) => {
    required(path.inventoryId, { message: 'Choose The Inventory Id' });
    required(path.availableQuantity, { message: 'Enter The Available Quantity' });
  });

  onUpdateTypeChange(event: Event): void {
    const value = (event.target as HTMLSelectElement).value === 'true';
    this.updateModel.update(m => ({ ...m, updateType: value }));
  }

  updateInventory() {
    this.errorMessage.set(null);
    this.successMessage.set(null);

    if (this.updateForm().invalid()) {
      this.errorMessage.set("Enter proper details");
      return;
    }

    this.progress.set(true);
    this.inventoryService.updateInventory(this.updateModel()).subscribe({
      next: () => {
        this.progress.set(false);
        this.successMessage.set("Inventory updated successfully. Closing in 3 seconds...");
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
          const messages = Object.values(error.error.errors).flat().join(", ");
          this.errorMessage.set(messages);
        } else {
          this.errorMessage.set(error.error?.message ?? "Something went wrong. Please try again.");
        }
      }
    });
  }

  close() {
    this.updateModel.set(new UpdateInventoryModel());
    this.errorMessage.set(null);
    this.successMessage.set(null);
    this.closed.emit();
  }
}
