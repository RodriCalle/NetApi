using System;
using AutoMapper;
using DefaultProject.Dtos.Product;
using DefaultProject.Models;

namespace DefaultProject.Mapping
{
    public class ProfileConfiguration : Profile
    {
        public ProfileConfiguration()
        {
            CreateMap<Product, ProductResponseDto>();
            CreateMap<ProductRequestDto, Product>();
        }
    }
}
