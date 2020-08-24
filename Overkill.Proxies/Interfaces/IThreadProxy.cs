using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace Overkill.Proxies.Interfaces
{
    public interface IThreadProxy
    {
        IThreadProxy Create(ThreadStart function);
        IThreadProxy Create(ParameterizedThreadStart function);
        void Start();
        void Abort();
    }
}
