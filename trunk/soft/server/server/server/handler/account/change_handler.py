# -*- coding: utf-8 -*-

"""
切换账号处理器
"""

import tornado.web
from constant.error import const
from constant.tag import const
from constant.game import const
from utility import util, client, auth
from utility.msg_pb2 import *
from model import user_data, global_data

class ChangeHandle(tornado.web.RequestHandler):
    @auth.authenticated
    def post(self, cmsg):
        if not util.check_input(cmsg.openid, 20, True, True):
            return auth.pack(None, const.ERROR_INPUT)
        if not util.check_input(cmsg.openkey, 20, True, True):
            return auth.pack(None, const.ERROR_INPUT)

        platform = const.TAG_OPENID_TO_USERID

        userid = global_data.get_userid(platform, cmsg.openid)
        if not userid:
            return auth.pack(None, const.ERROR_NO_ACCOUNT)

        if int(userid) == cmsg.common.userid:
            return auth.pack(None, const.ERROR_HAS_LOGIN)

        user = user_data.get_user(userid)
        if not user:
            return auth.pack(None, const.ERROR_NO_ACCOUNT)

        if not util.check_passwd(cmsg.openkey, 
                                 user.get("opensalt", "fuck"), 
                                 user.get("openkey", "shit")):
            return auth.pack(None, const.ERROR_PASSWD)

        # 用户登录
        user_data.login(userid,
                        user, 
                        self.application.static_data, 0)

        global_data.log(const.TAG_INFO_LOG, od = cmsg.openid, ud = userid, op = 2, 
                                pt = cmsg.channel, dt = util.now_time())

        smsg = smsg_login()
        smsg.openid         = cmsg.openid
        smsg.openkey        = cmsg.openkey
        smsg.sig            = user.get("sig")
        smsg.userid         = int(userid)
        smsg.nationality    = user.get("country")
        smsg.name           = user.get("name")
        smsg.visitor        = int(user.get("visitor"))
        smsg.head           = int(user.get("head"))
        smsg.level          = user.get("level")
        smsg.exp            = user.get("exp")
        smsg.jewel          = user.get("jewel")
        smsg.life           = user.get("life")
        smsg.life_time      = user.get("life_time")
        smsg.upload         = user.get("upload")
        smsg.testify        = int(user.get("support"))
        smsg.exp_time       = long(user.get("exp_time"))
        smsg.guide          = int(user.get("guide"))
        smsg.mapid          = user.get("test_mapid")
        smsg.support        = user.get("test_maphard")
        smsg.server_time    = util.now_time()
        smsg.review         = 0
        smsg.notify_uri     = self.application.notify_addr + cmsg.channel
        smsg.init_life      = const.LIFE_MAX 
        smsg.life_per_time  = const.LIEF_TIME / 1000
        smsg.challenge_start = 1
        smsg.download_num    = user.get("download_num")
        smsg.download_max    = const.DOWNLOAD_MAX
        return auth.pack(smsg)