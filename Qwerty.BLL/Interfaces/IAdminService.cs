using Qwerty.BLL.Infrastructure;
using System.Threading.Tasks;

namespace Qwerty.BLL.Interfaces
{
    public interface IAdminService
    {
        Task<OperationDetails> BlockUserAsync(string UserId);
        Task<OperationDetails> UnblockUserAsync(string UserId);

    }
}
