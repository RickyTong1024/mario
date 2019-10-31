# -*- coding: utf-8 -*-

import tornado.web
from constant.error import const
from constant.tag import const
from constant.game import const
from utility import util, auth
from utility.msg_pb2 import *
from model import user_data, global_data, map_data

class UploadHandler(tornado.web.RequestHandler):
    @auth.authenticated
    def post(self, cmsg):
        user = user_data.get_user_attrs(cmsg.common.userid, "head", "name", "country", "master_exp", "master_level", "upload")
        user = filter(None, user)
        if not user or len(user) != 6:
            return auth.pack(None, const.ERROR_SYSTEM)

        master_level    = int(user[4])
        master_exp      = int(user[3])
        upload          = int(user[5])

        master_template = self.application.static_data.get_master_template(master_level)
        if not master_template:
            return auth.pack(None, const.ERROR_SYSTEM)

        if upload >= master_template.upload:
            return auth.pack(None, const.ERROR_SYSTEM)

        usermap = user_data.get_usermap_attrs(cmsg.common.userid, cmsg.id, "name", "url", "data", "upload")
        usermap = filter(None, usermap)
        if not usermap or len(usermap) != 4:
            return auth.pack(None, const.ERROR_SYSTEM)
        if usermap[3] != '0':
            return auth.pack(None, const.ERROR_SYSTEM)

        master_exp, master_level = self._check_levelup(master_exp, master_level)

        # 上传录像
        uptime = None
        upvideo = ""
        if cmsg.video != "":
            upvideo = cmsg.video
            uptime = cmsg.time

        mapid = global_data.get_counter(const.TAG_MAP_COUNT)
        map_data.upload_map(mapid, usermap[0], usermap[1],
                            usermap[2],
                            cmsg.common.userid,
                            user[1],
                            user[2],
                            user[0],
                            cmsg.ver, uptime)
        user_data.upload_usermap(cmsg.common.userid, 
                                 cmsg.id, 
                                 mapid, 
                                 master_exp, 
                                 master_level,
                                 upvideo)

        self._index_map(mapid, usermap[0])

        return auth.pack(None, 0, cmsg.common.userid)

    def _check_levelup(self, master_exp, master_level):
        next_level = master_level + 1
        master_exp = master_exp + 100

        while True:
            master_template = self.application.static_data.get_master_template(next_level)
            if not master_template:
                break
            if master_exp >= master_template.exp:
                master_exp -= master_template.exp
                master_level += 1
                next_level += 1
            else:
                break

        return (master_exp, master_level)


    def _index_map(self, id, name):
        import platform
        if platform.system() == "Windows":
            return

        import redis_search.index
        map_index = redis_search.index.index(const.TAG_MAP_SEARCH, id, name)
        map_index.save()
