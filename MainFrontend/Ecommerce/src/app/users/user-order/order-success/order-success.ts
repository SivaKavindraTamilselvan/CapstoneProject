import { Component, inject, signal } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';

@Component({
  selector: 'app-order-success',
  imports: [],
  templateUrl: './order-success.html',
  styleUrl: './order-success.css',
})
export class OrderSuccess {
  private router = inject(Router);
  private route  = inject(ActivatedRoute);
 
  orderId = signal<string | null>(null);
 
  ngOnInit(): void {
    this.route.queryParams.subscribe(p => this.orderId.set(p['orderId'] ?? null));
  }
 
  goToOrders(): void  { this.router.navigate(['/user/orders']); }
  goToHome(): void    { this.router.navigate(['/']); }
}
 
