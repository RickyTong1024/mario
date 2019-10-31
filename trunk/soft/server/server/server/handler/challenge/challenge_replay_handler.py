# -*- coding: utf-8 -*-

import tornado.web
from constant.error import const
from constant.tag import const
from constant.game import const
from utility import auth
from utility.msg_pb2 import *
from model import user_data, map_data, global_data


class RePlayHandler(tornado.web.RequestHandler):
    @auth.authenticated
    def post(self, cmsg):
        mapid, life = user_data.get_user_attrs(cmsg.common.userid,
                                               "mapid",
                                               "mission_life")

        if int(life) <= 0:
            return auth.pack(None, const.ERROR_SYSTEM)

        user_data.play_map(cmsg.common.userid, mapid, const.GAME_CHALLENGE_PLAY_MODE)
        ownerid =  map_data.replay_map(mapid)
        if ownerid != '0':
            user_data.inc_user_attr(ownerid, "master_exp", 1)
        
        return auth.pack(None, 0, cmsg.common.userid)  