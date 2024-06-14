using Mango.Service.ShopingCartAPI.Models.Dto;

namespace Mango.Services.ShopingCartAPI.Service.IService;

public interface IProductService
{
    public Task<IEnumerable<ProductDto>> GetProducts();
}
