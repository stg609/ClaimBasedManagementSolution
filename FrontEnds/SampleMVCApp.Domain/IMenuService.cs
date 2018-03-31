using System.Collections.Generic;
using System.Security.Claims;

namespace SampleMVCApp.Domain
{
    public interface IMenuService
    {
        MenuDTO FindByKey(int key);

        List<MenuDTO> GenerateMenusByControllerAction(bool isSync = false);
        List<MenuDTO> GetAll();
        List<MenuDTO> GetMenusByClaims(IEnumerable<Claim> claims);
        void Create(MenuDTO menuDTO);
        void Update(int key, string name, string url, bool visible = true, int parentKey = 0);
        void SetVisibility(int key, bool visible);
        void Move(int key, int step = 0);
        void Delete(int key);
    }
}