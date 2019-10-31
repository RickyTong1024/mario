# -*- coding: utf-8 -*-

import tornado.web
from constant.error import const
from constant.tag import const
from constant.game import const
from utility import auth
from utility.msg_pb2 import *
from model import user_data, map_data, global_data

class BuyHandler(tornado.web.RequestHandler):
    @auth.authenticated
    def post(self, cmsg):
        jewel, relife = user_data.get_user_attrs(cmsg.common.userid, "jewel", "mission_relive")
        jewel  = int(jewel)
        relife = int(relife) + 1
        if relife > 6:
            relife = 6
        mission_template = self.application.static_data.get_mission_template(relife)
        if not mission_template:
            return auth.pack(None, const.ERROR_SYSTEM)

        if jewel < mission_template.jewel:
            return auth.pack(None, const.ERROR_NO_JEWEL)

        user_data.inc_user_attrs(cmsg.common.userid, [("mission_life",10), ("jewel", -mission_template.jewel),
                                                      ("mission_relive",1)])

        return auth.pack(None, 0, cmsg.common.userid)