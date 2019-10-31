# -*- coding: utf-8 -*-

import msgpack
import tornado.web
from constant.error import const
from constant.tag import const
from constant.game import const
from utility import auth
from utility.msg_pb2 import *
from model import user_data, map_data, global_data, cache_data


class SuccessHandler(tornado.web.RequestHandler):
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
                              -1,
                              0,
                              0,
                              cmsg.point,
                              self.application.update_map_script)

        # 成功结束
        if int(index) + 1 >= 8:
            global_data.challenge_rank(const.TAG_RANK_CHALLENGE,
                                          cmsg.common.userid, 
                                          (int(index) + 1) * 100000 + 100000 - int(total),
                                          self.application.challenge_rank_script,
                                          top)

            challenge_info = cache_data.get_cache(const.TAG_CHALLENGE_INFO, 300)
            inc_exp = challenge_info.get("exp") or 0
            inc_jewel = challenge_info.get("jewel") or 0
            user_data.challenge_complete(cmsg.common.userid, inc_exp, inc_jewel, self.application.static_data)

            rank = 0
            rank = global_data.challenge_finish_rank(const.TAG_RANK_CHALLENGE_CUR,
                                                  cmsg.common.userid, 
                                                  int(index) * 100000 + 100000 - int(total),
                                                  self.application.challenge_rank_finish_script)
            
            smsg = smsg_challenge_finish()
            smsg.suc = 0
            smsg.exp = inc_exp
            smsg.jewel = inc_jewel
            smsg.rank = rank
            all_map = global_data.get_challenge_map(const.TAG_CHALLENGE_MAP_LIST, 0, -1)
            for mapid in all_map:
                author = smsg.authors.add()
                submap = msgpack.loads(mapid)
                author.user_head    = int(submap[5])
                author.user_name    = submap[2]
                author.user_country = submap[4]
                author.map_name     = submap[1]
            return auth.pack(None, -1, cmsg.common.userid, smsg)

        
        mapinfo = global_data.get_challenge_mapid(const.TAG_CHALLENGE_MAP_LIST, int(index) + 1)
        if mapinfo is None:
            return auth.pack(None, const.ERROR_SYSTEM)
        mapinfo = msgpack.loads(mapinfo)
        
        user_data.play_map(cmsg.common.userid, mapinfo[0], const.GAME_CHALLENGE_SUCC_MODE)
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