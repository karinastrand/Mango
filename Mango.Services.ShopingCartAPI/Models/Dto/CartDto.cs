﻿namespace Mango.Services.ShopingCartAPI.Models.Dto;

public class CartDto
{
    public CartHeaderDto CartHeader { get; set; }   
    public IEnumerable<CartDetailDto> CartDetails { get; set; }
}
