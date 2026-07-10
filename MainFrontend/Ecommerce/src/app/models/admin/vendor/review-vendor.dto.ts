export class ReviewVendorRequestModel {
  constructor(
    public vendorId: number = 0,
    public approvalStatusId: number = 0,
    public remark: string = ""
  ) {}
}