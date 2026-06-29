import { Component, inject, signal } from '@angular/core';
import { ActivatedRoute, ActivatedRouteSnapshot, Router } from '@angular/router';

@Component({
  selector: 'app-order-failure',
  imports: [],
  templateUrl: './order-failure.html',
  styleUrl: './order-failure.css',
})
export class OrderFailure {
  orderId = signal<string | null>(null);

  constructor(private router: Router, private route: ActivatedRoute) {

  }

  ngOnInit(): void {
    this.route.queryParams.subscribe(p => this.orderId.set(p['orderId'] ?? null));
  }

  retry(): void { this.router.navigate(['/user/checkout']); }
  goToOrders(): void { this.router.navigate(['/orders']); }
}

