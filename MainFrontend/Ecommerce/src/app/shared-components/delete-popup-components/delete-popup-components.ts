import { Component, input, output } from '@angular/core';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { FormField, FieldTree } from '@angular/forms/signals';

@Component({
  selector: 'app-delete-popup-components',
  imports: [FormField,ReactiveFormsModule,FormsModule],
  templateUrl: './delete-popup-components.html',
  styleUrl: './delete-popup-components.css',
})
export class DeletePopupComponents {
  show = input(false);
  title = input('Delete');
  message = input('Are you sure you want to delete this item?');
  confirmText = input('Delete');
  cancelText = input('Cancel');
  loadingText = input('Deleting...');
  loading = input(false);
  successMessage = input('');
  errorMessage = input('');
  remarkLabel = input('Remarks');
  remarkPlaceholder = input('Enter remarks');
  remarkField = input.required<FieldTree<string>>();

  confirm = output<void>();
  cancel = output<void>();

  onConfirm() {
    this.confirm.emit();
  }

  onCancel() {
    this.cancel.emit();
  }
}