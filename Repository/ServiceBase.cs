using DataAccess.Models;

namespace Repository;
public class ServiceBase<T> : GenericRepository<T> where T : class
{
    public ServiceBase(PetShop2023DbContext context) : base(context)
    {
    }
}
