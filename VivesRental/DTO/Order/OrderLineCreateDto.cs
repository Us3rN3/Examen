public class OrderLineCreateDto
{
    public Guid ArticleId { get; set; }
    public string ArticleName { get; set; } = null!;
    public string ProductName { get; set; } = null!;
    public DateTime RentalStartDate { get; set; }
}
