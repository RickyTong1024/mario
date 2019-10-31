# -*- coding: utf-8 -*-

import msgpack
import tornado.web
from constant.error import const
from constant.tag import const
from constant.game import const
from utility import auth
from utility.msg_pb2 import *
from model import user_data, map_data, global_data

class FailHandler(tornado.web.RequestHandler):
    @auth.authenticated
    def post(self, cmsg):
        mapid, index, life, top, total = user_data.get_user_attrs(cmsg.common.userid,
                                               "mapid",
                                               "mission",
                                               "mission_life",
                                               "mission_hard",
                                               "mission_total")
        map_data.complete_map(mapid,
                              cmsg.common.userid,
                              -2,
                              cmsg.x,
                              cmsg.y,
                              0,
                              self.application.update_map_script)

        if int(life) <= 0:
            rank = 0
            if int(index) >= 1:
                global_data.challenge_rank(const.TAG_RANK_CHALLENGE,
                                                  cmsg.common.userid, 
                                                  int(index) * 100000 + 100000 - int(total),
                                                  self.application.challenge_rank_script,
                                                  top)

                rank = global_data.challenge_finish_rank(const.TAG_RANK_CHALLENGE_CUR,
                                                  cmsg.common.userid, 
                                                  int(index) * 100000 + 100000 - int(total),
                                                  self.application.challenge_rank_finish_script)
            smsg = smsg_challenge_finish()
            smsg.suc = 1
            smsg.exp = 0
            smsg.jewel = 0
            smsg.rank = rank
            all_map = global_data.get_challenge_map(const.TAG_CHALLENGE_MAP_LIST, 0, index)
            for mapid in all_map:
                author = smsg.authors.add()
                submap = msgpack.loads(mapid)
                author.user_head    = int(submap[5])
                author.user_name    = submap[2]
                author.user_country = submap[4]
                author.map_name     = submap[1]
            return auth.pack(None, -1, cmsg.common.userid, smsg)

        return auth.pack(None, 0, cmsg.common.userid)