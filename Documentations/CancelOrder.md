# Cancel Service Module

## When Can an Order Item Be Cancelled?
- **Not restricted to Pending alone** — cancellation is allowed while the order item status is either **Pending** or **Packed**.
- Once the item moves past Packed (e.g. shipped/ready-for-pickup/delivered), cancellation is blocked outright with `"Order Already Shipped. Cannot be cancelled"`.
- Ownership check: the order item must belong to the requesting user — a user cannot cancel another user's order item.
- Cancel quantity cannot exceed the quantity actually ordered.

## Refund Calculation
- `orderItemAmount = Quantity * UnitPrice - Discount`
- `ConvenienceFee = orderItemAmount * 5%`
- `refundAmount = orderItemAmount - ConvenienceFee`
- Every cancellation deducts a flat 5% convenience fee from the refund, regardless of cancellation reason.

## Full/Partial Cancellation Handling
- **Full cancellation** (cancel quantity = ordered quantity): the existing `OrderItems` row is updated in place to `Cancelled` status.
- **Partial cancellation** (cancel quantity < ordered quantity): the original row's quantity is reduced, and a **new separate `OrderItems` row** is created for the cancelled portion (status `Cancelled`), with its discount proportionally split (`Discount / (remaining + cancelled) * cancelled`) so both rows carry an accurate discount share.
- Each path writes its own audit log entry.

## Order-Level Status Check
- After any cancellation, the parent `Order` is re-checked: if **no non-cancelled items remain**, the whole `Order` is marked `Cancelled`.
- If other active items still exist on the order, the order status is left untouched — only the individual item's status changed.

## Inventory Release
- Cancelled quantity is added back to `AvailableQuantity` and removed from `ReservedQuantity` on the relevant `Inventory` row.
- Both before/after values logged in the audit trail.

## Wallet Involvement — Refund Path
- **Refund is only created for non-Cash-On-Delivery orders.** The order's payment method is checked (`GetOrderInvoiceData(order.OrderId).PaymentMethod`); if it equals `"Cash On Delivery"`, `CreateRefund` is skipped entirely — there's nothing to refund back electronically since no online payment was actually collected up front.
- For all other payment methods (online/card/UPI/etc.), `CreateRefund` runs:
  1. Creates a `Refund` row (`RefundStatusId = Processed`, `ActualRefundAmount = refundAmount`)
  2. Creates a `CancelRefund` link row tying the `Cancel` record to the new `Refund` record
  3. **Credits the customer's wallet directly** — `user.WalletCost += refundAmount` — this is how the refund is actually delivered to the customer, not a reversal back to their original payment instrument
  4. Refund status then flipped to `Completed` (`ProcessedDate` set)
  5. The related `Cancel` record's own status flipped to `Refunded` (distinct from the `Cancel`'s initial `Approved` status set at cancellation time)
- Every wallet credit step, refund creation, and cancel-refund link is individually audit-logged (old/new wallet balance included).

## Notifications Sent
- **Vendor owner** — notified that an order item was cancelled by the customer, including quantity cancelled (skipped gracefully with a logged warning if no vendor/owner can be resolved).
- **All Order Admin users** — notified of the cancellation (loop over all admins with order-management access; empty list logged and skipped rather than erroring).
- **Customer (requesting user)** — sent a confirmation notification stating the refund amount that will be issued.
- All three notification sends happen after the core cancellation logic (status update, inventory release, refund) has completed.

## Edge Cases Handled
- Cancellation blocked once an item has moved beyond Pending/Packed (already shipped)
- Cancel quantity capped at the originally ordered quantity
- Cross-user cancellation attempts blocked (`UnauthorizationException`)
- COD orders skip refund/wallet-credit entirely, since no payment was collected upfront to refund
- Partial cancellation correctly splits quantity and proportional discount into a new row rather than losing data on the original
- Parent order auto-marked Cancelled only when every item under it is cancelled, not on the first cancellation
- Missing vendor/vendor-owner resolution for notifications handled without failing the cancellation itself
- Refund and Cancel statuses tracked as two related but independent lifecycles: `Cancel.CancelStatusId` (Approved → Refunded) and `Refund.RefundStatusId` (Processed → Completed), both updated in sequence as the refund actually completes