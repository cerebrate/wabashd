#!/usr/bin/python

import psutil

from daemonize import Daemonize
from time import sleep

pid = "/var/run/wabashd"

def main():
  while (1):
    init_count = -1
    daemon_count = 0

    for proc in psutil.process_iter():
      try:
        pinfo = proc.as_dict(attrs=['pid', 'ppid', 'name'])
      except psutil.NoSuchProcess:
        pass
      else:
        if pinfo['ppid'] == 1:
          if pinfo['name'] == 'init':
            init_count += 1
          else:
            daemon_count += 1

    # do not change this format; the Windows software relies on it
    print 'inits: ', init_count, 'daemons: ', daemon_count

    sleep (10)

# unusual case in which this must run in foreground; otherwise dies on daemonization
daemon = Daemonize(app="wabashd", pid=pid, action=main, foreground = True)
daemon.start()


