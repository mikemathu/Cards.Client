namespace Cards.Client.Models
{
    public class Card
    {
        public int PageCount { get; set; } = 10;
         
        private string? _color;
        public string CardId { get; set; } = Guid.NewGuid().ToString();
        public string Name { get; set; } = null!;
        public string? Description { get; set; }
        public DateTime DateOfCreation { get; set; } = DateTime.UtcNow;
        public string StatusId { get; set; } = "Todo343d-f8ec-4197-b0b2-f3365f71f2e2";
        //public Status Status { get; set; } = null!;
        public string Status { get; set; } = null!;
        public string AppUserId { get; set; } = null!;
        public AppUser AppUser { get; set; } = null!;
        public string? Color
        {
            get { return _color; }
            set
            {
                if (string.IsNullOrEmpty(value))
                {
                    _color = null;
                    return;
                }

                const string prefixValue = "#";

                if (value.StartsWith(prefixValue))
                {
                    if (value.Length == 7)
                    {
                        _color = value;
                    }
                    else
                    {
                        throw new ArgumentException("Six alphanumeric characters are required for the color code.");
                    }
                }
                else
                {
                    throw new ArgumentException("The color code should start with '#'.");
                }
            }
        }
    }
}