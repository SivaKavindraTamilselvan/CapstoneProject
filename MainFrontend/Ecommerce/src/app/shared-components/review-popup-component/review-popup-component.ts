import { Component, input, output } from '@angular/core';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { FieldTree, FormField } from '@angular/forms/signals';

export interface ReviewStatusOption {
  id: number;
  label: string;
}

@Component({
  selector: 'app-review-popup-component',
  imports: [FormField,ReactiveFormsModule,FormsModule],
  templateUrl: './review-popup-component.html',
  styleUrl: './review-popup-component.css',
})
export class ReviewPopupComponent {
  show = input(false);
  title = input('Review Vendor');

  message = input('Vendor can be accepted or rejected?');

  statusLabel = input('Approval Status');
  statusPlaceholder = input('Select The Status');
  statusOptions = input.required<ReviewStatusOption[]>();
  statusField = input.required<FieldTree<number | null>>();

  remarkLabel = input('Remarks');
  remarkPlaceholder = input('Enter The Remarks');
  remarkField = input.required<FieldTree<string>>();

  confirmText = input('Submit');
  cancelText = input('Cancel');
  loadingText = input('Progressing...');
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