using FrontEnd.Models;

namespace FrontEnd.Services
{
    public interface IUserManagementService
    {
        Task<List<UserItemViewModel>> GetUsersAsync(int tenantId);
        Task<bool> CreateUserAsync(CreateUserViewModel model, int tenantId);
        Task<bool> UpdateUserAsync(EditUserViewModel model, int tenantId);
        Task<bool> DeleteUserAsync(int userId, int tenantId);
    }
}