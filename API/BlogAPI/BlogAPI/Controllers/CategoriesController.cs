using BlogAPI.Data;
using BlogAPI.Models.Domain;
using BlogAPI.Models.DTO;
using BlogAPI.Repository.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace BlogAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CategoriesController : ControllerBase
    {
        private readonly ICategoryRepository categoryRepository;

        public CategoriesController(ICategoryRepository categoryRepository)
        {
            this.categoryRepository = categoryRepository;
        }

        //POST : https://localhost:7119/api/categories 
        [HttpPost]
        [Authorize(Roles = "Writer")]
        public async Task<IActionResult> CreateCategory([FromBody] CreateCategoryRequestDto request)
        {
            //Map DTO to Domain Model
            var category = new Category
            {
                Name = request.Name,
                UrlHandle = request.UrlHandle,
            };


            category = await this.categoryRepository.CreateAsync(category);

            //Map domain model to DTO
            var response = new CategoryDto
            {
                Id = category.Id,
                Name = category.Name,
                UrlHandle = category.UrlHandle,
            };
            return Ok(response);
        }

        //GET : https://localhost:7119/api/categories 

        [HttpGet]
        public async Task<IActionResult> GetAllAsync()
        {
           var categories = await this.categoryRepository.GetAllAsync();
            var rs = new List<CategoryDto>();

            //Map models to DTO

            foreach (var item in categories)
            {

                rs.Add(new CategoryDto
                {
                    Name = item.Name,
                    UrlHandle = item.UrlHandle,
                    Id = item.Id
                });
            }

            return Ok(rs);
        }

        //GET : https://localhost:7119/api/categories/{id} 
        [HttpGet]
        [Route("{id:Guid}")]
        public async Task<IActionResult> GetCategoryById([FromRoute] Guid id)
        {
            var existingCategory = await this.categoryRepository.GetById(id);
            if(existingCategory is null)
            {
                return NotFound();
            }

            //Map models to DTO
            var response = new CategoryDto
            {
               Id=existingCategory.Id,
               Name = existingCategory.Name,
               UrlHandle = existingCategory.UrlHandle,
            };
            return Ok(response);
        }

        //PUT :  https://localhost:7119/api/categories/{id} 
        [HttpPut]
        [Route("{id:Guid}")]
        [Authorize(Roles = "Writer")]
        public async Task<IActionResult> EditCategory( [FromRoute] Guid id, EditCategoryRequestDto request)
        {
            var category = new Category
            {
                Id = id,
                Name = request.Name,
                UrlHandle = request.UrlHandle,
            };
            category = await this.categoryRepository.UpdateAsync(category);

            if(category is null)
            {
                return NotFound();
            }

            //Map domain model to dto

            var response = new CategoryDto
            {
                Id = category.Id,
                Name = category.Name,
                UrlHandle = category.UrlHandle,
            };

            return Ok(response);
        }


        //DELETE :  https://localhost:7119/api/categories/{id} 
        [HttpDelete]
        [Route("{id:Guid}")]
        [Authorize(Roles = "Writer")]
        public async Task<IActionResult> DeleteCategory([FromRoute] Guid id)
        {
            var category = await this.categoryRepository.DeleteAsync(id);

            if (category is null)
            {
                return NotFound();
            }

            //Map domain model to DTO
            var response = new CategoryDto { 
                Id = category.Id,
                Name = category.Name,
                UrlHandle = category.UrlHandle,
            };

            return Ok(response);
        }
    }
}
