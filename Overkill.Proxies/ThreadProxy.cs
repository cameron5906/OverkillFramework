using Overkill.Proxies.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace Overkill.Proxies
{
    /// <summary>
    /// Proxy class to assist in unit testing thread related functionality
    /// </summary>
    public class ThreadProxy : IThreadProxy
    {
        private Thread thread;

        public IThreadProxy Create(ThreadStart function)
        {
            return new ThreadProxy(function);
        }

        public IThreadProxy Create(ParameterizedThreadStart function)
        {
            return new ThreadProxy(function);
        }

        public ThreadProxy() { }

        public ThreadProxy(ThreadStart function)
        {
            thread = new Thread(function);
        }

        public ThreadProxy(ParameterizedThreadStart function)
        {
            thread = new Thread(function);
        }

        public void Start()
        {
            thread.Start();
        }

        public void Abort()
        {
            thread.Abort();
        }
    }
}
