namespace Cards.Client.Models
{
    public class CardParameters : RequestParameters
    {
        private string _status = null!;
        public CardParameters()
        {
            OrderBy = "Name";
        }
        public string Name { get; set; } = "all";
        public string Color { get; set; } = "all";
        public string StatusId { get; set; } = "all";
        public DateTime? DateOfCreation { get; set; } = null;

        public string? Status
        {
            get { return _status; }
            set
            {
                if (value != null)
                    _status = value.Trim();
                else
                    _status = null!;

                if (_status != null)
                {
                    StringComparison stringComparison = StringComparison.InvariantCultureIgnoreCase;

                    if (_status.Equals("ToDo", stringComparison))
                    {
                        StatusId = "Todo343d-f8ec-4197-b0b2-f3365f71f2e2";
                    }
                    else if (_status.Equals("In Progress", stringComparison))
                    {
                        StatusId = "InProgress643-4e2e-bba7-8ebebb32d606";
                    }
                    else if (_status.Equals("Done", stringComparison))
                    {
                        StatusId = "Done83ea-b4c1-4107-a66b-da86fcecf73f";
                    }
                }
            }
        }
    }
}
