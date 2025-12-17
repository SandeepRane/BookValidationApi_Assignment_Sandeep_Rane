namespace BookValidationApi_Assignment_Sandeep_Rane.Models
{
    public class Book
    {
        public string Title { get; set; } = string.Empty;
        public string Author { get; set; } = string.Empty;
        public string Genre { get; set; } = string.Empty;
        public int Year { get; set; }
        public string Publisher { get; set; } = string.Empty;
    }
}
