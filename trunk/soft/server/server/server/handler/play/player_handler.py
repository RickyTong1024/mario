# -*- coding: utf-8 -*-

import tornado.web
from constant.error import const
from constant.tag import const
from constant.game import const
from utility import util, auth
from utility.msg_pb2 import *
from model import user_data, map_data


class ViewPlayerHandler(tornado.web.RequestHandler):
    @auth.authenticated
    def post(self, cmsg):
        if cmsg.userid == 0:
            return auth.pack(None, const.ERROR_ADMINISTRTOR)

        
        user = user_data.get_user_attrs(cmsg.userid, "name", "country", 
                                            "head", "register", "level", "exp", "play", "pass",
                                            "point", "comment", "video", "visitor", "watched", "master_exp", "master_level")

        smsg = smsg_view_player()
        self._get_recent_map(cmsg.userid, smsg)
        self._get_upload_map(cmsg.userid, smsg)
        self._get_play_map(cmsg.userid, smsg)

        smsg.data.userid    = cmsg.userid
        smsg.data.name      = user[0]
        smsg.data.country   = user[1]
        smsg.data.head      = int(user[2])
        smsg.data.level     = int(user[4])
        smsg.data.exp       = int(user[5])
        smsg.data.register  = user[3]
        smsg.data.amount    = int(user[6])
        smsg.data.pas       = int(user[7])
        smsg.data.point     = int(user[8])
        smsg.data.comment   = int(user[9])
        smsg.data.video     = int(user[10])
        smsg.data.visitor   = int(user[11])
        smsg.data.watched   = int(user[12])
        smsg.data.mlevel    = int(user[14])
        smsg.data.mexp      = int(user[13])
        return auth.pack(smsg, 0, cmsg.common.userid)
       
    def _get_recent_map(self, userid, smsg):
        recent_map_ids = user_data.get_recent_map_list(userid)
        map_list = []
        map_recentt = []
        for recent in recent_map_ids:
            t, r, m = recent.split(":")
            map_list.append(int(m))
            map_recentt.append((long(t), int(r)))

        maps = map_data.get_map_set(0, map_list, "id", "name", "url")

        i = 0
        for map in maps:
            map_recent = smsg.recent.add()
            map_recent.id     = int(map[0])
            map_recent.name   = map[1]
            map_recent.url    = map[2]
            map_recent.time   = util.msc_to_str(map_recentt[i][0])
            map_recent.rank   = map_recentt[i][1]
            i = i + 1

    def _get_upload_map(self, userid, smsg):
        upload_map_ids = user_data.get_range_map_list_with_score(userid, const.TAG_USER_UPLOAD.format(userid), 0, 4, True)
        map_list = [item[0] for item in upload_map_ids]
        maps = map_data.get_map_set(0, map_list, "id", "name", "url")
        i = 0
        for map in maps:
            map_upload = smsg.upload.add()
            map_upload.id = int(map[0])
            map_upload.name = map[1]
            map_upload.url = map[2]
            map_upload.time = util.msc_to_str(upload_map_ids[i][1])
            i = i + 1


    def _get_play_map(self, userid, smsg):
        amount_map_ids = user_data.get_range_map_list_with_score(userid, const.TAG_USER_AMOUNT.format(userid), 0, 4, True)
        map_list = [item[0] for item in amount_map_ids]
        maps = map_data.get_map_set(0, map_list, "id", "name", "url")
        i = 0
        for map in maps:
            map_amount = smsg.play.add()
            map_amount.id = int(map[0])
            map_amount.name = map[1]
            map_amount.url = map[2]
            map_amount.play = int(amount_map_ids[i][1])
            i = i + 1