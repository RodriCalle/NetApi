using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using DefaultProject.Dtos.Product;
using DefaultProject.Models;
using DefaultProject.Persistence.Repositories.UnitOfWork;
using Microsoft.AspNetCore.Mvc;

namespace DefaultProject.Controllers
{
    [ApiController]
    [Route("/api/[controller]")]
    [Produces("application/json")]
    public class ProductsController : ControllerBase
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly IMapper _mapper;

        public ProductsController(IUnitOfWork unitOfWork, IMapper mapper)
        {
            this.unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<ProductResponseDto>>> GetAll()
        {
            var products = await unitOfWork.ProductRepository.GetAllProducts();
            var resources = _mapper
                .Map<IEnumerable<Product>, IEnumerable<ProductResponseDto>>(products);
            return Ok(resources);
        }

    }
}
