export class AdminUserFilter {
  constructor(
    public adminRoleId: number | null = null,
    public status: boolean | null = null,
    public email : string = '',
    public phoneNumber : string ='',
    public pageNumber: number = 1,
    public pageSize: number = 10
  ) {}
}