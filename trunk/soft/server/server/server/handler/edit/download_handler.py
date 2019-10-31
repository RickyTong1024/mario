# -*- coding: utf-8 -*-

import tornado.web
from constant.error import const
from constant.tag import const
from constant.game import const
from utility import util, auth
from utility.msg_pb2 import *
from model import user_data, global_data, map_data


class DownloadHandler(tornado.web.RequestHandler):
    @auth.authenticated
    def post(self, cmsg):
        jewel, download_num = user_data.get_user_attrs(cmsg.common.userid, "jewel", "download_num")
        if download_num is None:
            download_num = 0

        name, ownerid, url, data = map_data.get_map_attrs(cmsg.id, "name", "ownerid", "url", "data")

        download_num = int(download_num)
        if download_num >= const.DOWNLOAD_MAX:
            return auth.pack(None, const.ERROR_SYSTEM)

        download_template = self.application.static_data.get_download_template(download_num)
        if not download_template:
            return auth.pack(None, const.ERROR_SYSTEM)

        need_guild = 0
        need_jewel = -download_template.price
        if int(ownerid) != cmsg.common.userid:
            need_guild = 1
            if int(jewel) < download_template.price:
                return auth.pack(None, const.ERROR_NO_JEWEL)
        else:
            need_jewel = 0


        empty_slot = -1
        edit_map_list = user_data.get_usermap_list(cmsg.common.userid)
        if not edit_map_list:
            user_data.create_usermap_list(cmsg.common.userid)
            empty_slot = 1
        else:
            for i in range(len(edit_map_list)):
                if edit_map_list[i] == '0':
                    empty_slot = i
                    break

        if empty_slot == -1:
            return auth.pack(None, const.ERROR_DOWNLOAD)

        mapid = global_data.get_counter(const.TAG_USERMAP_COUNT)
        user_data.create_usermap(cmsg.common.userid, mapid, empty_slot, name, url, data, need_guild, need_jewel)

        return auth.pack(None, 0, cmsg.common.userid)