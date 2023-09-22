using BlogAPI.Data;
using BlogAPI.Models.Domain;
using BlogAPI.Repository.Interface;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;

namespace BlogAPI.Repository.Implementation
{
    public class BlogPostRepository : IBlogPostRepository
    {
        private readonly ApplicationDbContext _dbContext;

        public BlogPostRepository(ApplicationDbContext dbContext)
        {
            this._dbContext = dbContext;
        }
        public async Task<BlogPost> CreateAsync(BlogPost blogPost)
        {
            await _dbContext.BlogPosts.AddAsync(blogPost);
            await _dbContext.SaveChangesAsync();

            return blogPost;
        }

        public async Task<BlogPost?> DeleteAsync(Guid id)
        {
            var existingBlogpost = await _dbContext.BlogPosts.FirstOrDefaultAsync(b => b.Id == id);
            if (existingBlogpost is not null)
            {
                _dbContext.BlogPosts.Remove(existingBlogpost);
                await _dbContext.SaveChangesAsync();
                return existingBlogpost;
            }

            return null;
        }

        public async Task<IEnumerable<BlogPost>> GetAllAsync()
        {
            //Lay ca relationship
            return await _dbContext.BlogPosts.Include(x => x.Categories).ToListAsync();
        }

        public async Task<BlogPost?> GetById(Guid id)
        {
            return await _dbContext.BlogPosts.Include(x => x.Categories).FirstOrDefaultAsync(b => b.Id == id);
        }

        public async Task<BlogPost?> GetByUrlHandleAsync(string urlHandle)
        {
            return await _dbContext.BlogPosts.Include(x => x.Categories).FirstOrDefaultAsync(b => b.UrlHandle == urlHandle);
        }

        public async Task<BlogPost?> UpdateAsync(BlogPost blogPost)
        {
            var existingBlogPost = await _dbContext.BlogPosts.Include(x => x.Categories).FirstOrDefaultAsync(b => b.Id == blogPost.Id);
            if (existingBlogPost is null)
            {
                return null;
            }

            //Update blog
            _dbContext.Entry(existingBlogPost).CurrentValues.SetValues(blogPost);

            //Update categories
            existingBlogPost.Categories = blogPost.Categories;
            await _dbContext.SaveChangesAsync();
            return blogPost;

        }
    }
}
