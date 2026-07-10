import { Component, signal } from '@angular/core';
import { CancelSummaryModel } from '../../../models/user/order/cancel.order.model';
import { AdminOrderService } from '../../../services/admin-order.Service';
import { ActivatedRoute, Router } from '@angular/router';
import { DatePipe, DecimalPipe, NgClass } from '@angular/common';

@Component({
  selector: 'app-admin-cancel-detail-order',
  imports: [DatePipe,DecimalPipe,NgClass],
  templateUrl: './admin-cancel-detail-order.html',
  styleUrl: './admin-cancel-detail-order.css',
})
export class AdminCancelDetailOrder {
  cancelDetail = signal<CancelSummaryModel | null>(null);
  loading = signal(false);

  constructor(
    private adminCancelService: AdminOrderService,
    private route: ActivatedRoute,
    private router: Router
  ) {}

  ngOnInit() {
    const cancelId = Number(this.route.snapshot.paramMap.get('id'));
    if (cancelId) {
      this.loadCancelDetail(cancelId);
    }
  }

  loadCancelDetail(cancelId: number) {
    this.loading.set(true);
    this.adminCancelService.getCancelOrdersDetails(cancelId).subscribe({
      next: (response: any) => {
        this.cancelDetail.set(response);
        this.loading.set(false);
      },
      error: (error) => {
        console.log(error);
        this.loading.set(false);
      }
    });
  }

  goBack() {
    this.router.navigate(['admin/orders/cancelled-orders']);
  }

  statusClass(status: string): string {
    switch (status) {
      case 'Approved':
        return 'bg-green-100 text-green-700 border border-green-300';
      case 'Rejected':
        return 'bg-red-100 text-red-700 border border-red-300';
      case 'Pending':
        return 'bg-yellow-100 text-yellow-700 border border-yellow-300';
      default:
        return 'bg-gray-100 text-gray-700 border border-gray-300';
    }
  }
}
