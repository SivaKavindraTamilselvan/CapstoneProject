import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';

@Injectable({
  providedIn: 'root'
})
export class PaymentService {

  private apiUrl = 'http://localhost:5173/api/Payment';

  constructor(private http: HttpClient) {}

  createPayment(orderId: number, modeOfPaymentId: number) {
    return this.http.post<any>(
      `${this.apiUrl}/create-payment`,
      {
        orderId: orderId,
        modeOfPaymentId: modeOfPaymentId
      }
    );
  }

  verifyPayment(data: any) {
    return this.http.post<any>(
      `${this.apiUrl}/verify-razorpay-payment`,
      data
    );
  }
}