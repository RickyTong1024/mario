# -*- coding: utf-8 -*-


import tornado.web
from constant.error import const
from constant.tag import const
from constant.game import const
from utility import util, client, auth
from utility.msg_pb2 import *
from model import user_data, global_data, map_data

class MapLikeHandler(tornado.web.RequestHandler):
    @auth.authenticated
    def post(self, cmsg):
        mapid = user_data.get_user_attr(cmsg.common.userid, "mapid") 
        if user_data.like_pass_map(cmsg.common.userid, mapid, self.application.like_script) == 1:
            map_data.inc_map_attr(mapid, "like", 1)
            
        return auth.pack(None, 0, cmsg.common.userid)