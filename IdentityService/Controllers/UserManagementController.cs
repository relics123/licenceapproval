using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using IdentityService.Data;
using IdentityService.DTOs;
using IdentityService.Helpers;
using IdentityService.Models;

namespace IdentityService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserManagementController : ControllerBase
    {
        private readonly AppDbContext _context;

        public UserManagementController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet("list/{tenantId}")]
        public async Task<ActionResult<UsersListResponse>> GetUsers(int tenantId)
        {
            try
            {
                var users = await _context.Users
                    .Include(u => u.UserRole)
                    .Where(u => u.TenantId == tenantId)
                    .Select(u => new UserDetail
                    {
                        UserId = u.Id,
                        Username = u.Username,
                        Email = u.Email,
                        FullName = u.FullName,
                        Role = u.UserRole.Name,
                        
                        IsActive = u.IsActive,
                        CreatedDate = u.CreatedDate
                    })
                    .OrderByDescending(u => u.CreatedDate)
                    .ToListAsync();

                return Ok(new UsersListResponse
                {
                    Success = true,
                    Users = users
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new UsersListResponse
                {
                    Success = false,
                    Users = new List<UserDetail>()
                });
            }
        }

        [HttpPost("create")]
        public async Task<ActionResult<UserResponse>> CreateSubUser([FromBody] CreateSubUserRequest request)
        {
            try
            {
                // Validate tenant exists
                var tenant = await _context.Tenants.FindAsync(request.TenantId);
                if (tenant == null)
                {
                    return BadRequest(new UserResponse
                    {
                        Success = false,
                        Message = "Tenant not found"
                    });
                }

                // Check if username already exists in tenant
                var existingUser = await _context.Users
                    .FirstOrDefaultAsync(u => u.Username == request.Username && u.TenantId == request.TenantId);

                if (existingUser != null)
                {
                    return BadRequest(new UserResponse
                    {
                        Success = false,
                        Message = "Username already exists in this tenant"
                    });
                }

                // Create user with TenantUser role (RoleId = 2)
                var passwordHash = PasswordHelper.HashPassword(request.Password);

                var user = new User
                {
                    Username = request.Username,
                    Email = request.Email,
                    PasswordHash = passwordHash,
                    FullName = request.FullName,
                    TenantId = request.TenantId,
                    UserRoleId = 2, // TenantUser
                    IsActive = true,
                    CreatedDate = DateTime.UtcNow
                };

                _context.Users.Add(user);
                await _context.SaveChangesAsync();

                // Load role for response
                user.UserRole = await _context.UserRoles.FindAsync(2);

                return Ok(new UserResponse
                {
                    Success = true,
                    Message = "User created successfully",
                    User = new UserDetail
                    {
                        UserId = user.Id,
                        Username = user.Username,
                        Email = user.Email,
                        FullName = user.FullName,
                        Role = user.UserRole.Name,
                        IsActive = user.IsActive,
                        CreatedDate = user.CreatedDate
                    }
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new UserResponse
                {
                    Success = false,
                    Message = $"Error creating user: {ex.Message}"
                });
            }
        }

        [HttpPut("update")]
        public async Task<ActionResult<UserResponse>> UpdateUser([FromBody] UpdateUserRequest request)
        {
            try
            {
                var user = await _context.Users
                    .Include(u => u.UserRole)
                    .FirstOrDefaultAsync(u => u.Id == request.UserId && u.TenantId == request.TenantId);

                if (user == null)
                {
                    return NotFound(new UserResponse
                    {
                        Success = false,
                        Message = "User not found"
                    });
                }

                // Don't allow changing username to one that already exists
                var existingUser = await _context.Users
                    .FirstOrDefaultAsync(u => u.Username == request.Username && 
                                             u.TenantId == request.TenantId && 
                                             u.Id != request.UserId);

                if (existingUser != null)
                {
                    return BadRequest(new UserResponse
                    {
                        Success = false,
                        Message = "Username already exists"
                    });
                }

                user.Username = request.Username;
                user.Email = request.Email;
                user.FullName = request.FullName;
                user.IsActive = request.IsActive;

                await _context.SaveChangesAsync();

                return Ok(new UserResponse
                {
                    Success = true,
                    Message = "User updated successfully",
                    User = new UserDetail
                    {
                        UserId = user.Id,
                        Username = user.Username,
                        Email = user.Email,
                        FullName = user.FullName,
                        Role = user.UserRole.Name,
                        IsActive = user.IsActive,
                        CreatedDate = user.CreatedDate
                    }
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new UserResponse
                {
                    Success = false,
                    Message = $"Error updating user: {ex.Message}"
                });
            }
        }

        [HttpDelete("delete/{userId}/{tenantId}")]
        public async Task<ActionResult<UserResponse>> DeleteUser(int userId, int tenantId)
        {
            try
            {
                var user = await _context.Users
                    .FirstOrDefaultAsync(u => u.Id == userId && u.TenantId == tenantId);

                if (user == null)
                {
                    return NotFound(new UserResponse
                    {
                        Success = false,
                        Message = "User not found"
                    });
                }

                // Don't allow deleting admin users
                if (user.UserRoleId == 1)
                {
                    return BadRequest(new UserResponse
                    {
                        Success = false,
                        Message = "Cannot delete admin users"
                    });
                }

                _context.Users.Remove(user);
                await _context.SaveChangesAsync();

                return Ok(new UserResponse
                {
                    Success = true,
                    Message = "User deleted successfully"
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new UserResponse
                {
                    Success = false,
                    Message = $"Error deleting user: {ex.Message}"
                });
            }
        }
    }
}