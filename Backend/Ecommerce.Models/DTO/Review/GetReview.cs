public class ReviewItemDTO
{
    public int ReviewId { get; set; }
    public int StarId { get; set; }
    public string ReviewDescription { get; set; } = string.Empty;
    public string? AdditionalReviewDescription { get; set; }
    public string? UserName { get; set; }
    public DateTime CreatedAt { get; set; }
}

public class ProductReviewSummaryDTO
{
    public int ProductId { get; set; }
    public double AverageRating { get; set; }
    public int TotalReviews { get; set; }
    public Dictionary<int, int> StarBreakdown { get; set; } = new(); // {5:12, 4:5, 3:1, 2:0, 1:0}
    public List<ReviewItemDTO> Reviews { get; set; } = new();
}