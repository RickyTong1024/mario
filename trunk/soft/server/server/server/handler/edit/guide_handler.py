# -*- coding: utf-8 -*-

import tornado.web
from constant.error import const
from constant.tag import const
from constant.game import const
from utility import auth
from utility.msg_pb2 import *
from model import user_data, global_data

class GuideHandler(tornado.web.RequestHandler):
    @auth.authenticated
    def post(self, cmsg):
        edit_map_list = user_data.get_usermap_list(cmsg.common.userid)
        if not edit_map_list:
            user_data.create_usermap_list(cmsg.common.userid)

        if edit_map_list and edit_map_list[0] != '0':
            return auth.pack(None, const.ERROR_HAS_GUIDE)

        mapid = global_data.get_counter(const.TAG_USERMAP_COUNT)
        user_data.create_usermap(cmsg.common.userid, mapid, 0, "empty", cmsg.url, cmsg.data, 100)

        return auth.pack(None, 0, cmsg.common.userid)