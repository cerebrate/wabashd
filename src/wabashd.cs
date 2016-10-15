using System;
using System.Diagnostics;
using System.ServiceProcess;
using System.Timers;

namespace ArkaneSystems.Wabash.D
{
    public class WabashD : ServiceBase
    {
        private const string psCommand = "ps" ;
        private const string psArguments = "--ppid 1 --no-headers -o comm" ;

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

            var proc = new Process {
              StartInfo = new ProcessStartInfo {
                FileName = psCommand,
                Arguments = psArguments,
                UseShellExecute = false,
                RedirectStandardOutput = true
              }
            };

            proc.Start();
            var raw = proc.StandardOutput.ReadToEnd();
            proc.WaitForExit();

            var cooked = raw.Split (new string [] { Environment.NewLine },
                                    StringSplitOptions.RemoveEmptyEntries ) ;

            foreach (string s in cooked)
              if (s == "init")
                initCount++;
              else
                daemonCount++;

            Console.WriteLine ("sess: {0} daem: {1}", initCount, daemonCount);
        }
    }
}
