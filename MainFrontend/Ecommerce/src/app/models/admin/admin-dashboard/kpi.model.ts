export class AdminDashboardKPI {
    constructor(
        public grossSales: number | null = null,
        public commission: number | null = null,
        public yesterdayCommission: number | null = null,
        public last30DaysCommission: number | null = null,
        public totalOrders: number | null = null,
        public pendingOrders: number | null = null,
        public totalCustomers: number | null = null,
        public activeVendors: number | null = null,
    ) { }
}

export class RevenueTrend {
  constructor(
    public label: string,
    public revenue: number
  ) {}
}

export class ProductApprovalStatus {
  constructor(
    public status: string = '',
    public count: number = 0
  ) {}
}

export class ProductSubCategory {
  constructor(
    public subCategory: string = '',
    public count: number = 0
  ) {}
}

export class OrderStatusChart {
  constructor(
    public status: string = '',
    public count: number = 0
  ) {}
}

export class OrdersByMonth {
  constructor(
    public month: string = '',
    public count: number = 0
  ) {}
}