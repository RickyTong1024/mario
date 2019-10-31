# -*- coding: utf-8 -*-


import tornado.web
from constant.error import const
from constant.tag import const
from constant.game import const
from utility import auth
from utility.msg_pb2 import *
from model import user_data, map_data

class ViewVideoHandler(tornado.web.RequestHandler):
    @auth.authenticated
    def post(self, cmsg):
        video_data = ""
        if cmsg.video_id == cmsg.common.userid:
            video_data = user_data.watch_video_map(cmsg.video_id, cmsg.map_id, True)
        else:
            user_data.inc_user_attr(cmsg.common.userid, "video", 1)
            video_data = user_data.watch_video_map(cmsg.video_id, cmsg.map_id, False)
        
        smsg = smsg_view_video()
        smsg.video_data = video_data if video_data else ""
        smsg.map_data   = map_data.get_map_attr(cmsg.map_id, "data") or ""
        return auth.pack(smsg, 0, cmsg.common.userid)