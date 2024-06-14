using AutoMapper;
using Mango.Services.ShopingCartAPI.Models;
using Mango.Services.ShopingCartAPI.Models.Dto;
using Mango.Services.ShoppingCartAPI.Models;
using Mango.Services.ShoppingCartAPI.Models.Dto;

namespace Mango.Services.ShoppingCartAPI;

public class MappingConfig
{
    public static MapperConfiguration RegisterMaps()
    {
        var mappingConfig = new MapperConfiguration(config =>
        {
            config.CreateMap<CartDetailDto, CartDetail>().ReverseMap();
            config.CreateMap<CartHeaderDto, CartHeader>().ReverseMap();
        });  
        return mappingConfig;
    }
}
