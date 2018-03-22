using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Reflection;
using System.Security.Claims;
using System.Text;
using System.Text.RegularExpressions;
using Constants;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace Infra
{
    public static class ClaimsAnalyzer
    {
        public static bool HasClaim(this IEnumerable<Claim> claims, string targetClaimType, string targetClaimValue)
        {
            if (claims.Any(itm => itm.Type.Equals(ClaimConstants.PermissionClaimType) && itm.Value.Equals(String.Join(".", ClaimConstants.PolicyPrefix, ClaimConstants.ClaimValue_AllowAll))))
            {
                return true;
            }

            if (!claims.Any(itm => itm.Type.Equals(targetClaimType) && itm.Value.Equals(targetClaimValue)))
            {
                return false;
            }

            return true;
        }

        public static List<MethodInfo> GetControllerActionList(Assembly assembly)
        {
            //得到所有 actions of Controller (no matter it is Api Controller or Mvc Controller)
            var controlleractionlist = assembly.GetTypes()
                  .Where(type => typeof(Controller).IsAssignableFrom(type))
                  .SelectMany(type => type.GetMethods(BindingFlags.Instance | BindingFlags.DeclaredOnly | BindingFlags.Public))
                  .Where(action => !(action.GetCustomAttributes(typeof(System.Runtime.CompilerServices.CompilerGeneratedAttribute), true).Any()
                                      || action.GetCustomAttributes<HttpPostAttribute>().Any()
                                      || action.GetCustomAttributes<HttpPutAttribute>().Any()
                                      || action.GetCustomAttributes<HttpDeleteAttribute>().Any()))//过滤掉编译器生成的 action
                  .OrderBy(action => action.DeclaringType.Name)
                  .ThenBy(action => action.Name)
                  .ToList();

            return controlleractionlist;
        }

        public static IEnumerable<Claim> GetAllClaimsOfControllers(Assembly assembly)
        {
            if (assembly == null)
            {
                return Extensions.Empty<List<Claim>>();
            }
            
            List<Claim> results = new List<Claim>();

            var controlleractionlist = GetControllerActionList(assembly);
            //遍历每个 action 得到带有 CLAIM 的 authorizeAttribute
            controlleractionlist.ForEach(action =>
            {
                IEnumerable<AuthorizeAttribute> authorizeAttrs = action.GetCustomAttributes<AuthorizeAttribute>();

                if (authorizeAttrs != null && authorizeAttrs.Any())
                {
                    Regex regexClaimPolicy = new Regex(ClaimConstants.CLAIM_REGULAR_PATTERN);
                    var claims = from attr in authorizeAttrs
                                 where attr.Policy != null && regexClaimPolicy.IsMatch(attr.Policy)
                                 let match = regexClaimPolicy.Match(attr.Policy)
                                 select new Claim(ClaimConstants.PermissionClaimType, match.Groups[0].Value);

                    results.AddRange(claims);
                }
            });
            //        IEnumerable<AuthorizeAttribute> attrInAssembly = assembly.GetCustomAttributes<AuthorizeAttribute>();

            //List<AuthorizeAttribute> attrInType = new List<AuthorizeAttribute>();
            //List<AuthorizeAttribute> attrInMethod = new List<AuthorizeAttribute>();

            //var types = assembly.GetTypes();
            //types.AsParallel().ForAll(itm =>
            //{
            //    //Get Authorize Attribute from Type
            //    attrInType.AddRange(itm.GetCustomAttributes<AuthorizeAttribute>());

            //    itm.GetMethods(BindingFlags.).AsParallel().ForAll(mtd =>
            //    {
            //        //Get Authorize Attribute from method
            //        attrInMethod.AddRange(mtd.GetCustomAttributes<AuthorizeAttribute>());
            //    });
            //});

            //Regex regexClaimPolicy = new Regex(ClaimConstants.PolicyPrefix + @"\.[\w\.]+[\w\*]+");


            //var claims = from attr in attrInAssembly.Union(attrInType).Union(attrInMethod)
            //             where attr.Policy != null && regexClaimPolicy.IsMatch(attr.Policy)
            //             let match = regexClaimPolicy.Match(attr.Policy)
            //             select new Claim(ClaimConstants.PermissionClaimType, match.Groups[0].Value);

            //if (_dbContext.Claims.Any())
            //{
            //    claims = claims.Except(_dbContext.Claims, new LambdaEqualityComparer<ClaimDTO>((x, y) => x.Type.Equals(y.Type) && x.Value.Equals(y.Value)));
            //}

            //int maxKey = _dbContext.Claims.Any() ? _dbContext.Claims.Max(itm => itm.Key) : 0;
            //foreach (var itm in claims)
            //{
            //    itm.Key = maxKey++;
            //}

            //await _dbContext.Claims.AddRangeAsync(claims);
            //await _dbContext.SaveChangesAsync();

            return results;
        }

        public static bool SendClaimToIdentityServer(HttpClient httpClient, string idServerClaimAPIUrl, string identity)
        {
            var claims = ClaimsAnalyzer.GetAllClaimsOfControllers(Assembly.GetExecutingAssembly());
            if (claims == null || !claims.Any())
            {
                return true;
            }

            var body = JsonConvert.SerializeObject(new
            {
                Identity = identity,
                Claims = from claim in claims
                         select new { Type = claim.Type, Value = claim.Value }
            });
            var stringContent = new StringContent(body, Encoding.UTF8, "application/json");

            var result = httpClient.PostAsync(idServerClaimAPIUrl, stringContent).Result;
            return result.IsSuccessStatusCode;
        }
    }

    public class LambdaEqualityComparer<T> : IEqualityComparer<T>
    {
        private Func<T, T, bool> _predicate;

        public LambdaEqualityComparer(Func<T, T, bool> predicate)
        {
            _predicate = predicate;
        }

        public bool Equals(T x, T y)
        {
            return _predicate(x, y);
        }

        public int GetHashCode(T obj)
        {
            return obj.GetHashCode();
        }
    }
}
 
