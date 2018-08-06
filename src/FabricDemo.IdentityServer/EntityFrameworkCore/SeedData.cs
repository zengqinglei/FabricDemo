using FabricDemo.IdentityServer.Domain;
using IdentityModel;
using IdentityServer4.EntityFramework.DbContexts;
using IdentityServer4.EntityFramework.Mappers;
using IdentityServer4.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace FabricDemo.IdentityServer.EntityFrameworkCore
{
    /// <summary>
    /// 初始化数据
    /// </summary>
    public class SeedData
    {
        /// <summary>
        /// 资源集合
        /// </summary>
        public static IEnumerable<IdentityResource> GetIdentityResources()
        {
            return new List<IdentityResource>
            {
                new IdentityResources.OpenId(),
                new IdentityResources.Profile(),
            };
        }

        /// <summary>
        /// api 资源集合
        /// </summary>
        public static IEnumerable<ApiResource> GetApiResources()
        {
            return new List<ApiResource>
            {
                new ApiResource("default-api", "基础服务 API"),
                new ApiResource("userservice-api", "用户服务 API"),
                new ApiResource("productservice-api", "产品服务 API")
            };
        }

        /// <summary>
        /// clients want to access resources (aka scopes)
        /// </summary>
        public static IEnumerable<Client> GetClients()
        {
            return new List<Client>
            {
                new Client
                {
                    ClientId = "UserService",
                    AllowedGrantTypes = GrantTypes.ClientCredentials,
                    ClientSecrets =
                    {
                        new Secret("UserService".Sha256())
                    },
                    AllowedScopes = { "userservice-api" }
                },
                new Client
                {
                    ClientId = "ProductService",
                    AllowedGrantTypes = GrantTypes.ClientCredentials,
                    ClientSecrets =
                    {
                        new Secret("ProductService".Sha256())
                    },
                    AllowedScopes = { "productservice-api" }
                }
            };
        }

        /// <summary>
        /// 初始化数据
        /// </summary>
        public static async Task Initialize(IServiceProvider serviceProvider)
        {
            using (var scope = serviceProvider.GetRequiredService<IServiceScopeFactory>().CreateScope())
            {
                var context = scope.ServiceProvider.GetService<FabricDemoDbContext>();
                context.Database.Migrate();

                var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
                var admin = await userManager.FindByNameAsync("admin");
                if (admin == null)
                {
                    admin = new ApplicationUser
                    {
                        UserName = "admin"
                    };
                    var result = await userManager.CreateAsync(admin, "Pass123$");
                    if (!result.Succeeded)
                    {
                        throw new Exception(result.Errors.First().Description);
                    }

                    result = await userManager.AddClaimsAsync(
                        admin,
                        new Claim[]{
                            new Claim(JwtClaimTypes.Name, "Administrator"),
                            new Claim(JwtClaimTypes.Email, "zengql@live.cn"),
                            new Claim(JwtClaimTypes.EmailVerified, "true", ClaimValueTypes.Boolean)
                        });
                    if (!result.Succeeded)
                    {
                        throw new Exception(result.Errors.First().Description);
                    }
                    Console.WriteLine("admin created");
                }
                else
                {
                    Console.WriteLine("admin already exists");
                }

                var zengql = await userManager.FindByNameAsync("zengql");
                if (zengql == null)
                {
                    zengql = new ApplicationUser
                    {
                        UserName = "zengql"
                    };
                    var result = await userManager.CreateAsync(zengql, "Pass123$");
                    if (!result.Succeeded)
                    {
                        throw new Exception(result.Errors.First().Description);
                    }

                    result = await userManager.AddClaimsAsync(
                        zengql,
                        new Claim[]{
                            new Claim(JwtClaimTypes.Name, "zengqinglei"),
                            new Claim(JwtClaimTypes.GivenName, "qinglei"),
                            new Claim(JwtClaimTypes.FamilyName, "zengql"),
                            new Claim(JwtClaimTypes.Email, "zengql@live.cn"),
                            new Claim(JwtClaimTypes.EmailVerified, "true", ClaimValueTypes.Boolean),
                            new Claim(JwtClaimTypes.WebSite, "http://zengql.com"),
                            new Claim(
                                JwtClaimTypes.Address,
                                @"{ 'street_address': 'One Hacker Way', 'locality': 'Heidelberg', 'postal_code': 69118, 'country': 'Germany' }",
                                IdentityServer4.IdentityServerConstants.ClaimValueTypes.Json),
                            new Claim("location", "somewhere")
                        });
                    if (!result.Succeeded)
                    {
                        throw new Exception(result.Errors.First().Description);
                    }
                    Console.WriteLine("user: zengql created");
                }
                else
                {
                    Console.WriteLine("user: zengql already exists");
                }

                scope.ServiceProvider.GetRequiredService<PersistedGrantDbContext>().Database.Migrate();

                var configurationDbContext = scope.ServiceProvider.GetRequiredService<ConfigurationDbContext>();
                configurationDbContext.Database.Migrate();

                foreach (var client in GetClients())
                {
                    if (!await configurationDbContext.Clients.AnyAsync(m => m.ClientId == client.ClientId))
                    {
                        await configurationDbContext.Clients.AddAsync(client.ToEntity());
                        await configurationDbContext.SaveChangesAsync();
                    }
                }

                foreach (var resource in GetIdentityResources())
                {
                    if (!await configurationDbContext.IdentityResources.AnyAsync(m => m.Name == resource.Name))
                    {
                        await configurationDbContext.IdentityResources.AddAsync(resource.ToEntity());
                        await configurationDbContext.SaveChangesAsync();
                    }
                }

                foreach (var apiResource in GetApiResources())
                {
                    if (!await configurationDbContext.ApiResources.AnyAsync(m => m.Name == apiResource.Name))
                    {
                        await configurationDbContext.ApiResources.AddAsync(apiResource.ToEntity());
                        await configurationDbContext.SaveChangesAsync();
                    }
                }
            }
        }
    }
}
