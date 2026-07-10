export class UpdateInventoryModel {
    constructor(
        public inventoryId: number = 0,
        public availableQuantity: number = 0,
        public UpdateType : boolean = true,
    ) { }
}