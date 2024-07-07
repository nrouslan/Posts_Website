using Microsoft.EntityFrameworkCore;
using Posts_Website.Data;
using Posts_Website.Entities;

namespace Posts_Website.Repositories
{
    public interface IPostRepository
    {
        Post[] Get(int limit, int offset);

        int GetLength();

        Post? GetById(Guid id);

        void Insert(Post post);

        void Update(Post post);

        void Delete(Guid id);

        void Save();
    }

    public class PostRepository(ApplicationContext db) : IPostRepository
    {
        public Post[] Get(int limit, int offset)
        {
            var posts = db.Posts.Skip(offset);

            if (limit > 0)
            {
                posts = posts.Take(limit);
            }

            return [.. posts.Include(p => p.User)
                            .Include(p => p.Comments)];
        }

        public int GetLength()
        {
            return db.Posts.Count();
        }

        public Post? GetById(Guid id)
        {
            return db.Posts.Include(p => p.User)
                           .Include(p => p.Comments)
                                .ThenInclude(c => c.User)
                           .FirstOrDefault(p => p.Id == id);
        }

        public void Insert(Post post)
        {
            db.Posts.Add(post);
        }

        public void Update(Post post)
        {
            var postInDb = db.Posts.Find(post.Id);

            if (postInDb != null)
            {
                postInDb.Content = post.Content;
            }
        }

        public void Delete(Guid id)
        {
            var postInDb = db.Posts.Find(id);

            if (postInDb != null)
            {
                db.Posts.Remove(postInDb);
            }
        }

        public void Save()
        {
            db.SaveChanges();
        }
    }
}