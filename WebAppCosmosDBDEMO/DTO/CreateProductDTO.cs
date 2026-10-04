namespace WebAppCosmosDBDEMO.DTO
{
    public record CreateProductDto(
        string Category,
        string Name,
        int Quantity,
        decimal Price
    );
}
