using FabricDemo.IdentityServer.Domain;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace FabricDemo.IdentityServer.EntityFrameworkCore
{
    /// <summary>
    /// 数据库访问上下文
    /// </summary>
    public class FabricDemoDbContext : IdentityDbContext<ApplicationUser>
    {
        /// <inheritdoc />
        public FabricDemoDbContext(DbContextOptions<FabricDemoDbContext> options)
            : base(options)
        {

        }
    }
}
