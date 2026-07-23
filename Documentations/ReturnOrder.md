# Return Lifecycle Module (Customer Return → Vendor Review → Shipment → Inspection → Refund)

## Overview of the Full Flow
```
Customer requests return
        |
Vendor Reviews (Approve / Reject)
        |
   Approved? ----No----> Rejected (terminal, customer notified, no shipment/refund)
        |
       Yes
        |
Return Pickup Shipment auto-created (courier serviceability + tracking)
        |
Product physically picked up and received by vendor
        |
Vendor Inspects the Product (post-receipt) --> two possible outcomes:
        |
   +---------------------------+
   |                           |
Accept                     Dispute
(no damage found)          (damage found, DamageCost recorded)
   |                           |
Refund auto-created        Return marked "DisputeReturn"
Wallet credited            (Refund creation is a separate,
Return -> RefundProcessed   later/manual step using the
                            recorded DamageCost as the
                            allowed deduction ceiling)
```

## Step 1 — Customer Requests a Return (`AddReturn`)
- Product variant must be flagged as return-eligible (`IsReturn`)
- Return quantity cannot exceed the ordered quantity
- Order item must actually be **Delivered** (status = 4) and have a shipment `DeliveryDate` set — cannot request a return before delivery
- **7-day return window** — rejected if `DateTime.Now > DeliveryDate + 7 days`
- Duplicate return request blocked — only one active return per order item
- Full vs. partial return handled the same way cancellation is: full return updates the existing order item in place (status `Return_Requested`); partial return splits into a reduced original row + a new row for the returned quantity (status 9), with discount proportionally split
- **Convenience Fee locked in at request time:** `ConvenienceFee = UnitPrice * ReturnQuantity * 15%` — always applied to every return, independent of whatever the vendor later decides about damage

## Step 2 — Vendor Reviews the Return Request (`ReviewReturnOrder`)
- This is the **first** vendor decision point — Approve or Reject the *request itself*, before any physical pickup happens
- **Reject:** Return status → `Rejected`. Customer notified. Terminal — no shipment, no refund.
- **Approve:** Return status → `Approved`. Triggers `AdminReturnService.CreateReturnShipment` automatically in the same flow:
  - Resolves customer address (pickup point), vendor's inventory address (delivery point for the returned goods), and the product variant's weight
  - Checks courier serviceability via ShipRocket and gets a shipping rate/estimated days
  - Creates the return `Shipment` (`ShipmentTypeId = 2`, distinguishing it from a normal forward shipment), links the order item to it, generates a tracking number (`SHIPTRACK-RETURN{ShipmentId}`), sets status to `Ready_For_Pick_Up`
  - Creates an initial tracking entry ("Return shipment created from customer location")
  - Releases the returned quantity back to `AvailableQuantity` on the vendor's inventory immediately at shipment-creation time (not deferred to inspection)
- Customer notified either way (Approved: "return shipment has been arranged" / Rejected: rejection message)

## Step 3 — Product Picked Up and Received by Vendor
- (Physical/logistics step — return shipment tracking progresses through pickup to delivery back at the vendor, using the same shipment-tracking mechanism as forward orders)

## Step 4 — Vendor Inspects the Returned Product (Two Outcomes)

**Outcome A: Accept (`AcceptReturnProduct`)** — no issue found with the returned item
- Return status → `RefundProcessed`
- Immediately calls `AdminRefundService.CreateReturnRefund` with `RefundAmount = 0` (no deduction beyond the standard convenience fee) — refund processing happens automatically as part of acceptance
- Customer notified: "return accepted, refund is being processed"

**Outcome B: Dispute (`ReviewReturnOrderProduct`)** — vendor finds damage on the returned item
- **Damage cost cap:** vendor-declared `DamageCost` cannot exceed 50% of the order item's refundable amount (`Quantity*UnitPrice - Discount`)
- Return status → `DisputeReturn`, `DamageCost` and `VendorReview` (remarks) recorded
- Customer notified of the dispute and the vendor's stated reason
- **This does not automatically create a refund** — disputing sets the recorded damage cost as a ceiling, but actual refund creation (`CreateReturnRefund`) with a real deduction amount is a separate, later step (typically an admin using the vendor's recorded `DamageCost` to decide how much to actually deduct)

## Step 5 — Refund Creation & Wallet Credit (`CreateReturnRefund`)
- Callable both directly (admin-driven, after a dispute) and automatically (from `AcceptReturnProduct` with zero deduction)
- Validates the admin's requested deduction (`RefundAmount`) does not exceed the vendor's recorded `DamageCost` — the admin can't deduct more than what was actually flagged as damage
- **Two deductions stack in the final refund calculation:**
  ```
  refundAmount = (Quantity * UnitPrice - Discount) - RefundAmount(admin deduction) - ConvenienceFee(15%, from request time)
  ```
- Refund amount bounds-checked: cannot exceed the order item's total refundable amount, cannot go negative
- Creates `Refund` (status `Processed`) and `ReturnRefund` (records the deduction amount + reason)
- Return status → `RefundProcessed`
- **Wallet credited directly** (`user.WalletCost += refundAmount`) — same delivery mechanism as cancellation refunds, not a reversal to original payment method
- Refund then flipped to `Completed` (`ProcessedDate` set)
- Return's `ReviewRemarks` updated with the admin's final remarks
- Customer notified that the refund has been processed and credited to their wallet
- Runs inside a nested-transaction-safe wrapper — participates in the caller's transaction when called from `AcceptReturnProduct`, or manages its own when called standalone by an admin

## Two Distinct Deductions on Every Return — Summary
| Deduction | When Set | Who Controls It | Always Applied? |
|---|---|---|---|
| Convenience Fee (15%) | At return request time | System (fixed formula) | Yes, on every return |
| Damage Cost / Admin Deduction | At vendor dispute review | Vendor declares a ceiling; Admin decides actual amount up to that ceiling | Only if disputed / admin chooses a non-zero deduction |

## Edge Cases Handled
- Return blocked if the product isn't return-eligible, not yet delivered, past the 7-day window, or already has an active return request
- Return quantity capped at ordered quantity; partial returns split rows with proportional discount, same as cancellation
- Vendor's damage-cost declaration capped at 50% of refundable order item amount
- Admin's actual deduction capped at the vendor's declared damage cost — admin cannot deduct more than what was flagged
- Refund amount bounded both above (order item cost) and below (zero)
- Inventory released back to available stock at return-shipment-creation time (on approval), not held until inspection completes
- Every stage (request, approve/reject, dispute/accept, refund) independently audit-logged and independently notifies the customer
- Missing customer reference during any notification step handled gracefully (logged, skipped) rather than failing the underlying operation
- `CreateReturnRefund` works correctly whether invoked standalone by an admin or nested inside `AcceptReturnProduct`'s existing transaction