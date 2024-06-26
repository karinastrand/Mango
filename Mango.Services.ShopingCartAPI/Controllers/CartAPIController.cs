﻿using AutoMapper;
using Mango.Service.ShopingCartAPI.Models.Dto;
using Mango.Services.ShopingCartAPI.Models;
using Mango.Services.ShopingCartAPI.Models.Dto;
using Mango.Services.ShopingCartAPI.Service.IService;
using Mango.Services.ShoppingCartAPI.Data;
using Mango.Services.ShoppingCartAPI.Models.Dto;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Reflection.PortableExecutable;

namespace Mango.Services.CartAPI.Controllers;

[Route("api/cart")]
[ApiController]
public class CartAPIController : ControllerBase
{
    private readonly AppDbContext _db;
    private  IMapper _mapper;
    private readonly ResponseDto _response;
    private IProductService _productService;
    private ICouponService _couponService;
    public CartAPIController(AppDbContext db, IMapper mapper,IProductService productService, ICouponService couponService)
    {
        _db = db;
        _mapper = mapper;
        _productService = productService;
        this._response = new ResponseDto();
        _couponService = couponService; 
    }
    [HttpGet("GetCart/{userId}")]
    public async Task<ResponseDto> GetCart(string userId)
    {
        try
        {
            CartDto cart = new()
            {
                CartHeader = _mapper.Map<CartHeaderDto>(_db.CartHeaders.First(u => u.UserId == userId))

            };
            cart.CartDetails = _mapper.Map<IEnumerable<CartDetailDto>>(_db.CartDetails
                .Where(u=>u.CartHeaderId==cart.CartHeader.CartHeaderId));
            IEnumerable<ProductDto> productList = await _productService.GetProducts();
            foreach (var item in cart.CartDetails) 
            {
                item.Product = productList.FirstOrDefault(u => u.ProductId == item.ProductId);
                cart.CartHeader.CartTotal += (item.Count * item.Product.Price);
            }

            if (!string.IsNullOrEmpty(cart.CartHeader.CouponCode)) 
            {
                CouponDto couponDto=await _couponService.GetCoupon(cart.CartHeader.CouponCode);
                if (couponDto != null && cart.CartHeader.CartTotal > couponDto.MinAmount) 
                {
                    cart.CartHeader.CartTotal -= couponDto.DiscountAmount;
                    cart.CartHeader.Discount=couponDto.DiscountAmount;
                }
            }

            _response.Result = cart;
        }
        catch (Exception ex)
        {

            _response.Message = ex.Message.ToString();
            _response.IsSuccess = false;
        }
        return _response;
    }

    [HttpPost("ApplyCoupon")]
    public async Task<ResponseDto> ApplyCoupon([FromBody] CartDto cartDto)
    {
        try
        {
            var cartFromDb=await _db.CartHeaders.FirstAsync(u=>u.UserId==cartDto.CartHeader.UserId);
            cartFromDb.CouponCode=cartDto.CartHeader.CouponCode;
            _db.CartHeaders.Update(cartFromDb);
            await _db.SaveChangesAsync();
            _response.Result = true;
        }
        catch (Exception ex)
        {

            _response.Message = ex.Message.ToString();
            _response.IsSuccess = false;
        }
        return _response;
    }
   

    [HttpPost("CartUpsert")]
    public async Task<ResponseDto> CartUpsert(CartDto cartDto)
    {
        try
        {
            var cartHeaderFromDb = await _db.CartHeaders.AsNoTracking().FirstOrDefaultAsync(u=>u.UserId==cartDto.CartHeader.UserId);
            if (cartHeaderFromDb == null)
            { 

                CartHeader cartHeader = _mapper.Map<CartHeader>(cartDto.CartHeader);
                _db.CartHeaders.Add(cartHeader);
                await _db.SaveChangesAsync();
                cartDto.CartDetails.First().CartHeaderId=cartHeader.CartHeaderId;
                _db.CartDetails.Add(_mapper.Map<CartDetail>(cartDto.CartDetails.First()));
                await _db.SaveChangesAsync();
            }
            else
            {
                var cartDetailsFromDb=await _db.CartDetails.AsNoTracking().FirstOrDefaultAsync(u=>u.ProductId==cartDto.CartDetails.First().ProductId
                && u.CartHeaderId==cartHeaderFromDb.CartHeaderId);
                if (cartDetailsFromDb == null)
                {
                    cartDto.CartDetails.First().CartHeaderId = cartHeaderFromDb.CartHeaderId;
                    _db.CartDetails.Add(_mapper.Map<CartDetail>(cartDto.CartDetails.First()));
                    await _db.SaveChangesAsync();
                }
                else
                {
                    cartDto.CartDetails.First().Count += cartDetailsFromDb.Count;
                    cartDto.CartDetails.First().CartHeaderId = cartDetailsFromDb.CartHeaderId;
                    cartDto.CartDetails.First().CartDetailsId = cartDetailsFromDb.CartDetailsId;
                    _db.CartDetails.Update(_mapper.Map<CartDetail>(cartDto.CartDetails.First()));
                    await _db.SaveChangesAsync();
                }
            }
            _response.Result=cartDto;
        }
        catch (Exception ex)
        {

            _response.Message = ex.Message.ToString();
            _response.IsSuccess = false;
        }
        return _response;
    }

    [HttpPost("RemoveCart")]
    public async Task<ResponseDto> RemoveCart([FromBody] int cartDetailId)
    {
        try
        {
            CartDetail cartDetail = _db.CartDetails.First(u=>u.CartDetailsId==cartDetailId);
            int totalCountofCartItem=_db.CartDetails.Where(u=>u.CartHeaderId==cartDetail.CartHeaderId).Count();
            _db.CartDetails.Remove(cartDetail);
            if (totalCountofCartItem ==1) 
            {
                var cartHeaderToRemove=await _db.CartHeaders.FirstOrDefaultAsync(u=>u.CartHeaderId == cartDetail.CartHeaderId);
                _db.CartHeaders.Remove(cartHeaderToRemove); 
            }

            await _db.SaveChangesAsync();

           
            _response.Result = true;
        }
        catch (Exception ex)
        {

            _response.Message = ex.Message.ToString();
            _response.IsSuccess = false;
        }
        return _response;
    }
}
