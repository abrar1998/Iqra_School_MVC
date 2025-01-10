using SchoolProj.Models.AccountDTO;
using SchoolProj.Models.DTO;

namespace SchoolProj.DAL.UserRepository
{
    public interface IUserRepo
    {
        Task<Response> PasswordChangeAsync(PasswordChangeDTO passwordObject);
    }
}
