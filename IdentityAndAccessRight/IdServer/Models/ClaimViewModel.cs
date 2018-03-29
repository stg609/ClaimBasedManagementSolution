namespace IdServer.Models
{
    public class ClaimViewModel
    {
        public int Key { get; set; }
        public string Value { get; set; }
        public string OwnerIdentity { get; set; }
        public string OwnerIp { get; set; }
    }
}
