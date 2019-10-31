# -*- coding: utf-8 -*-

import re
import tornado.web
from constant.error import const
from constant.tag import const
from constant.game import const
from utility import util, auth
from utility.msg_pb2 import *
from model import user_data, map_data

class SearchMapHandler(tornado.web.RequestHandler):
    @auth.authenticated
    def post(self, cmsg):
        if not util.check_input(cmsg.name, 32):
            return auth.pack(None, const.ERROR_INPUT)

        smsg = smsg_view_map()
        smsg.page = 0

        import platform
        if platform.system() == "Windows":
            return auth.pack(smsg, 0, cmsg.common.userid)

        map_list = None
        import redis_search.query
        # 纯英文数字组合
        if re.match('^[a-zA-Z0-9]+$', cmsg.name):
            if cmsg.name.isdigit() and len(cmsg.name) == 8:
                if int(cmsg.name) < 10010000:
                    return auth.pack(smsg, 0, cmsg.common.userid)

                if map_data.exist_map(cmsg.name):
                    map_list = [cmsg.name]
                else:
                    return auth.pack(smsg, 0, cmsg.common.userid)
            else:
                map_query = redis_search.query.complete(const.TAG_MAP_SEARCH, cmsg.name)
                map_list =  [s["id"] for s in map_query]

                map_query = redis_search.query.query(const.TAG_MAP_SEARCH, cmsg.name)
                map_list_temp =  [s["id"] for s in map_query]
                for itemyuan in map_list_temp:
                    if itemyuan not in map_list:
                        map_list.append(itemyuan)
        # 其他情况
        else:
            map_query = redis_search.query.query(const.TAG_MAP_SEARCH, cmsg.name)
            map_list =  [s["id"] for s in map_query]

        if len(map_list) == 0:
            return auth.pack(smsg, 0, cmsg.common.userid)

        maps                = map_data.get_map_set(0, map_list, "id", "name", "url", "amount", "pass")
        favorite_and_pass   = user_data.is_favourite_and_pass_map(cmsg.common.userid, map_list)
        i = 0
        for info in maps:
            map = smsg.infos.add()
            map.id         = int(info[0])
            map.name       = info[1]
            map.url        = info[2]
            map.amount     = int(info[3])
            map.pas        = int(info[4])
            map.collect    = favorite_and_pass[i * 2]
            map.finish     = favorite_and_pass[i * 2 + 1]
            i = i + 1
        return auth.pack(smsg, 0, cmsg.common.userid)
        

        
        
        