using Mango.Web.Models;
using Mango.Web.Service.IService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.IdentityModel.Tokens.Jwt;

namespace Mango.Web.Controllers;

public class CartController : Controller
{
    public readonly ICartService _cartService;
    public CartController(ICartService cartService)
    {
        _cartService = cartService; 
    }

    public async Task<IActionResult> Remove(int cartDetailsId)
    {
        var userId=User.Claims.Where(u=>u.Type==JwtRegisteredClaimNames.Sub)?.FirstOrDefault()?.Value;
        ResponseDto? response=await _cartService.RemoveFromCartAsync(cartDetailsId);
        if (Response != null && response.IsSuccess) 
        {
            TempData["Success"] = "Cart updated successfully";
            return RedirectToAction(nameof(CartIndex));
        }
        return View();
    }
    [HttpPost]
    public async Task<IActionResult> ApplyCoupon(CartDto cartDto)
    {
      
        if (cartDto.CartDetails == null)
        {
            IEnumerable<CartDetailDto>? CartDetails=new List<CartDetailDto>();
            cartDto.CartDetails = CartDetails;
        }
        ResponseDto? response = await _cartService.ApplyCouponAsync(cartDto);
        if (response != null && response.IsSuccess)
        {
            TempData["Success"] = "Cart updated successfully";
            return RedirectToAction(nameof(CartIndex));
        }
        return View();
    }
    [HttpPost]
    public async Task<IActionResult> RemoveCoupon(CartDto cartDto)
    {
        if (cartDto.CartDetails == null)
        {
            IEnumerable<CartDetailDto>? CartDetails = new List<CartDetailDto>();
            cartDto.CartDetails = CartDetails;
        }
        cartDto.CartHeader.CouponCode = "";
        ResponseDto? response = await _cartService.ApplyCouponAsync(cartDto);
        if (response != null && response.IsSuccess)
        {
            TempData["Success"] = "Cart updated successfully";
            return RedirectToAction(nameof(CartIndex));
        }
        return View();
    }

    [Authorize]
    public async Task<IActionResult> CartIndex()
    {
        return View(await LoadCartDtoBasedOnLoggedInUser());
    }
    private async Task<CartDto> LoadCartDtoBasedOnLoggedInUser()
    {
        var userId = User.Claims.Where(u => u.Type == JwtRegisteredClaimNames.Sub)?.FirstOrDefault()?.Value;
        ResponseDto response=await _cartService.GetCartByUserIdAsync(userId);
        if(response!=null & response.IsSuccess)
        {
            CartDto cartDto = JsonConvert.DeserializeObject<CartDto>(Convert.ToString(response.Result));
            return cartDto;
        }
        return new CartDto();
    }

}
