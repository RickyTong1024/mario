# -*- coding: utf-8 -*-

import msgpack
import tornado.web
from constant.error import const
from constant.tag import const
from constant.game import const
from utility import auth
from utility.msg_pb2 import *
from model import user_data, map_data, global_data

class StartHandler(tornado.web.RequestHandler):
    @auth.authenticated
    def post(self, cmsg):
        index, life, start = user_data.get_user_attrs(cmsg.common.userid,
                                               "mission",
                                               "mission_life",
                                               "mission_next")
        if int(life) <= 0:
            return auth.pack(None, const.ERROR_SYSTEM)

        mapinfo = global_data.get_challenge_mapid(const.TAG_CHALLENGE_MAP_LIST, index)
        if mapinfo is None:
            return auth.pack(None, const.ERROR_SYSTEM)
        mapinfo = msgpack.loads(mapinfo)
        
        user_data.play_map(cmsg.common.userid, mapinfo[0], const.GAME_CHALLENGE_PLAY_MODE)
        if start == "0":
            user_data.inc_user_attr(cmsg.common.userid, "mission_next", 1)
        map_data.play_map(mapinfo[0])

        if mapinfo[3] != '0':
            user_data.inc_user_attr(mapinfo[3], "master_exp", 1)

        smsg = smsg_challenge_play()
        smsg.map_name       = mapinfo[1]
        smsg.user_name      = mapinfo[2]
        smsg.user_country   = mapinfo[4]
        smsg.user_head      = int(mapinfo[5])
        smsg.map_data       = mapinfo[6]
        map_coordinate = map_data.get_dead_coordinate(mapinfo[0], 50)
        for coord in map_coordinate:
            x, y = coord.split(":")
            smsg.x.append(int(x))
            smsg.y.append(int(y))
        return auth.pack(smsg, 0, cmsg.common.userid)