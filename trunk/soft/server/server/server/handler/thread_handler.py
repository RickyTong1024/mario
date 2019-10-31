# -*- coding: utf-8 -*-

import tornado.web
import tornado.gen
from constant.error import const
from utility import auth

class ThreadHandler(tornado.web.RequestHandler):
    handler_connection_closed = False

    @tornado.web.asynchronous
    @tornado.gen.coroutine
    def post(self):
        cmsg = self.application.msg_factory(self.request.uri)
        result = auth.unpack(cmsg, self.request.body)
        if not result:
            self.__inter_response(auth.pack(None, const.ERROR_DECODE))
            return

        if cmsg.DESCRIPTOR.fields_by_name.has_key("common"):
            result = auth.check(cmsg.common.userid, cmsg.common.sig, cmsg.common.pck_id)
            if isinstance(result, int):
                if result != 0:
                    self.__inter_response(auth.pack(None, result))
                    return
            else:
                self.__inter_response(result)
                return

        response = yield self.application.executer.cmd(self._handle_msg, 
                                                     cmsg)
        self.__inter_response(response)

    def _handle_msg(self, body):
        raise NotImplementedError()

    def __inter_response(self, content):
        if not self.handler_connection_closed:
            self.write(content)
            self.finish()

    def on_connection_close(self):
        self.handler_connection_closed = True


        