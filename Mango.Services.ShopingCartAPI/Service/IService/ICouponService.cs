using Mango.Service.ShopingCartAPI.Models.Dto;
using Mango.Services.ShopingCartAPI.Models.Dto;

namespace Mango.Services.ShopingCartAPI.Service.IService;

public interface ICouponService
{
    public Task<CouponDto> GetCoupon(string couponCode);
}
