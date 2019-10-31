# -*- coding: utf-8 -*-


import tornado.web
import tornado.gen
import functools
from protocol.msg_pb2 import *
from protocol import proto
from constant.error import const
from utility import util
from model import user_data
from protocol import proto


def login(method):
    @functools.wraps(method)
    def wrapper(self, *args, **kwargs):
        msg = self.application.msg_factory(self.request.uri)
        if msg is None:
            raise tornado.web.HTTPError(403)
        check = True
        if self.request.uri == "/10001":
            check = False 
        result = proto.unpack(msg, self.request.body, check)
        if result != 0:
            if result == 1:
                self.write(user_data.get_user_attr(msg.common.userid, "pck"))
            else:
                self.write(proto.pack(msg.common.userid, None, result))
            return
        return method(self, msg)
    return wrapper


class SyncHandler(tornado.web.RequestHandler):
    @login
    @tornado.web.authenticated
    def post(self, msg):
        print msg