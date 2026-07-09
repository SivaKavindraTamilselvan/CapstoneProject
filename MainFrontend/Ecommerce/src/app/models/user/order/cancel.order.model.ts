export class CancelOrderModel {
    constructor(
        public cancelReasonId: number = 0,
        public orderItemId: number = 0,
        public cancelStatusId: number = 1,
        public additionalReason: string = '',
        public cancelQuantity: number = 0,
    ) { }
}