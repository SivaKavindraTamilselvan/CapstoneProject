import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { BaseURL } from '../environment';

@Injectable({
  providedIn: 'root'
})
export class UserPaymentService {

  constructor(private http: HttpClient) {}

  createPayment(orderId: number, modeOfPaymentId: number): Observable<any> {
    return this.http.post<any>(
      `${BaseURL}/Payment/create-payment`,
      {
        orderId,
        modeOfPaymentId
      }
    );
  }

  verifyPayment(request: any): Observable<any> {
    return this.http.post(
      `${BaseURL}/Payment/verify-razorpay-payment`,
      request
    );
  }

  paymentFailed(request: any): Observable<any> {
    return this.http.post(
      `${BaseURL}/Payment/payment-failed`,
      request
    );
  }
}