namespace ElectroKart_Api.Services.CartServices
{
    public interface ICartService
    {
        Task AddToCartAsync(int userId, int productId, int quantity);
    }
}