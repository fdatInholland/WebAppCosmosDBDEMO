namespace WebAppCosmosDBDEMO.DTO
{
    public class ProductQueryRequestDTO
    {
        public string SqlQuery { get; set; } = string.Empty;
        public Dictionary<string, object>? Parameters { get; set; }
    }
}
