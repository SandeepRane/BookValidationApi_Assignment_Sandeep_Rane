namespace BookValidationApi_Assignment_Sandeep_Rane.Models
{

    public class BookValidationResult
    {
        public List<Book> ValidBooks { get; set; } = new();
        public List<InvalidBookEntry> InvalidBooks { get; set; } = new();
    }

    public class InvalidBookEntry
    {
        public string Title { get; set; } = string.Empty;
        public string Reason { get; set; } = string.Empty;
    }
}
