using Ecommerce.Models;

namespace Ecommerce.Repositories.Interfaces;

public interface IReviewRepsository : IRepository<int,Reviews>
{
        public  Task<List<Reviews>> GetByProductId(int productId);
}