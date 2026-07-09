import { Component, computed, input } from '@angular/core';

export interface OrdersByMonthItem {
  month: string;
  count: number;
}

@Component({
  selector: 'app-orders-by-month-chart-component',
  standalone: true,
  imports: [],
  templateUrl: './orders-by-month-chart-component.html',
  styleUrl: './orders-by-month-chart-component.css',
})
export class OrdersByMonthChartComponent {
  title = input<string>('Orders by Month');
  subtitle = input<string>('Monthly order growth.');
  ordersByMonth = input.required<OrdersByMonthItem[]>();
  loading = input<boolean>(false);

  private maxOrderCount = computed(() =>
    Math.max(...this.ordersByMonth().map(x => x.count), 1)
  );

  monthBarWidth(count: number): number {
    const max = this.maxOrderCount();
    return max > 0 ? (count / max) * 100 : 0;
  }
}