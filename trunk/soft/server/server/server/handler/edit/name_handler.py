# -*- coding: utf-8 -*-

import tornado.web
from constant.error import const
from constant.tag import const
from constant.game import const
from utility import util, auth
from utility.msg_pb2 import *
from model import user_data


class ChangeNameHandler(tornado.web.RequestHandler):
    @auth.authenticated
    def post(self, cmsg):
        if not util.check_input_nospace(cmsg.name, 16):
            return auth.pack(None, const.ERROR_INPUT)

        if not user_data.exist_usermap(cmsg.common.userid, cmsg.id):
            return auth.pack(None, const.ERROR_SYSTEM)

        user_data.set_usermap_attr(cmsg.common.userid, cmsg.id, "name", cmsg.name)

        return auth.pack(None, 0, cmsg.common.userid)