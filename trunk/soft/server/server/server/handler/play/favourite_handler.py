# -*- coding: utf-8 -*-


import tornado.web
from constant.error import const
from constant.tag import const
from constant.game import const
from utility import util, client, auth
from utility.msg_pb2 import *
from model import user_data, global_data, map_data

class FavouriteMapHandler(tornado.web.RequestHandler):
    @auth.authenticated
    def post(self, cmsg): 
        smsg = smsg_favorite_map()
        if not user_data.is_favourite_map(cmsg.common.userid, cmsg.id):
            smsg.num = map_data.inc_map_attr(cmsg.id, "favorite", 1)
            user_data.favourite_map(cmsg.common.userid, cmsg.id, False)
        else:
            smsg.num = map_data.inc_map_attr(cmsg.id, "favorite", -1)
            user_data.favourite_map(cmsg.common.userid, cmsg.id, True)

        return auth.pack(smsg, 0, cmsg.common.userid)

        