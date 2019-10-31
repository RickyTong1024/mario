# -*- coding: utf-8 -*-


import tornado.web
from constant.error import const
from constant.tag import const
from constant.game import const
from utility import util, auth
from utility.msg_pb2 import *
from model import user_data, global_data

class CreateMapHandler(tornado.web.RequestHandler):
    @auth.authenticated
    def post(self, cmsg):
        if cmsg.index < 0 or cmsg.index >= 12:
            return auth.pack(None, const.ERROR_SYSTEM)

        edit_map_list = user_data.get_usermap_list(cmsg.common.userid)
        if not edit_map_list:
            user_data.create_usermap_list(cmsg.common.userid)
        else:
            if edit_map_list[cmsg.index] != '0':
                return auth.pack(None, const.ERROR_SYSTEM)

        mapid = global_data.get_counter(const.TAG_USERMAP_COUNT)
        user_data.create_usermap(cmsg.common.userid, mapid, cmsg.index)

        smsg = smsg_create_map()
        smsg.map.id         = mapid
        smsg.map.name       = "empty"
        smsg.map.url        = ""
        smsg.map.date       = util.now_date_str()
        smsg.map.upload     = 0
        return auth.pack(smsg, 0, cmsg.common.userid)
