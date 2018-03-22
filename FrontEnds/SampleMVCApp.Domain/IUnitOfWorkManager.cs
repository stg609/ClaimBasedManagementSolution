using System;
using System.Collections.Generic;
using System.Text;

namespace SampleMVCApp.Domain
{
    public interface IUnitOfWorkManager
    {
        IUnitOfWork Begin();
    }
}
