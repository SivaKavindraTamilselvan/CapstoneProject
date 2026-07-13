import { Component, signal } from '@angular/core';
import { ReturnSummaryModel } from '../../../models/user/order/return.order.model';
import { AdminOrderService } from '../../../services/admin-order.Service';
import { ActivatedRoute, Router } from '@angular/router';
import { DatePipe, DecimalPipe, NgClass } from '@angular/common';

@Component({
  selector: 'app-admin-retur-details',
  imports: [DatePipe, DecimalPipe, NgClass],
  providers : [DatePipe],
  templateUrl: './admin-retur-details.html',
  styleUrl: './admin-retur-details.css',
})
export class AdminReturDetails {

  returnDetail = signal<ReturnSummaryModel | null>(null);
  loading = signal(false);

  constructor(
    private adminReturnService: AdminOrderService,
    private route: ActivatedRoute,
    private router: Router
  ) { }

  ngOnInit() {
    const returnId = Number(this.route.snapshot.paramMap.get('id'));
    if (returnId) {
      this.loadReturnDetail(returnId);
    }
  }

  loadReturnDetail(returnId: number) {
    this.loading.set(true);
    this.adminReturnService.getReturnOrdersDetails(returnId).subscribe({
      next: (response: any) => {
        this.returnDetail.set(response);
        this.loading.set(false);
      },
      error: (error) => {
        console.log(error);
        this.loading.set(false);
      }
    });
  }

  goBack() {
    this.router.navigate(['admin/orders/return-orders']);
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
