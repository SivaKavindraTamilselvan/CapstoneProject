import { Component, input, output } from '@angular/core';
import { NgClass } from '@angular/common';

import { Column } from '../data-table-component/column.model';
import { TableAction } from '../data-table-component/table-actions.model';

@Component({
  selector: 'app-detailed-card-componenet',
  standalone: true,
  imports: [NgClass],
  templateUrl: './detailed-card-componenet.html',
  styleUrl: './detailed-card-componenet.css',
})
export class DetailedCardComponenet {

  title = input.required<string>();

  row = input.required<any>();

  columns = input.required<Column[]>();

  actions = input<TableAction<any>[]>([]);

  actionClick = output<{ type: string; row: any }>();

  getCellValue(column: Column): any {
    const value = this.row()[column.key];

    return column.formatter
      ? column.formatter(value, this.row())
      : value;
  }

  isActionVisible(action: TableAction<any>): boolean {
    return action.visible ? action.visible(this.row()) : true;
  }

  onAction(action: TableAction<any>): void {
    this.actionClick.emit({
      type: action.action,
      row: this.row()
    });
  }
}