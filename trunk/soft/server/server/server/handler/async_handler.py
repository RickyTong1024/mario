# -*- coding: utf-8 -*-

import tornado.web
import tornado.gen
import urllib
from tornado import escape
from tornado.httpclient import AsyncHTTPClient
from constant.error import const
from utility import auth

class AsyncHandler(tornado.web.RequestHandler):
    handler_connection_closed = False

    @tornado.gen.coroutine
    def parse(self):
        cmsg = self.application.msg_factory(self.request.uri)
        result = auth.unpack(cmsg, self.request.body)
        if not result:
            self.response(const.ERROR_DECODE)
            raise tornado.gen.Return(False)

        if cmsg.DESCRIPTOR.fields_by_name.has_key("common"):
            result = auth.check(cmsg.common.userid, cmsg.common.sig, cmsg.common.pck_id)
            if isinstance(result, int):
                if result != 0:
                    self.response(result)
                    raise tornado.gen.Return(False)
            else:
                self.response_last(result)
                raise tornado.gen.Return(False)

        raise tornado.gen.Return(cmsg)


    @tornado.gen.coroutine
    def rpc(self, cmd, **kwargs):
        url = self.application.notify_addr + cmd
        url += "?" + urllib.urlencode(kwargs)
        http_client = AsyncHTTPClient()
        try:
            respone = yield http_client.fetch(url)
        except Exception, e:
            raise tornado.gen.Return({"res":-1})

        try:
            data = escape.json_decode(respone.body)
        except:
            raise tornado.gen.Return({"res":-1})

        raise tornado.gen.Return(data)


    def response(self, res, smsg = None, userid = None, error = None):
        if not self.handler_connection_closed:
            self.write(auth.pack(smsg, res, userid, error))
            self.finish()

    def response_last(self, smsg):
        if not self.handler_connection_closed:
            self.write(smsg)
            self.finish()

    def on_connection_close(self):
        self.handler_connection_closed = True
