using System;
using System.ServiceProcess;

namespace ArkaneSystems.Wabash.D
{
    static class Program
    {
        static void Main()
        {
            ServiceBase[] servicesToRun = new ServiceBase []
                {
                    new WabashD ()
                } ;

            ServiceBase.Run (servicesToRun);
        }
    }
}
