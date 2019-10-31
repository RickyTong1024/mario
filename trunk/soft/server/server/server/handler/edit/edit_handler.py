# -*- coding: utf-8 -*-

import tornado.web
from constant.error import const
from constant.tag import const
from constant.game import const
from utility import auth
from utility.msg_pb2 import *
from model import user_data

class EditHandler(tornado.web.RequestHandler):
    @auth.authenticated
    def post(self, cmsg):
        smsg = smsg_play_edit_map()
        smsg.mapdata = user_data.get_usermap_attr(cmsg.common.userid, cmsg.id, "data") or ""
        return auth.pack(smsg, 0, cmsg.common.userid)
