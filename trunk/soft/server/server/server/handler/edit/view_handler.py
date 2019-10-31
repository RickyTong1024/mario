# -*- coding: utf-8 -*-

'''查看编辑地图列表信息'''

import tornado.web
from constant.error import const
from constant.tag import const
from constant.game import const
from utility import auth
from utility.msg_pb2 import *
from model import user_data

class ViewAllHandler(tornado.web.RequestHandler):
    @auth.authenticated
    def post(self, cmsg):
        exp, level = user_data.get_user_attrs(cmsg.common.userid, "master_exp", "master_level")
        if exp is None or level is None:
            return auth.pack(None, const.ERROR_SYSTEM)

        exp             = int(exp)
        level           = int(level)
        before_level    = level
        exp, level = self._check_levelup(exp, level)
        if level != before_level:
            user_data.set_user_attrs(cmsg.common.userid, {"master_exp":exp,
                                                          "master_level":level})

        smsg = smsg_view_edit()
        smsg.exp    = exp
        smsg.level  = level
        map_list = user_data.get_usermap_set_with_script(cmsg.common.userid, 
                                                         self.application.get_usermap_script)
        if not map_list:
            for i in range(12):
                data = smsg.infos.add()
                data.id = 0
                data.name = ""
                data.url = ""
                data.date = ""
                data.upload = 0
        else:
            for map_data in map_list:
                data = smsg.infos.add()
                if map_data:
                    data.id     = int(map_data[0])
                    data.name   = map_data[1]
                    data.url    = map_data[2]
                    data.date   = map_data[3]
                    data.upload = int(map_data[4])
                else:
                    data.id = 0
                    data.name = ""
                    data.url = ""
                    data.date = ""
                    data.upload = 0
        
        return auth.pack(smsg, 0, cmsg.common.userid)


    def _check_levelup(self, master_exp, master_level):
        next_level = master_level + 1

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


class ViewSingleHandler(tornado.web.RequestHandler):
    @auth.authenticated
    def post(self, cmsg):
        mapattrs = user_data.get_usermap_attrs(cmsg.common.userid, cmsg.map_id, "id", "name", "url", "date", "upload")
        if not mapattrs:
            return auth.pack(None, const.ERROR_SYSTEM)

        smsg = smsg_view_edit_single()
        smsg.info.id        = int(mapattrs[0])
        smsg.info.name      = mapattrs[1]
        smsg.info.url       = mapattrs[2]
        smsg.info.date      = mapattrs[3]
        smsg.info.upload    = int(mapattrs[4])
        return auth.pack(smsg, 0, cmsg.common.userid)
