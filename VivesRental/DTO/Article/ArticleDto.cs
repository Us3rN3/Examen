namespace VivesRental.DTO.Article;

public class ArticleDto
{
    public Guid Id { get; set; }
    public string ProductName { get; set; } = string.Empty;
    public int Status { get; set; }
}
