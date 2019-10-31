# -*- coding: utf-8 -*-

"""
登录处理器
"""

import tornado.web
from constant.error import const
from constant.tag import const
from constant.game import const
from utility import util, client, auth
from utility.msg_pb2 import *
from model import user_data, global_data

class ChangeNameHandler(tornado.web.RequestHandler):
    @auth.authenticated
    def post(self, cmsg):
        visitor = user_data.get_user_attr(cmsg.common.userid, "visitor")
        if not visitor or visitor != "1":
            return auth.pack(None, const.ERROR_SYSTEM)

        if not util.check_input_nospace(cmsg.nickname, 16):
            return auth.pack(None, const.ERROR_INPUT)

        user_data.set_user_attrs(cmsg.common.userid, {"visitor":0,
                                                     "name":cmsg.nickname,
                                                     "head":cmsg.head})

        return auth.pack(None, 0, cmsg.common.userid)