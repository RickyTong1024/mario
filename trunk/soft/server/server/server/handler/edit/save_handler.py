# -*- coding: utf-8 -*-

import tornado.web
from constant.error import const
from constant.tag import const
from constant.game import const
from utility import auth
from utility.msg_pb2 import *
from model import user_data

class SaveHandler(tornado.web.RequestHandler):
    @auth.authenticated
    def post(self, cmsg):
        if not user_data.exist_usermap(cmsg.common.userid, cmsg.id):
            return auth.pack(None, const.ERROR_SYSTEM)

        user_data.set_usermap_attrs(cmsg.common.userid, cmsg.id, {"url":cmsg.url, "data":cmsg.mapdata})
        return auth.pack(None, 0, cmsg.common.userid)