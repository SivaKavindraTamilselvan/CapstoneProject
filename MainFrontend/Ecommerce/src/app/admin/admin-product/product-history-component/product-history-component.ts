import { Component, effect, input, signal } from '@angular/core';
import { ApprovalHistoryModel } from '../../../models/product/review-product.model';
import { AdminProductService } from '../../../services/admin-product.Service';
import { DatePipe } from '@angular/common';

@Component({
  selector: 'app-product-history-component',
  imports: [DatePipe],
  templateUrl: './product-history-component.html',
  styleUrl: './product-history-component.css',
})
export class ProductHistoryComponent {
  productId = input.required<number>();
  isVariant = input<boolean>(false);

  protected loading = signal(true);
  protected history = signal<ApprovalHistoryModel[]>([]);
  protected errorMessage = signal<string | null>(null);

  constructor(private adminProductService: AdminProductService) {
    effect(() => {
      const id = this.productId();
      const variantFlag = this.isVariant();

      this.loading.set(true);
      this.errorMessage.set(null);

      this.adminProductService.getApprovalHistory(id, variantFlag).subscribe({
        next: (data) => {
          this.history.set(data);
          this.loading.set(false);
        },
        error: () => {
          this.errorMessage.set('Failed to load approval history.');
          this.loading.set(false);
        }
      });
    });
  }
}
