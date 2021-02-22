using System.Collections.Generic;
using System.Threading.Tasks;

namespace FieldLevel.Controllers
{
    public interface IPostService
    {
        Task<IEnumerable<Post>> GetPosts();
        Task<IEnumerable<Post>> GetUsersLastPost();
    }
}