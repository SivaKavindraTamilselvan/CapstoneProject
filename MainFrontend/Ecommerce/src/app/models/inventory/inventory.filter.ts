export class VendorInventoryFilterModel {
  constructor(
    public productVariantId: number | null = null,
    public addressId: number | null = null,
    public minimumAvailableQuantity: number | null = null,
    public minimumReservedQuantity: number | null = null,
    public maximumAvailableQuantity: number | null = null,
    public maximumReservedQuantity: number | null = null,
    public status: boolean | null = null,
    public pageNumber: number = 1,
    public pageSize: number = 10
  ) {}
}