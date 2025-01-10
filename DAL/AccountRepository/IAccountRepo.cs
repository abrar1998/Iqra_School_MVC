using SchoolProj.Models.AccountDTO;
using SchoolProj.Models.Domain;

namespace SchoolProj.DAL.AccountRepository
{
    public interface IAccountRepo
    {
        Task<User> Login(LoginDTO login);
    }
}
