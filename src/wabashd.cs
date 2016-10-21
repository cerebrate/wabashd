using System;
using System.Diagnostics;
using System.ServiceProcess;
using System.Threading;
using System.Timers;

using Mono.Unix.Native;

namespace ArkaneSystems.Wabash.D
{
    public class WabashD : ServiceBase
    {
        private const string psCommand = "ps" ;
        private const string psArguments = "--ppid 1 --no-headers -o comm" ;

	private System.Timers.Timer loop = new System.Timers.Timer();

        private object consoleLock = new object ();

        public WabashD ()
        { }

        protected override void OnStart (string [] args)
        {
            loop.Interval = 2000 ;
            loop.AutoReset = true ;
            loop.Elapsed += (x, y) => Tick ();

            lock (consoleLock)
            {
                Console.WriteLine ("version: 2") ;
            }

            Tick ();
            loop.Start();

            ThreadPool.QueueUserWorkItem ( this.HandleInput );
        }

        protected override void OnStop ()
        {
            loop.Stop();

            lock (consoleLock)
            {
                Console.WriteLine ("stop");
            }
        }

        protected void Tick ()
        {
            // Count processes.
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

            lock (consoleLock)
            {
                Console.WriteLine ("sess: {0} daem: {1}", initCount, daemonCount);
            }
        }

	protected void HandleInput (object state)
        {
            // blocking
            var commandString = Console.ReadLine ();

            if (commandString.Length < 4)
            {
                lock (consoleLock)
                {
                    Console.WriteLine ("error");
                    goto skip;
                }
            }

            var action = commandString.Substring (0, 4);

            switch (action)
            {
                case "ping":
                    {
                        lock (consoleLock)
                        {
                            Console.WriteLine ("pong");
                        }
                        break;
                    }
                case "stop":
                    {
                        var pid = Syscall.getpid();
                        Syscall.kill (pid, Signum.SIGTERM);
                        break;
                    }
                case "strt":
                    {
                        var split = commandString.Split (' ') ;
                        if (split.Length != 2)
                        {
                            lock (consoleLock)
                            {
                                Console.WriteLine ("serror <none>");
                                break;
                            }
                        }

                        var serv = split[1];

                        lock (consoleLock)
                        {
                            Process toStart = new Process {
                                StartInfo = new ProcessStartInfo {
                                    FileName = "/usr/bin/sudo",
                                    Arguments = $"/usr/sbin/service {serv} start",
                                    UseShellExecute = false,
                                    RedirectStandardOutput = true
                                }
                            };

                            toStart.Start();
                            var raw = toStart.StandardOutput.ReadToEnd();
                            toStart.WaitForExit();

                            var cooked = raw.Split (new string [] { Environment.NewLine },
                                                    StringSplitOptions.RemoveEmptyEntries );

                            if (cooked.Length == 0)
                            {
                                Console.WriteLine ($"serror {serv}");
                                break;
                            }

                            Console.WriteLine ($"svup {cooked[0]}");
                        }
                        break;
                    }
                default:
                    {
                        lock (consoleLock)
                        {
                            Console.WriteLine ("error");
                        }
                        break;
                    }
            }

            skip:

            // Regenerate self
            ThreadPool.QueueUserWorkItem ( this.HandleInput ) ;
        }
    }
}
