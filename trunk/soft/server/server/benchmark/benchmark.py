# -*- coding: utf-8 -*-

import random
import time
import tornado.ioloop
import tornado.httpclient
from tornado import process
from Crypto.Cipher import DES
from tornado import process
import redis
import msgpack
from msg_pb2 import *

BS = 8
pad = lambda s: s + (BS - len(s) % BS) * chr(BS - len(s) % BS) 

from tornado.options import define, options
define("url", default="120.131.3.168:8080")
define('proc_num', type=int, default=0)
define('client_num', type=int, default=10)
define('max_client_num', type=int, default=10)
define('time', type=int, default=60)
define('timeout', type=int, default=10)
define('redis', default="192.168.2.66:6379:0")

class Benchmark(object):
    def __init__(self, 
                 clientnum, 
                 max_clientnum, 
                 time, 
                 timeout, 
                 redis_conf, 
                 url,
                 proc_num
                 ):
        self.clientnum     = clientnum
        self.max_clientnum = max_clientnum
        self.test_time     = time
        self.io_loop       = tornado.ioloop.IOLoop()
        self.request_timemout = timeout
        self.url = url

        self.client = tornado.httpclient.AsyncHTTPClient(self.io_loop, 
                                                        max_clients = self.max_clientnum)

        host, port, num = redis_conf.split(":")
        self.redis = redis.StrictRedis(host = host, port = int(port), db = int(num))
        self.redis.ping()
        if self.redis.exists("benchmark"):
            self.redis.delete("benchmark")


        self.redis.hmset("benchinfo", {"clientnum":clientnum, "max_clientnum":max_clientnum,
                                      "time":time, "proc_num":proc_num, "request_fail":0,
                                      "request_succ":0, "request_cost":0.0})

    def test(self):
        for i in xrange(self.clientnum):
            self.client.fetch(self._get_request(10001), callback=lambda response:self._handle_request(response, 10001))

        self.start_time = time.time()
        self.end = self.start_time + self.test_time
        self.io_loop.start()

    def _get_request(self, opcode, **kwargs):
        url     =   "http://" + self.url + "/%d" % opcode

        msg = None
        if opcode == 10001:
            msg = cmsg_login()
            msg.openid  = ""
            msg.openkey = ""
            msg.nationality = "CN"
        elif opcode == 10004:
            msg = cmsg_view_map()
            msg.common.userid = kwargs.get("userid")
            msg.common.pck_id = kwargs.get("pck")
            msg.common.sig    = kwargs.get("sig")
            msg.type          = random.randint(1,3)
            msg.index         = 0
        elif opcode == 10006:
            msg = cmsg_play_map()
            msg.common.userid = kwargs.get("userid")
            msg.common.pck_id = kwargs.get("pck")
            msg.common.sig    = kwargs.get("sig")
            msg.id            = random.randint(10010001, 10010040)
        elif opcode == 10010:
            msg = cmsg_replay_map()
            msg.common.userid = kwargs.get("userid")
            msg.common.pck_id = kwargs.get("pck")
            msg.common.sig    = kwargs.get("sig")
        elif opcode == 10007:
            msg = cmsg_complete_map()
            msg.common.userid = kwargs.get("userid")
            msg.common.pck_id = kwargs.get("pck")
            msg.common.sig    = kwargs.get("sig")
            msg.suc = 1
            msg.x = 0
            msg.y = 0
        else:
            pass

        body = pad(msg.SerializeToString())
        codec = DES.new('tsjhtsjh', DES.MODE_CBC, '51478543')
        body = codec.encrypt(body)

        httprequest = tornado.httpclient.HTTPRequest(url = url,
                                                     method = "POST", 
                                                     body = body, 
                                                     request_timeout = self.request_timemout)
        return httprequest

    def _get_response(self, opcode, body):
        msg = msg_response()
        msg.ParseFromString(body)
        if msg.res != 0:
            print msg.res


    def _handle_request(self, response, opcode, userid, sig, pck):
        if time.time() > self.end:
            self.io_loop.stop()
            return

        if response.error:
            print "response error"
            return

        #cost_time = response.request_time

        ##if response.error:
        ##    self.redis.rpush("benchmark", msgpack.dumps([-1, cost_time]))
        ##else:
        ##    self.redis.rpush("benchmark", msgpack.dumps([0, cost_time]))
        ##    self._get_response(opcode, response.body)

        if opcode == 10001:
            msg = msg_response()
            msg.ParseFromString(response.body)
            if msg.res != 0:
                print "response code error", msg.res
                return
            smsg = smsg_login()
            smsg.ParseFromString(msg.msg)

            self.client.fetch(self._get_request(10004, userid = smsg.userid, sig = smsg.sig, pck = 0), 
                              callback=lambda response:self._handle_request(response, 10004, smsg.userid, smsg.sig, 0))
        elif opcode == 10004:
            self.client.fetch(self._get_request(10006, userid = userid, sig = sig, pck = pck + 1), 
                              callback=lambda response:self._handle_request(response, 10004, smsg.userid, smsg.sig, 0))
        elif opcode == 10006:
            self.client.fetch(self._get_request(10010, userid = userid, sig = sig, pck = pck + 1), 
                              callback=lambda response:self._handle_request(response, 10010, smsg.userid, smsg.sig, 0))
        elif opcode == 10010:
            self.client.fetch(self._get_request(10007, userid = userid, sig = sig, pck = pck + 1), 
                              callback=lambda response:self._handle_request(response, 10007, smsg.userid, smsg.sig, 0))
        elif opcode == 10007:
            self.client.fetch(self._get_request(10010, userid = userid, sig = sig, pck = pck + 1), 
                              callback=lambda response:self._handle_request(response, 10010, smsg.userid, smsg.sig, 0))
        else:
            return

        




if __name__ == "__main__":
    tornado.options.parse_command_line()

    proc_num = options.proc_num
    if proc_num == -1:
        import multiprocessing
        proc_num = multiprocessing.cpu_count()
          
    if proc_num != 0:  
        tornado.process.fork_processes(proc_num)

    benchmark = Benchmark(options.client_num, 
                          options.max_client_num, 
                          options.time, 
                          options.timeout,
                          options.redis,
                          options.url,
                          proc_num)
    benchmark.test()
            
    
