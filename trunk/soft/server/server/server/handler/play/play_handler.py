# -*- coding: utf-8 -*-

import tornado.web
from constant.error import const
from constant.tag import const
from constant.game import const
from utility import util, auth
from utility.msg_pb2 import *
from model import user_data, map_data


class PlayHandler(tornado.web.RequestHandler):
    @auth.authenticated
    def post(self, cmsg):
        if not user_data.check_life(cmsg.common.userid, self.application.static_data):
            user_life_time = user_data.get_user_attr(cmsg.common.userid, "life_time")
            error = msg_life_error()
            error.server_time   = util.now_time()
            error.life_time     = long(user_life_time)
            return  auth.pack(None, -1, cmsg.common.userid, error)

        mapatts = map_data.get_map_attrs(cmsg.id, 
                                         "ownerid", 
                                         "template",
                                         "data",
                                         "hard",
                                         "amount")

        user_data.play_map(cmsg.common.userid, 
                           cmsg.id,
                           const.GAME_NORMAL_MODE)
        if mapatts[0] != '0':
            user_data.inc_user_attr(mapatts[0], "master_exp", 1)
        map_data.play_map(cmsg.id)

        dead_coordinate = map_data.get_dead_coordinate(cmsg.id, int(mapatts[4]))
        smsg = smsg_play_map()
        smsg.map_data = mapatts[2]
        for coord in dead_coordinate:
            x, y = coord.split(":")
            smsg.x.append(int(x))
            smsg.y.append(int(y))
        return auth.pack(smsg, 0, cmsg.common.userid)


class RePlayHandler(tornado.web.RequestHandler):
    @auth.authenticated
    def post(self, cmsg):   
        if not user_data.check_life(cmsg.common.userid, self.application.static_data):
            user_life_time = user_data.get_user_attr(cmsg.common.userid, "life_time")
            error = msg_life_error()
            error.server_time   = util.now_time()
            error.life_time     = long(user_life_time) if user_life_time else util.now_time()
            return auth.pack(None, -1, cmsg.common.userid, error)
        
        mapcache = user_data.get_user_attr(cmsg.common.userid, "mapid")
        if not mapcache:
            return auth.pack(None, const.ERROR_SYSTEM)
        user_data.play_map(cmsg.common.userid, mapcache, const.GAME_NORMAL_MODE)

        ownerid =  map_data.replay_map(mapcache)
        if ownerid != '0':
            user_data.inc_user_attr(ownerid, "master_exp", 1)
        return auth.pack(None, 0, cmsg.common.userid)