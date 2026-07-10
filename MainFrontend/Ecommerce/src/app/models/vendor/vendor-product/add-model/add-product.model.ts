export class AddProductModel {
  constructor(
    public productName: string = "",
    public description: string = "",
    public productSubCategoryId: number = 0,
    public mainProductSubCategoryAttributeId: number = 0
  ) {}
}