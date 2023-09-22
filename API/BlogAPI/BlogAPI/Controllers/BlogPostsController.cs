using BlogAPI.Models.Domain;
using BlogAPI.Models.DTO;
using BlogAPI.Repository.Implementation;
using BlogAPI.Repository.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace BlogAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BlogPostsController : ControllerBase
    {
        private readonly IBlogPostRepository blogPostRepository;
        private readonly ICategoryRepository categoryRepository;

        public BlogPostsController(IBlogPostRepository blogPostRepository, ICategoryRepository categoryRepository) { 
            this.blogPostRepository = blogPostRepository;
            this.categoryRepository = categoryRepository;
        }

        //POST : https://localhost:7119/api/blogPosts
        [HttpPost]
        [Authorize(Roles = "Writer")]
        public async Task<IActionResult> CreateBlogPost(CreateBlogPostRequestDto request)
        {
            //Map dto to domain
            var blogPost = new BlogPost
            {
                Author = request.Author,
                Title = request.Title,
                Content = request.Content,
                FeaturedImageUrl = request.FeaturedImageUrl,
                PublishedDate = request.PublishedDate,
                ShortDescription = request.ShortDescription,
                IsVisible = request.IsVisible,
                UrlHandle = request.UrlHandle,
                Categories = new List<Category>()
            };

            foreach (var categoryGuid in request.Categories)
            {
                var existingCategory = await categoryRepository.GetById(categoryGuid);
                if(existingCategory is not null)
                {
                    blogPost.Categories.Add(existingCategory);
                }
            }

            blogPost = await blogPostRepository.CreateAsync(blogPost);

            //Map domain model to dto
            var response = new BlogPostDto
            {
                Author = blogPost.Author,
                Title = blogPost.Title,
                Content = blogPost.Content,
                FeaturedImageUrl = blogPost.FeaturedImageUrl,
                PublishedDate = blogPost.PublishedDate,
                ShortDescription = blogPost.ShortDescription,
                IsVisible = blogPost.IsVisible,
                UrlHandle = blogPost.UrlHandle,
                Id = blogPost.Id,
                Categories = blogPost.Categories.Select(c => new CategoryDto
                {
                    Id = c.Id,
                    Name = c.Name,
                    UrlHandle = c.UrlHandle,
                }
                ).ToList()
            };

            return Ok(response);
        }

        //GET : https://localhost:7119/api/blogPosts
        [HttpGet]
        public async Task<IActionResult> GetAllBlogPosts()
        {
            var blogPosts = await blogPostRepository.GetAllAsync();
            var response = new List<BlogPostDto>();

            //Map domain models to Dto
            foreach (var item in blogPosts)
            {
                response.Add(new BlogPostDto
                {
                    Author = item.Author,
                    Title = item.Title,
                    Content = item.Content,
                    FeaturedImageUrl = item.FeaturedImageUrl,
                    PublishedDate = item.PublishedDate,
                    ShortDescription = item.ShortDescription,
                    IsVisible = item.IsVisible,
                    UrlHandle = item.UrlHandle,
                    Id = item.Id,
                    Categories = item.Categories.Select(c => new CategoryDto
                     {
                         Id = c.Id,
                         Name = c.Name,
                         UrlHandle = c.UrlHandle,
                     }).ToList()
                });
            }

            return Ok(response);
        }

        //GET : https://localhost:7119/api/blogPosts/{id}
        [HttpGet]
        [Route("{id:Guid}")]
        public async Task<IActionResult> GetBlogPostById([FromRoute] Guid id)
        {
            //Get the BlogPost from repository
            var blogPost = await blogPostRepository.GetById(id);
            if(blogPost is null)
            {
                return NotFound();
            }

            //Map domain model to dto
            var response = new BlogPostDto
            {
                Id = blogPost.Id,
                Author = blogPost.Author,
                Title = blogPost.Title,
                Content = blogPost.Content,
                FeaturedImageUrl = blogPost.FeaturedImageUrl,
                PublishedDate = blogPost.PublishedDate,
                ShortDescription = blogPost.ShortDescription,
                IsVisible = blogPost.IsVisible,
                UrlHandle = blogPost.UrlHandle,
                Categories = blogPost.Categories.Select(c => new CategoryDto
                {
                    Id = c.Id,
                    Name = c.Name,
                    UrlHandle = c.UrlHandle,
                }).ToList(),
            };

            return Ok(response);
        }

        //GET : https://localhost:7119/api/blogPosts/{urlHandle}
        [HttpGet]
        [Route("{urlHandle}")]
        public async Task<IActionResult> GetBlogPostByUrlHandle([FromRoute] string urlHandle)
        {
            //Get the BlogPost from repository
            var blogPost = await blogPostRepository.GetByUrlHandleAsync(urlHandle);
            if (blogPost is null)
            {
                return NotFound();
            }

            //Map domain model to dto
            var response = new BlogPostDto
            {
                Id = blogPost.Id,
                Author = blogPost.Author,
                Title = blogPost.Title,
                Content = blogPost.Content,
                FeaturedImageUrl = blogPost.FeaturedImageUrl,
                PublishedDate = blogPost.PublishedDate,
                ShortDescription = blogPost.ShortDescription,
                IsVisible = blogPost.IsVisible,
                UrlHandle = blogPost.UrlHandle,
                Categories = blogPost.Categories.Select(c => new CategoryDto
                {
                    Id = c.Id,
                    Name = c.Name,
                    UrlHandle = c.UrlHandle,
                }).ToList(),
            };

            return Ok(response);
        }

        //PUT : https://localhost:7119/api/blogPosts/{id}
        [HttpPut]
        [Route("{id:Guid}")]
        [Authorize(Roles = "Writer")]
        public async Task<IActionResult> UpdateBlogPostById([FromRoute] Guid id, EditBlogPostRequestDto request)
        {
            //Convert dto to domain model
            var blogPost = new BlogPost
            {
                Id = id,
                Title = request.Title,
                Content = request.Content,
                FeaturedImageUrl = request.FeaturedImageUrl,
                PublishedDate = request.PublishedDate,
                ShortDescription = request.ShortDescription,
                IsVisible = request.IsVisible,
                UrlHandle = request.UrlHandle,
                Author = request.Author,
                Categories = new List<Category>()
            };

            foreach (var categoryGuid in request.Categories)
            {
                var existingCategory = await categoryRepository.GetById(categoryGuid);
                if (existingCategory is not null) {
                    blogPost.Categories.Add(existingCategory);
                }
            }


            // Call repository To Update BlogPost Domain Model
            var updatedBlogPost = await blogPostRepository.UpdateAsync(blogPost);

            if(updatedBlogPost is null) {
                return NotFound();
            }

            //Convert domain model to dto
            var response = new BlogPostDto
            {
                Id = blogPost.Id,
                Title = blogPost.Title,
                Content = blogPost.Content,
                Author = blogPost.Author,
                FeaturedImageUrl =blogPost.FeaturedImageUrl,
                PublishedDate = blogPost.PublishedDate,
                ShortDescription = blogPost.ShortDescription,
                IsVisible = blogPost.IsVisible,
                UrlHandle = blogPost.UrlHandle,
                Categories = blogPost.Categories.Select(c => new CategoryDto {
                    Id = c.Id,
                    Name = c.Name,
                    UrlHandle = c.UrlHandle,
                }).ToList(),
            };

          

            return Ok(response);

        }

        //DELETE : https://localhost:7119/api/blogPosts/{id}
        [HttpDelete]
        [Route("{id:Guid}")]
        [Authorize(Roles = "Writer")]
        public async Task<IActionResult> DeleteBlogPostById([FromRoute] Guid id)
        {
            var deletedBlogPost = await blogPostRepository.DeleteAsync(id);
            if(deletedBlogPost is null) {  return NotFound(); }

            //Convert Domain model to dto
            var response = new BlogPostDto
            {
                Id= deletedBlogPost.Id,
                Title = deletedBlogPost.Title,
                Content = deletedBlogPost.Content,
                Author = deletedBlogPost.Author,
                FeaturedImageUrl=deletedBlogPost.FeaturedImageUrl,
                PublishedDate = deletedBlogPost.PublishedDate,
                ShortDescription = deletedBlogPost.ShortDescription,
                IsVisible = deletedBlogPost.IsVisible,
                UrlHandle = deletedBlogPost.UrlHandle,
            };

            return Ok(response);
        }

    }
}
