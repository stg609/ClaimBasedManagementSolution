using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DryIoc;
using SampleMVCApp.Domain;
using SampleMVCApp.Infra;

namespace SampleMVCApp.Services
{
    public class CompositionRoot
    {
        public CompositionRoot(IRegistrator registrator)
        {
            registrator.Register<IUnitOfWorkManager, UnitOfWorkManager>();
            registrator.Register<IRepository<MenuDTO, string>, GeneralRepository<MenuDTO, string>>();
            registrator.Register<IUnitOfWork, UnitOfWork>();
            registrator.Register<MenuService>();
        }
    }
}
