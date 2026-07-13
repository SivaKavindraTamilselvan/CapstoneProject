import { Component, input, output } from '@angular/core';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { FieldTree, FormField } from '@angular/forms/signals';
import { AiValidationResult } from '../../models/ai.model';

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

  // --- AI check additions (all optional so other usages are unaffected) ---
  showAiCheck = input(false);          // toggle: only product review popup passes true
  aiReview = input<AiValidationResult | null>(null);
  loadingAi = input(false);

  aiCheckRequested = output<void>();
  // ------

  onConfirm() {
    this.confirm.emit();
  }

  onCancel() {
    this.cancel.emit();
  }
  onRunAiCheck() {
    this.aiCheckRequested.emit();
  }

  onStatusChange(event: Event): void {
    const value = (event.target as HTMLSelectElement).value;
    this.statusField()().value.set(value ? Number(value) : null);
  }
}