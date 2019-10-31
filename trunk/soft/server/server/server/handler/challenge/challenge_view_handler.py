# -*- coding: utf-8 -*-

import tornado.web
from constant.error import const
from constant.tag import const
from constant.game import const
from utility import auth
from utility.msg_pb2 import *
from model import user_data, map_data, cache_data, global_data

class ViewHandler(tornado.web.RequestHandler):
    @auth.authenticated
    def post(self, cmsg):
        country, index, top, life, start, version, relive = user_data.get_user_attrs(cmsg.common.userid,
                                               "country",
                                               "mission",
                                               "mission_hard",
                                               "mission_life",
                                               "mission_next",
                                               "mission_ver",
                                               "mission_relive")

        challenge_info = cache_data.get_cache(const.TAG_CHALLENGE_INFO, 300)

        if version is None or int(version) != challenge_info.get("version", 0):
            index   = 0
            top     = 0
            life    = 10
            start   = 0
            user_data.set_user_attrs(cmsg.common.userid, {"mission":0,
                                                          "mission_hard":0,
                                                          "mission_life":10,
                                                          "mission_next":0,
                                                          "mission_ver":challenge_info.get("version", 0),
                                                          "mission_total":0,
                                                          "mission_relive":0})

        # 重置(如果没有过关)
        if int(life) <= 0 and int(index) < 8:
            first_reset = (int(index) > 0) and 1 or 0
            index   = 0
            life    = 10
            start   = 0
            user_data.set_user_attrs(cmsg.common.userid, {"mission":0, "mission_hard":first_reset, "mission_life":10, "mission_next":0, "mission_relive":0, "mission_total":0})

        smsg = smsg_challenge_view()
        smsg.index  = int(index)
        smsg.life   = int(life)
        smsg.life_num = int(relive) if relive else 0
        smsg.start  = 2 if int(index) >= 8 else int(start)
        if challenge_info:
            smsg.exp    = challenge_info.get("exp") or 0
            smsg.jewel  = challenge_info.get("jewel") or 0
            smsg.date   = challenge_info.get("date") or ""
            if country == "CN":
                smsg.subject = challenge_info.get("cn_subject")
            else:
                smsg.subject = challenge_info.get("en_subject")
        return auth.pack(smsg, 0, cmsg.common.userid)