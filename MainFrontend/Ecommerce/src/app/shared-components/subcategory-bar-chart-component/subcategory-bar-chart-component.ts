import { Component, computed, input } from '@angular/core';
import { ProductSubCategory } from '../../models/admin/admin-dashboard/kpi.model';

@Component({
  selector: 'app-subcategory-bar-chart-component',
  imports: [],
  templateUrl: './subcategory-bar-chart-component.html',
  styleUrl: './subcategory-bar-chart-component.css',
})
export class SubcategoryBarChartComponent {
  title = input<string>('Products per Subcategory');
  subCategories = input.required<ProductSubCategory[]>();
  loading = input<boolean>(false);

  private maxCount = computed(() =>
    Math.max(...this.subCategories().map(x => x.count), 1)
  );

  barWidth(count: number): number {
    return (count / this.maxCount()) * 100;
  }
}
