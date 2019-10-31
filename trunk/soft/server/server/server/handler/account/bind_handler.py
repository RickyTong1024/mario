# -*- coding: utf-8 -*-

"""
绑定账号处理器
"""

from handler import thread_handler
from constant.error import const
from constant.tag import const
from constant.game import const
from utility import util, client, auth
from utility.msg_pb2 import *
from model import user_data, global_data


class BindHandle(thread_handler.ThreadHandler):
    def _handle_msg(self, cmsg):
        if not util.check_input(cmsg.openid, 20, True, True):
            return auth.pack(None, const.ERROR_INPUT)

        if not util.check_input(cmsg.openkey, 20, True, True):
            return auth.pack(None, const.ERROR_INPUT)

        if not util.check_input_nospace(cmsg.nickname, 16):
            return auth.pack(None, const.ERROR_INPUT)

        user = user_data.get_user(cmsg.common.userid)
        if not user or user.get("visitor") == '0':
            return auth.pack(None, const.ERROR_SYSTEM)
        
        lock_name = const.TAG_OPENID_TO_USERID + ":" + cmsg.openid
        lock_id = global_data.acquire_lock(0, lock_name, self.application.lock_script)
        if not lock_id:
            return auth.pack(None, const.ERROR_HAS_BIND)

        if global_data.get_userid(const.TAG_OPENID_TO_USERID, cmsg.openid):
            global_data.release_lock(0, lock_name, lock_id, self.application.unlock_script)
            return auth.pack(None, const.ERROR_HAS_BIND)

        # 绑定账号
        global_data.bind_openid(const.TAG_OPENID_TO_USERID, cmsg.openid, cmsg.common.userid)
        user_data.bind_account(cmsg.common.userid, 
                               cmsg.openid, 
                               cmsg.openkey, 
                               cmsg.nickname,
                               cmsg.head, 
                               cmsg.nationality)
        global_data.release_lock(0, lock_name, lock_id, self.application.unlock_script)

        map_template = self.application.static_data.get_map_template(int(user.get("test", 0)))

        smsg = smsg_login()
        smsg.openid         = cmsg.openid
        smsg.openkey        = cmsg.openkey
        smsg.sig            = cmsg.common.sig
        smsg.userid         = cmsg.common.userid
        smsg.nationality    = cmsg.nationality
        smsg.name           = cmsg.nickname
        smsg.visitor        = 0
        smsg.head           = cmsg.head
        smsg.level          = int(user.get("level"))
        smsg.exp            = int(user.get("exp"))
        smsg.jewel          = int(user.get("jewel"))
        smsg.life           = int(user.get("life"))
        smsg.life_time      = long(user.get("life_time"))
        smsg.upload         = int(user.get("upload"))
        smsg.testify        = int(user.get("support"))
        smsg.exp_time       = long(user.get("exp_time"))
        smsg.guide          = int(user.get("guide"))
        smsg.mapid          = map_template.id if map_template else -1
        smsg.support        = map_template.hard if map_template else 1
        smsg.server_time    = util.now_time()
        smsg.review         = 0
        smsg.init_life      = const.LIFE_MAX 
        smsg.life_per_time  = const.LIEF_TIME / 1000
        smsg.challenge_start = 1
        smsg.download_num    = int(user.get("download_num"))
        smsg.download_max    = const.DOWNLOAD_MAX
        return auth.pack(smsg)
        




        
