using Mango.Web.Models;

namespace Mango.Web.Service.IService;

public interface IProductService
{
    Task<ResponseDto?> GetAllProductsAsync();
    Task<ResponseDto?> GetProductsByCategoryAsync(string productCategory);
    Task<ResponseDto?> GetProductByIdAsync(int id);
    Task<ResponseDto?> GetProductAsync(string name);
    Task<ResponseDto?> CreateProductsAsync(ProductDto productDto);
    Task<ResponseDto?> UpdateProductsAsync(ProductDto productDto);
    Task<ResponseDto?> DeleteProductsAsync(int id);
}
