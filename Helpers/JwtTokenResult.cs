namespace InvoiceApi.Helpers
{
    public class JwtTokenResult
    {
        public string Token { get; set; }
        public DateTime ExpiresAt { get; set; }
    }
}