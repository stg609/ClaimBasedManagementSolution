using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace Infra
{
    public static class Extensions
    {
        public static TList Empty<TList>() where TList : IEnumerable, new()
        {
            return new TList();
        }
    }
}
