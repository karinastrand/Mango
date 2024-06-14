using AutoMapper;
using Mango.Service.ProductAPI.Models;
using Mango.Service.ProductAPI.Models.Dto;
using Mango.Service.ProductAPI.Models.ModelDto;
using Mango.Services.ProductAPI.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Mango.Service.ProductAPI.Controllers;

[Route("api/product")]
[ApiController]

public class ProductAPIController : ControllerBase
{
    private readonly AppDbContext _db;
    private IMapper _mapper;
    private ResponseDto _response;
    
    public ProductAPIController(AppDbContext db, IMapper mapper)
    {
        _db=db;
        _mapper=mapper;
        _response=new ResponseDto();
    }

    [HttpGet]
    public ResponseDto Get()
    {
        try
        {
            IEnumerable<Product> productList= _db.Products.ToList();
            _response.Result=_mapper.Map<IEnumerable<ProductDto>>(productList);

        }
        catch (Exception ex)
        {
            _response.IsSuccess=false;
            _response.Message=ex.Message;
        }
        return _response;
    }
    [HttpGet]
    [Route("GetByCategory/categoryName")]
    public ResponseDto GetByCategory(string categoryName)
    {
        try
        {
            IEnumerable<Product> productList = _db.Products.Where(u=>u.CategoryName.ToLower()==categoryName.ToLower()).ToList();
            _response.Result = _mapper.Map<IEnumerable<ProductDto>>(productList);

        }
        catch (Exception ex)
        {
            _response.IsSuccess = false;
            _response.Message = ex.Message;
        }
        return _response;
    }
    [HttpGet]
    [Route("{id:int}")]
    public ResponseDto Get(int id) 
    {
        try
        {
            Product product = _db.Products.First(u => u.ProductId == id);
            _response.Result=_mapper.Map<ProductDto>(product);
        }
        catch (Exception ex)
        {

            _response.IsSuccess = false;
            _response.Message = ex.Message;
        }
    

        return _response;
    }
    [HttpGet]
    [Route("GetByName/{name}")]
    public ResponseDto GetByName(string name)
    {
        try
        {
            Product product = _db.Products.FirstOrDefault(u => u.Name.ToLower() == name.ToLower());
            if (product == null)
            {
                _response.IsSuccess=false;
            }
            _response.Result = _mapper.Map<ProductDto>(product);
        }
        catch (Exception ex)
        {

            _response.IsSuccess = false;
            _response.Message = ex.Message;
        }


        return _response;
    }

    [HttpPost]
    [Authorize(Roles ="ADMIN")]
    public ResponseDto Post([FromBody] ProductDto productDto) 
    {
        try
        {
            Product product=_mapper.Map<Product>(productDto);
            _db.Products.Add(product);
            _db.SaveChanges();
            _response.Result = _mapper.Map<ProductDto>(product);

        }
        catch (Exception ex)
        {

            _response.IsSuccess = false;
            _response.Message = ex.Message;
        }
        return _response;
    }

    [HttpPut]
    [Authorize(Roles = "ADMIN")]
    public ResponseDto Put([FromBody] ProductDto productDto)
    {
        try
        {
            Product product = _mapper.Map<Product>(productDto);
            _db.Products.Update(product);
            _db.SaveChanges();
            _response.Result = _mapper.Map<ProductDto>(product);

        }
        catch (Exception ex)
        {

            _response.IsSuccess = false;
            _response.Message = ex.Message;
        }
        return _response;
    }
    [HttpDelete]
    [Authorize(Roles = "ADMIN")]
    [Route("{id:int}")]
    public ResponseDto Delete(int id)
    {
        try
        {
            Product product = _db.Products.First(u => u.ProductId == id);
            _db.Remove(product);
            _db.SaveChanges();

        }
        catch (Exception ex)
        {

            _response.IsSuccess = false;
            _response.Message = ex.Message;
        }
        return _response;
    }
}
