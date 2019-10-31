# -*- coding: utf-8 -*-

import tornado.web
from constant.error import const
from constant.tag import const
from constant.game import const
from utility import auth
from utility.msg_pb2 import *
from model import user_data, map_data


class ViewRankHandler(tornado.web.RequestHandler):
    @auth.authenticated
    def post(self, cmsg):
        smsg = smsg_view_map_point_rank()
        map_users = map_data.get_rank_map_list(const.TAG_MAP_POINT.format(cmsg.map_id), 0, -1, False)
        for users in map_users:
            userattrs = user_data.get_user_attrs(users[0], "name", "level", "country", "visitor")
            rank_info = smsg.ranks.add()
            rank_info.player_name       = userattrs[0]
            rank_info.player_point      = int(users[1])
            rank_info.player_level      = int(userattrs[1])
            rank_info.player_country    = userattrs[2]
            rank_info.video_id          = int(users[0])
            rank_info.user_id           = int(users[0])
            rank_info.visitor           = int(userattrs[3])

        return auth.pack(smsg, 0, cmsg.common.userid)
        