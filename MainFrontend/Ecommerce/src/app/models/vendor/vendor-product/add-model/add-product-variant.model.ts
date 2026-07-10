import { AddProductVariantAttributeModel } from "./add-variant-attribute.model";

export class AddProductVariantModel {
  constructor(
    public productId: number = 0,
    public price: number = 0,
    public weightInKgs: number = 0,
    public lengthInCm: number = 0,
    public widthInCm: number = 0,
    public heightInCm: number = 0,
    public minimuQuantityPerUser: number = 1,
    public isReturn: boolean = true,
    public isExchange: boolean = true,
    public productVariantAttribute : AddProductVariantAttributeModel[] = []
  ) {}
}