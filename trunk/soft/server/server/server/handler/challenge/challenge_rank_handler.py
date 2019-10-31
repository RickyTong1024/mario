# -*- coding: utf-8 -*-

import tornado.web
from constant.error import const
from constant.tag import const
from constant.game import const
from utility import auth
from utility.msg_pb2 import *
from model import user_data, map_data, global_data

class RankHandler(tornado.web.RequestHandler):
    @auth.authenticated
    def post(self, cmsg):
        smsg = smsg_challenge_rank()
        rank_list = global_data.get_map_list(const.TAG_RANK_CHALLENGE, 0, -1, True)
        if rank_list:
            for useridpoint in rank_list:
                user = user_data.get_user_attrs(useridpoint[0], "name", "country", "head", "level", "visitor")
                userinfo = smsg.ranks.add()
                userinfo.user_head      = int(user[2])
                userinfo.user_name      = user[0]
                userinfo.user_country   = user[1]
                userinfo.user_id        = int(useridpoint[0])
                userinfo.user_visitor   = int(user[4])
                userinfo.user_level     = int(user[3])
                userinfo.user_index     = int(useridpoint[1]) / 100000
                userinfo.user_life      = 100000 - (int(useridpoint[1]) - userinfo.user_index  * 100000) 

        return auth.pack(smsg, 0, cmsg.common.userid)