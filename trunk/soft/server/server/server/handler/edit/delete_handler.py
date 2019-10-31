# -*- coding: utf-8 -*-


import tornado.web
from constant.error import const
from constant.tag import const
from constant.game import const
from utility import auth
from utility.msg_pb2 import *
from model import user_data

class DeleteMapHandler(tornado.web.RequestHandler):
    @auth.authenticated
    def post(self, cmsg):
        user_map_edit_list = user_data.get_usermap_list(cmsg.common.userid)
        try:
            index = user_map_edit_list.index(str(cmsg.id))
            if index < 0 or index >= 12:
                return auth.pack(None, const.ERROR_SYSTEM)
        except:
            return auth.pack(None, const.ERROR_SYSTEM)

        user_data.delete_usermap(cmsg.common.userid, cmsg.id, index)

        return auth.pack(None, 0, cmsg.common.userid)

