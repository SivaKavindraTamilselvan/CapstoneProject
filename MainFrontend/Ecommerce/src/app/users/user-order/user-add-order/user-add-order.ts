import { Component, computed, inject, signal } from '@angular/core';
import { UserCartService } from '../../../services/user-cart.Service';
import { UserCouponService } from '../../../services/user-coupon.Service';
import { UserOrderService } from '../../../services/user-order.Service';
import { Router } from '@angular/router';
import { UserCouponModel } from '../../../models/user/coupon/user-coupon.model';
import { AddUserOrderModel } from '../../../models/user/order/place-order.model';
import { AddressService } from '../../../services/address.Service';
import { AddressModel } from '../../../models/address/address-response.model';
import { CommonModule } from '@angular/common';
import { UserPaymentService } from '../../../services/user-payment.Service';
import { ShipmentServiceModel } from '../../../models/user/order/servicibility.model';
import { environment } from '../../../environment.secure';

export type CheckoutStep = 1 | 2 | 3;

export interface ShippingCheckResponse {
  totalShippingCharge: number;
  isShippingAvailable: boolean;
}

const PAYMENT_METHODS = [
  { id: 1, label: 'Cash on Delivery', icon: 'cod' },
  { id: 2, label: 'Credit Card', icon: 'card' },
  { id: 3, label: 'Debit Card', icon: 'card' },
  { id: 4, label: 'UPI', icon: 'upi' },
] as const;

declare var Razorpay: any;

@Component({
  selector: 'app-user-add-order',
  imports: [CommonModule],
  templateUrl: './user-add-order.html',
  styleUrl: './user-add-order.css',
})

export class UserAddOrder {

  constructor(private cartService: UserCartService, private addressService: AddressService, private couponService: UserCouponService, private orderService: UserOrderService, private router: Router, private paymentService: UserPaymentService) {

  }

  ngOnInit(): void {
    this.loadAddresses();
    this.loadCoupons();
    this.loadCartSummary();
  }

  currentStep = signal<CheckoutStep>(1);
  isLoading = signal(false);
  error = signal<string | null>(null);
  successMsg = signal<string | null>(null);
  steps: CheckoutStep[] = [1, 2, 3];

  addresses = signal<AddressModel[]>([]);
  coupons = signal<UserCouponModel[]>([]);
  cartTotal = signal<number>(0);
  cartItemCount = signal<number>(0);

  deliveryCost = signal<number>(0);

  selectedAddressId = signal<number | null>(null);
  selectedPaymentId = signal<number | null>(null);
  selectedCouponId = signal<number | null>(null);
  couponCode = signal<string>('');
  showAddAddress = signal(false);

  shipmentAvailable = signal<boolean | null>(null);
  shipmentMessage = signal<string | null>(null);

  paymentMethods = PAYMENT_METHODS;

  selectedAddress = computed(() =>
    this.addresses().find(a => a.addressId === this.selectedAddressId()) ?? null
  );

  selectedCoupon = computed(() =>
    this.coupons().find(c => c.couponId === this.selectedCouponId()) ?? null
  );

  discount = computed(() => {
    const coupon = this.selectedCoupon();
    if (!coupon) return 0;
    return coupon.discountValue;

  });

  tax = computed(() => {
    const taxableAmount = Math.max(0, this.cartTotal() - this.discount());
    return taxableAmount * 0.18;
  });

  finalTotal = computed(() =>
    Math.max(
      0,
      this.cartTotal() - this.discount() + this.tax() + this.deliveryCost()
    )
  );
  step1Valid = computed(() => this.selectedAddressId() !== null);
  step2Valid = computed(() => this.selectedPaymentId() !== null);


  private loadAddresses(): void {
    this.isLoading.set(true);
    this.addressService.getUserAddress().subscribe({
      next: (data) => { this.addresses.set(data); this.isLoading.set(false); },
      error: () => { this.error.set('Failed to load addresses.'); this.isLoading.set(false); },
    });
  }

  private checkShipment(): void {
    this.shipmentAvailable.set(null);
    this.shipmentMessage.set(null);
    const id = this.selectedPaymentId();
    if (id == null) {
      return;
    }
    const address = this.selectedAddressId();
    if (address == null) {
      return;
    }
    var check = new AddUserOrderModel(address, null, id);
    this.orderService.checkShipment(check).subscribe({
      next: (res) => {
        this.deliveryCost.set(res.totalShippingCharge);
        this.shipmentAvailable.set(res.isShippingAvailable);
        this.shipmentMessage.set('');
      },
      error: (err) => {
        this.shipmentAvailable.set(false);
        this.shipmentMessage.set(
          err.error?.message ?? 'Unable to check delivery availability.'
        );
      }
    });
  }

  private loadCoupons(): void {
    this.couponService.getActiveCoupons().subscribe({
      next: (data) => this.coupons.set(data),
      error: () => { },
    });
  }

  private loadCartSummary(): void {
    this.cartService.getCartItems().subscribe({
      next: (items) => {
        this.cartItemCount.set(items.reduce((s, i) => s + i.qunatity, 0));
        this.cartTotal.set(items.reduce((s, i) => s + i.price * i.qunatity, 0));
      },
      error: () => { },
    });
  }

  goToStep(step: CheckoutStep): void {
    if (step === 2 && !this.step1Valid()) return;
    if (step === 3 && (!this.step1Valid() || !this.step2Valid())) return;
    this.currentStep.set(step);
    this.error.set(null);
  }

  nextStep(): void {
    if (this.currentStep() === 1 && this.step1Valid()) this.currentStep.set(2);
    else if (this.currentStep() === 2 && this.step2Valid()) this.currentStep.set(3);
  }

  prevStep(): void {
    if (this.currentStep() === 2) this.currentStep.set(1);
    else if (this.currentStep() === 3) this.currentStep.set(2);
  }

  selectAddress(id: number): void {
    this.selectedAddressId.set(id);
  }
  selectPayment(id: number): void {
    this.selectedPaymentId.set(id);
    this.checkShipment();
  }

  toggleCoupon(couponId: number): void {
    this.selectedCouponId.set(this.selectedCouponId() === couponId ? null : couponId);
  }
  placeOrderIdempotencyKey = signal<string>(crypto.randomUUID());


  placeOrder(): void {
    if (!this.selectedAddressId() || !this.selectedPaymentId()) return;


    const payload: AddUserOrderModel = {
      addressId: this.selectedAddressId()!,
      couponId: this.selectedCouponId() ?? null,
      paymentMethod: this.selectedPaymentId()!,
    };

    this.isLoading.set(true);
    this.error.set(null);
    const headers = { 'Idempotency-Key': this.placeOrderIdempotencyKey() };

    this.orderService.placeOrder(payload, this.placeOrderIdempotencyKey()).subscribe({
      next: (orderResponse) => {
        const orderId: number = orderResponse.orderId;
        this.initiatePayment(orderId, payload.paymentMethod);
      },
      error: (err) => {
        this.isLoading.set(false);
        this.error.set(err?.error?.message ?? 'Failed to place order. Please try again.');
      },
    });
  }

  private initiatePayment(orderId: number, modeOfPaymentId: number): void {
    this.paymentService.createPayment(orderId, modeOfPaymentId).subscribe({
      next: (payment) => {
        this.isLoading.set(false);

        if (!payment.requiresOnlinePayment) {
          this.router.navigate(['/order-success'], { queryParams: { orderId } });
          return;
        }

        const options = {
          key: payment.razorpayKeyId ?? environment.razorpayKeyId,
          amount: payment.amount,
          currency: payment.currency,
          name: 'Ecommerce App',
          description: 'Order Payment',
          order_id: payment.razorpayOrderId,

          handler: (response: any) => {
            const verifyRequest = {
              orderId: payment.orderId,
              razorpayOrderId: response.razorpay_order_id,
              razorpayPaymentId: response.razorpay_payment_id,
              razorpaySignature: response.razorpay_signature,
            };

            this.isLoading.set(true);
            this.paymentService.verifyPayment(verifyRequest).subscribe({
              next: () => {
                this.isLoading.set(false);
                this.router.navigate(['/user/order-success'], { queryParams: { orderId: payment.orderId } });
              },
              error: (err) => {
                this.isLoading.set(false);
                this.error.set(err?.error?.message ?? 'Payment verification failed. Contact support.');
              },
            });
          },

          modal: {
            ondismiss: () => {
              this.error.set('Payment was cancelled. You can retry from your orders page.');
              this.isLoading.set(false);
            },
          },
        };

        const rzp = new Razorpay(options);

        rzp.on('payment.failed', (response: any) => {
          rzp.close();
          const failedRequest = {
            orderId: payment.orderId,
            razorpayOrderId: payment.razorpayOrderId,
            errorCode: response.error.code,
            errorDescription: response.error.description,
            errorSource: response.error.source,
            errorStep: response.error.step,
            errorReason: response.error.reason,
            errorField: response.error.field,
          };

          this.paymentService.paymentFailed(failedRequest).subscribe({
            next: () => {
              this.router.navigate(['/user/order-failed'], { queryParams: { orderId: payment.orderId } });
            },
            error: () => {
              this.router.navigate(['/user/order-failed'], { queryParams: { orderId: payment.orderId } });
            },
          });
        });

        rzp.open();
      },

      error: (err) => {
        this.isLoading.set(false);
        this.error.set(err?.error?.message ?? 'Unable to initiate payment. Please try again.');
      },
    });
  }

  trackByAddress(_: number, a: AddressModel): number { return a.addressId; }
  trackByCoupon(_: number, c: UserCouponModel): number { return c.couponId; }
}

