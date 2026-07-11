public class RevenueTrendDto
{
    public string Label { get; set; } = string.Empty;
    public decimal Revenue { get; set; }
}

public class ProductApprovalStatusDto
{
    public string Status { get; set; } = string.Empty;
    public int Count { get; set; }
}

public class ProductSubCategoryDto
{
    public string SubCategory { get; set; } = string.Empty;
    public int Count { get; set; }
}

public class OrderStatusChartDto
{
    public string Status { get; set; } = string.Empty;
    public int Count { get; set; }
}

public class OrdersByMonthDto
{
    public string Month { get; set; } = string.Empty;
    public int Count { get; set; }
}