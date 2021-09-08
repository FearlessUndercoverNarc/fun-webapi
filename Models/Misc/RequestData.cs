namespace Models.Misc
{
    public class RequestData
    {
        public string LastRequest { get; set; }

        public long Amount { get; set; }

        public RequestData()
        {
        }

        public RequestData(string lastRequest, long amount)
        {
            LastRequest = lastRequest;
            Amount = amount;
        }
    }
}