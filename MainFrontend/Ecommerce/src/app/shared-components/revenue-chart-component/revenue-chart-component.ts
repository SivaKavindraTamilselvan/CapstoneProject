import { Component, input, output } from '@angular/core';
import { RevenueTrend } from '../../models/admin/admin-dashboard/kpi.model';
import { CurrencyPipe } from '@angular/common';

@Component({
  selector: 'app-revenue-chart-component',
  imports: [CurrencyPipe],
  templateUrl: './revenue-chart-component.html',
  styleUrl: './revenue-chart-component.css',
})
export class RevenueChartComponent {
  revenueTrend = input.required<RevenueTrend[]>();
  title = input<string>('Revenue Trend');
  subtitle = input<string>('Revenue based on delivered orders');

  periodChange = output<string>();

  changePeriod(period: string) {
    this.periodChange.emit(period);
  }
}
