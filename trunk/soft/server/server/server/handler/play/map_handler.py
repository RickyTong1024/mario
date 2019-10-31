# -*- coding: utf-8 -*-

import tornado.web
from constant.error import const
from constant.tag import const
from constant.game import const
from utility import util, client, auth
from utility.msg_pb2 import *
from model import user_data, global_data, map_data


class ViewMapHandler(tornado.web.RequestHandler):
    @auth.authenticated
    def post(self, cmsg):

        start   = cmsg.index * 10
        end     = cmsg.index * 10 + 9
        
        userid = cmsg.common.userid
        smsg = None

        # 最受欢迎
        if cmsg.type == const.MAP_MOST_POPULAR:
            smsg = self._get_global_map(const.TAG_RANK_MOST_POPULAR_EX, userid, start, end, True, cmsg.ver)
        # 最新上传
        elif cmsg.type == const.MAP_NEWER_UPLOAD:
            smsg = self._get_global_map(const.TAG_RANK_NEWER_UPLOAD, userid, start, end, True, cmsg.ver)
        # 近期热门
        elif cmsg.type == const.MAP_RECENT_POPULAR:
            smsg = self._get_global_map(const.TAG_RANK_RECENT_POPULAR, userid, start, end, True, cmsg.ver)
        # 初级难度
        elif cmsg.type == const.MAP_PRIMARY_HARD:
            smsg = self._get_global_map(const.TAG_RANK_PRIMARY_HARD, userid, start, end, False, cmsg.ver)
        # 中级难度
        elif cmsg.type == const.MAP_MIDDLE_HARD:
            smsg = self._get_global_map(const.TAG_RANK_MIDDLE_HARD, userid, start, end, False, cmsg.ver)
        # 高级难度
        elif cmsg.type == const.MAP_SENIOR_HARD:
            smsg = self._get_global_map(const.TAG_RANK_SENIOR_HARD, userid, start, end, False, cmsg.ver)
        # 大师难度
        elif cmsg.type == const.MAP_MASTER_HARD:
            smsg = self._get_global_map(const.TAG_RANK_MASTER_HARD, userid, start, end, False, cmsg.ver)

        # 我收藏的
        elif cmsg.type == const.MAP_MY_FAFOURITE:
            smsg = self._get_user_map(const.TAG_USER_FAFOURITE.format(cmsg.common.userid), userid,  start, end, False)
        # 我上传的
        elif cmsg.type == const.MAP_MY_UPLOAD:
            smsg = self._get_user_map(const.TAG_USER_UPLOAD.format(cmsg.common.userid), userid, start, end, True)
        # 我最近玩的
        elif cmsg.type == const.MAP_MY_RECENT_PLAY:
            smsg = self._get_user_map(const.TAG_USER_TIME.format(cmsg.common.userid), userid, start, end, True)
        else:
            return auth.pack(None, const.ERROR_SYSTEM)
        return auth.pack(smsg, 0, cmsg.common.userid)

    def _get_global_map(self, key, userid, start, end, desc = False, ver = 2):
        map_num, map_set =  map_data.get_map_set_ex(key,
                                                       self.application.get_map_script,
                                                       start,
                                                       end,
                                                       desc,
                                                       ver)
        map_list = [temp[0] for temp in map_set]
        favorite_and_pass   = user_data.is_favourite_and_pass_map(userid, map_list)

        smsg = smsg_view_map()
        smsg.page = (map_num - 1) / 10 if map_num else map_num
        i = 0
        for info in map_set:
            map = smsg.infos.add()
            map.id         = int(info[0])
            map.name       = info[1]
            map.url        = info[2]
            map.amount     = int(info[3])
            map.pas        = int(info[4])
            map.collect    = favorite_and_pass[i * 2]
            map.finish     = favorite_and_pass[i * 2 + 1]
            map.difficulty = int(info[6])
            map.like       = int(info[7]) if info[7] else 0
            i = i + 1
        return smsg

    def _get_user_map(self, key, userid, start, end, desc = False):
        map_num, map_list   = user_data.get_range_map_with_length(userid, key, start, end, desc)
        maps                = map_data.get_map_set(key, map_list, "id", "name", "url", "amount", "pass", "template", "hard", "like")
        favorite_and_pass   = user_data.is_favourite_and_pass_map(userid, map_list)

        smsg = smsg_view_map()
        smsg.page = (map_num - 1) / 10 if map_num else map_num
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
            map.difficulty = int(info[6])
            map.like       = int(info[7]) if info[7] else 0
            i = i + 1
        return smsg




        

        



            
                                     
