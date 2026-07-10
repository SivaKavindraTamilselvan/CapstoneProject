export class VendorInventoryModel {
  constructor(
    public inventoryId: number = 0,
    public addressId: number = 0,
    public productVariantId: number = 0,
    public sku: string = '',
    public availableQuantity: number = 0,
    public reservedQuantity: number = 0,
    public contactPhoneNumber: string = '',
    public addressLine: string = '',
    public city: string = '',
    public state: string = '',
    public country: string = '',
    public pinCode: string = '',
    public isActive: boolean = true
  ) {}
}