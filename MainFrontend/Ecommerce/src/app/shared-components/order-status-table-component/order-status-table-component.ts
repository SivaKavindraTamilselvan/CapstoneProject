import { Component, input } from '@angular/core';
import { NgClass } from '@angular/common';

export interface OrderStatusItem {
  status: string;
  count: number;
}

@Component({
  selector: 'app-order-status-table-component',
  standalone: true,
  imports: [NgClass],
  templateUrl: './order-status-table-component.html',
  styleUrl: './order-status-table-component.css',
})
export class OrderStatusTableComponent {
  title = input<string>('Orders by Status');
  subtitle = input<string>('Distribution of orders across different statuses.');
  orderStatus = input.required<OrderStatusItem[]>();
  loading = input<boolean>(false);

  statusClasses(status: string): string {
    const key = status.toLowerCase();
    if (key.includes('cancel') || key.includes('reject')) return 'bg-rose-50 text-rose-700 ring-1 ring-rose-200';
    if (key.includes('pending') || key.includes('process')) return 'bg-amber-50 text-amber-700 ring-1 ring-amber-200';
    if (key.includes('deliver') || key.includes('complet')) return 'bg-emerald-50 text-emerald-700 ring-1 ring-emerald-200';
    if (key.includes('ship')) return 'bg-sky-50 text-sky-700 ring-1 ring-sky-200';
    return 'bg-slate-50 text-slate-600 ring-1 ring-slate-200';
  }
}