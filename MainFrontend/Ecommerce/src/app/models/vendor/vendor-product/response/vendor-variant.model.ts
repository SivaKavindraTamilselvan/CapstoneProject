import { ProductImageModel } from "../../../product/product-image.model";
import { VendorProductAttributeModel } from "./vendor-attribute.model";

export class VendorProductVariantModel {
  constructor(
    public productVariantId: number = 0,
    public sku: string = '',

    public productName: string = '',
    public description: string = '',
    public productCategoryName: string = '',
    public productSubCategoryName: string = '',
    public mainProductSubCategoryAttributeName: string = '',
    public productStatus: string = '',
    public productApprovalStatus: string = '',

    public price: number = 0,
    public weightInKgs: number = 0,
    public lengthInCm: number = 0,
    public widthInCm: number = 0,
    public heightInCm: number = 0,

    public minimuQuantityPerUser: number = 0,

    public addedByVendorUserId: number = 0,
    public addedByVendorUser: string = '',

    public productVariantStatus: string = '',
    public productVariantApprovalStatus: string = '',

    public availableQuantity: number = 0,
    public reservedQuantity: number = 0,

    public isAvailableForSale: boolean = false,

    public isReturn: boolean = true,
    public isExchange: boolean = true,

    public createdAt: Date = new Date(),
    public updatedAt: Date | null = null,

    public validationIssues: string[] = [],

    public attributes: VendorProductAttributeModel[] = [],
    public productImages: ProductImageModel[] = []
  ) { }
}