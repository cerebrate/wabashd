using System;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Timers;

namespace ArkaneSystems.Wabash.D
{
    public class WabashD : ServiceBase
    {
	private Timer loop = new Timer();

        public WabashD ()
        { }

        protected override void OnStart (string [] args)
        {
            loop.Interval = 2000 ;
            loop.AutoReset = true ;
            loop.Elapsed += (x, y) => Tick ();

            // Console.WriteLine ("wabashd starting...") ;
            Console.WriteLine ("version: 1") ;

            Tick ();
            loop.Start();
        }

        protected override void OnStop ()
        {
            // Console.WriteLine ("wabashd stopping...") ;
            loop.Stop();

            Console.WriteLine ("stop");
        }

        protected void Tick ()
        {
            int initCount = -1 ;
            int daemonCount = 0 ;

            Process[] processes = from p in Process.GetProcesses()
                                  where p.Parent.Id == 1
                                  select p;

            Console.WriteLine ("sess: {0} daem: {1}", processes.Count(), 0);
        }
    }
}
