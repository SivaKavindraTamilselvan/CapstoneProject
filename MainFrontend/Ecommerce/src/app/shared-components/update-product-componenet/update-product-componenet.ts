import { Component, input, output } from '@angular/core';
import { FieldTree, FormField } from '@angular/forms/signals';

export interface UpdatePopupOption {
  id: number;
  label: string;
}

@Component({
  selector: 'app-update-product-componenet',
  imports: [FormField],
  templateUrl: './update-product-componenet.html',
  styleUrl: './update-product-componenet.css',
})

export class UpdateProductComponenet {
  show = input(false);
  title = input('Update');
  message = input('Update the details below.');

  statusLabel = input('Status');
  statusOptions = input<UpdatePopupOption[]>([]);
  statusField = input.required<FieldTree<number | null>>();

  confirmText = input('Update');
  cancelText = input('Cancel');
  loadingText = input('Updating...');
  loading = input(false);
  successMessage = input('');
  errorMessage = input('');

  confirm = output<void>();
  cancel = output<void>();

  onConfirm() {
    this.confirm.emit();
  }

  onCancel() {
    this.cancel.emit();
  }

  onStatusChange(event: Event): void {
    const value = (event.target as HTMLSelectElement).value;
    this.statusField()().value.set(value ? Number(value) : null);
  }
}
