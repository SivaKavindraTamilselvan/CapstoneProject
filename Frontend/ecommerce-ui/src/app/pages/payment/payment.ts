import { Component } from '@angular/core';
import { HttpClient } from '@angular/common/http';

declare var Razorpay: any;

@Component({
  selector: 'app-payment',
  standalone: true,
  templateUrl: './payment.html'
})
export class PaymentComponent {

  constructor(private http: HttpClient) {}

  payNow(orderId: number) {
    const modeOfPaymentId = 2;

    this.http.post<any>(
      'http://localhost:5173/api/Payment/create-payment',
      {
        orderId: orderId,
        modeOfPaymentId: modeOfPaymentId
      }
    ).subscribe({
      next: (payment) => {

        console.log('Create payment response:', payment);

        if (!payment.requiresOnlinePayment) {
          alert('COD order placed successfully');
          return;
        }

        const options = {
          key: 'rzp_test_Sxgi6JUqWE2euU',
          amount: payment.amount, // already in paise
          currency: payment.currency,
          name: 'Ecommerce App',
          description: 'Order Payment',
          order_id: payment.razorpayOrderId,

          handler: (response: any) => {

            console.log('Razorpay success response:', response);

            const verifyRequest = {
              orderId: payment.orderId,
              razorpayOrderId: response.razorpay_order_id,
              razorpayPaymentId: response.razorpay_payment_id,
              razorpaySignature: response.razorpay_signature
            };

            this.http.post(
              'http://localhost:5173/api/Payment/verify-razorpay-payment',
              verifyRequest
            ).subscribe({
              next: () => alert('Payment successful'),
              error: (err) => {
                console.log('Verify error:', err);
                alert('Payment verification failed');
              }
            });
          }
        };

        const razorpay = new Razorpay(options);

        razorpay.on('payment.failed', (response: any) => {

          console.log('Razorpay failed response:', response);

          const failedRequest = {
            orderId: payment.orderId,
            razorpayOrderId: payment.razorpayOrderId,

            errorCode: response.error.code,
            errorDescription: response.error.description,
            errorSource: response.error.source,
            errorStep: response.error.step,
            errorReason: response.error.reason,
            errorField: response.error.field
          };

          this.http.post(
            'http://localhost:5173/api/Payment/payment-failed',
            failedRequest
          ).subscribe({
            next: () => alert('Payment failed and stored in DB'),
            error: (err) => console.log('Failure store error:', err)
          });
        });

        razorpay.open();
      },
      error: (err) => {
        console.log('Create payment error:', err);
        alert('Unable to create payment');
      }
    });
  }
}