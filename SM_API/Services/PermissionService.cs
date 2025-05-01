using System;
using Microsoft.EntityFrameworkCore;
using SM_API.Data;

namespace SM_API.Services
{
    public class PermissionService : IPermissionService
    {
        private readonly SupermarketDbContext _context;

        public PermissionService(SupermarketDbContext context)
        {
            _context = context;
        }

        public async Task<bool> UserHasPermissionAsync(Guid userId, string objectName, string permissionName)
        {
            var HasPermission =  await _context.UserRoles
            .Where(ur => ur.UserId == userId)
            .SelectMany(ur => ur.Role.RoleObjectPermissions)
            .AnyAsync(rop =>
                rop.ObjectPermission.Object.Name == objectName &&
                rop.ObjectPermission.Permission.Name == permissionName);

            return HasPermission;
        }

        //public async Task<List<Models.ObjectPermission>> ObjectPermissionsAsync(Guid objectId)
        //{
        //    //var objectPermissions = await _context.ObjectPermissions
        //    //    .GroupBy(op => op.Object.Name)
        //    //    .Select(g => new
        //    //    {
        //    //        ObjectName = g.Key,
        //    //        Permissions = g.Select(op => op.Permission.Name).ToList()
        //    //    }).ToListAsync();
            
        //    var objectPermissions = await _context.ObjectPermissions
        //        .Where(ob=>ob.ObjectId == objectId)
        //        .Select(g => new
        //        {
        //            ObjectName = g.Object.Name,
        //            Permissions = g.Permission.Name
        //        }).ToListAsync();

        //    return objectPermissions;

        //}
        
        //public async Task<List<Models.ObjectPermission>> UserPermissionsAsync(Guid userId, string objectName, string permissionTypeName)
        //{
        //    //var Userpermissions = await _context.UserRoles
        //    //    .Where(ur => ur.UserId == user.Id)
        //    //    .SelectMany(ur => ur.Role.RoleObjectPermissions)
        //    //    .Select(rop => new
        //    //    {
        //    //        ObjectName = rop.ObjectPermission.Object.Name,
        //    //        PermissionName = rop.ObjectPermission.Permission.Name
        //    //    })
        //    //    .Distinct()
        //    //    .ToListAsync();

        //    //return Userpermissions;
        //}
    }
}





