namespace MovieAPI.Models
{
    public class metadata
    {
        public int Id { get; set; }
        public int movieId { get; set; }
        public string title { get; set; }
        public string language { get; set; }
        public string duration { get; set; }
        public int releaseYear { get; set; }
    }
}
