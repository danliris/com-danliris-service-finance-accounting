namespace Com.Danliris.Service.Finance.Accounting.Lib.Services.HttpClientService
{
    public class APIDefaultResponse<T>
    {
        public T data { get; set; }
    }

    /// <summary>
    /// You can add any property. Just don't remove!
    /// </summary>
    public class AccountBank
    {
        public int Id { get; set; }
        public string AccountCOA { get; set; }
    }
}