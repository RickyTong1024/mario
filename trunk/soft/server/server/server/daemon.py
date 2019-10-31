# -*- coding: utf-8 -*-

import redis
import msgpack
import tornadb
import sys, os, time, atexit
from signal import SIGTERM 

from model import global_data
from constant.tag import const
from app import client

class Daemon:
    def __init__(self, pidfile, stdin='/dev/null', stdout='/dev/null', stderr='/dev/null'):
        self.stdin = stdin
        self.stdout = stdout
        self.stderr = stderr
        self.pidfile = pidfile
    
    def _daemonize(self):
        try: 
            pid = os.fork() 
            if pid > 0:
                sys.exit(0) 
        except OSError, e: 
            sys.stderr.write("fork #1 failed: %d (%s)\n" % (e.errno, e.strerror))
            sys.exit(1)
        os.chdir("/")
        os.setsid() 
        os.umask(0) 
    
        try: 
            pid = os.fork() 
            if pid > 0:
                sys.exit(0) 
        except OSError, e: 
            sys.stderr.write("fork #2 failed: %d (%s)\n" % (e.errno, e.strerror))
            sys.exit(1) 
        sys.stdout.flush()
        sys.stderr.flush()
        si = file(self.stdin, 'r')
        so = file(self.stdout, 'a+')
        se = file(self.stderr, 'a+', 0)
        os.dup2(si.fileno(), sys.stdin.fileno())
        os.dup2(so.fileno(), sys.stdout.fileno())
        os.dup2(se.fileno(), sys.stderr.fileno())
        atexit.register(self.delpid)
        pid = str(os.getpid())
        file(self.pidfile,'w+').write("%s\n" % pid)
    
    def delpid(self):
        os.remove(self.pidfile)

    def start(self):
        try:
            pf = file(self.pidfile,'r')
            pid = int(pf.read().strip())
            pf.close()
        except IOError:
            pid = None
        if pid:
            message = "pidfile %s already exist. Daemon already running?\n"
            sys.stderr.write(message % self.pidfile)
            sys.exit(1)
        
        self._daemonize()
        self._run()

    def stop(self):
        try:
            pf = file(self.pidfile,'r')
            pid = int(pf.read().strip())
            pf.close()
        except IOError:
            pid = None
    
        if not pid:
            message = "pidfile %s does not exist. Daemon not running?\n"
            sys.stderr.write(message % self.pidfile)
            return 
        try:
            while 1:
                os.kill(pid, SIGTERM)
                time.sleep(0.1)
        except OSError, err:
            err = str(err)
            if err.find("No such process") > 0:
                if os.path.exists(self.pidfile):
                    os.remove(self.pidfile)
            else:
                print str(err)
                sys.exit(1)

    def restart(self):
        self.stop()
        self.start()

    def _run(self):
        pass

class PayDaemon(Daemon):
    def __init__(self, pidfile, stdin = '/dev/null', stdout = '/dev/null', stderr = '/dev/null'):
        Daemon.__init__(self, pidfile, stdin, stdout, stderr)

        self.client = redis.StrictRedis(host = "192.168.2.66", port = 6379, db = 5)
        self.client.ping()

        self.db = torndb.Connection(host="192.168.2.66",
                                    database="malio_pay", 
                                    user="root", 
                                    password="root", 
                                    time_zone="+8:00")

    def _run(self):
        while True:
            pack = self.client.blpop([const.TAG_PAY_QUEUE], 30)
            if not pack:
                continue
            try:
                data = msgpack.loads(pack[1])
            except:
                continue

            try:
                self.db.insert("insert into pay_t (guid, account, userid, title, text, sender_name, type, value1, value2, value3) values (0, %s, %s, %s, %s, %s, %s, %s, %s, %s)")
            except:
                pass
           


if __name__ == "__main__":
    daemon = PayDaemon('/tmp/daemon-update.pid')
    if len(sys.argv) == 2:
        if 'start' == sys.argv[1]:
            daemon.start()
        elif 'stop' == sys.argv[1]:
            daemon.stop()
        elif 'restart' == sys.argv[1]:
            daemon.restart()
        else:
            print "Unknown command"
            sys.exit(2)
        sys.exit(0)
    else:
        print "usage: %s start|stop|restart" % sys.argv[0]
        sys.exit(2)
        