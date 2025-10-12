using ElectroKart_Api.DTOs.Wishlist;

namespace ElectroKart_Api.Services.Wishlist
{
    public interface IWishlistService
    {

        Task AddProductToWishlistAsync(int userId, int productId);
    }
}