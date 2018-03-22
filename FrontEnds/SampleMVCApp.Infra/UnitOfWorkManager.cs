using System;
using System.Collections.Generic;
using System.Text;
using DryIoc;
using SampleMVCApp.Domain;

namespace SampleMVCApp.Infra
{
    public class UnitOfWorkManager : IUnitOfWorkManager
    {
        private IContainer _container;

        public UnitOfWorkManager(IContainer container)
        {
            _container = container;
        }

        public IUnitOfWork Begin()
        {
            return _container.Resolve<IUnitOfWork>();
        }
    }
}
