export class ReviewProductRequestModel {
  constructor(
    public productId: number = 0,
    public approvalStatusId: number = 0,
    public remark: string = ""
  ) {}
}