export class AdminUserFilter {
  constructor(
    public adminRoleId: number | null = null,
    public isActive: boolean | null = null,
    public pageNumber: number = 1,
    public pageSize: number = 10
  ) {}
}