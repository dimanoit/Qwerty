using System.Threading.Tasks;

namespace Qwerty.BLL.Interfaces
{
    public interface IAdminService
    {
        Task BlockUserAsync(string userId);
        Task UnblockUserAsync(string userId);

    }
}
