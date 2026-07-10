import { Component, input } from '@angular/core';
import { ProductApprovalStatus } from '../../models/admin/admin-dashboard/kpi.model';
import { NgClass } from '@angular/common';

@Component({
  selector: 'app-approval-status-componenet',
  imports: [NgClass],
  templateUrl: './approval-status-componenet.html',
  styleUrl: './approval-status-componenet.css',
})
export class ApprovalStatusComponenet {
  title = input<string>('Products by Approval Status');
  approvalStatus = input.required<ProductApprovalStatus[]>();
  loading = input<boolean>(false);

  statusClasses(status: string): string {
    const map: Record<string, string> = {
      'Pending': 'bg-amber-50 text-amber-700',
      'Vendor Approved': 'bg-blue-50 text-blue-700',
      'Vendor Rejected': 'bg-red-50 text-red-700',
      'Admin Approved': 'bg-emerald-50 text-emerald-700',
      'Admin Rejected': 'bg-red-50 text-red-700',
      'Deleted By Admin': 'bg-slate-100 text-slate-600',
    };
    return map[status] ?? 'bg-slate-100 text-slate-600';
  }
}
