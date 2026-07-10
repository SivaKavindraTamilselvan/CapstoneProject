export class ShipmentServiceModel {
    constructor(
        public pickupPostcode: string = '',
        public deliveryPostcode: string = '',
        public weight: number = 0,
        public cod : number = 0,
    ) { }
}