# -*- coding: utf-8 -*-

"""
登录处理器
"""

import tornado.web
from constant.error import const
from constant.tag import const
from constant.game import const
from utility import util, client, auth
from utility.msg_pb2 import *
from model import user_data, global_data, cache_data
import datetime

class LoginHandle(tornado.web.RequestHandler):
    @auth.authenticated
    def post(self, cmsg):
        platform = const.TAG_OPENID_TO_USERID
        notify_addr = self.application.notify_addr + cmsg.channel

        # TODO 读写分离
        userid, review, invalid =  global_data.get_userid_and_version(platform, cmsg.openid, cmsg.ver)
        can_login = 0
        if invalid:
            can_login = -1
        elif review:
            can_login = 1

        if userid:
            user = user_data.get_user(userid)                                                            
            if user:
                # 验证密码
                if util.check_passwd(cmsg.openkey, 
                                     user.get("opensalt", "fuck"), 
                                     user.get("openkey", "shit")):

                    # 检查充值
                    jewel = 0
                    try:
                        rechargs = self.application.db.query("select id, amount from pay_t where userid = 0 and openid = %s and pt = %s", cmsg.openid, cmsg.channel)
                        for recharge in rechargs:
                            jewel += recharge["amount"] / 10
                            self.application.db.update("update pay_t set userid = %s where id = %s", userid, recharge["id"])
                    except:
                        pass

                    # 用户登录
                    user_data.login(userid,                                                             
                                    user, 
                                    self.application.static_data, jewel)

                    # 登录日志
                    global_data.log(const.TAG_INFO_LOG, od = cmsg.openid, ud = userid, op = 2,         
                                pt = cmsg.channel, dt = util.now_time())


                    challenge_info = cache_data.get_cache(const.TAG_CHALLENGE_INFO, 1)
                    challenge_version = user.get("mission_ver", None)
                    challenge_start = 0
                    if challenge_version is None or int(challenge_version) != challenge_info.get("version", 0):
                        challenge_start = 1

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
                    smsg.review         = can_login
                    smsg.notify_uri     = notify_addr
                    smsg.init_life      = const.LIFE_MAX 
                    smsg.life_per_time  = const.LIEF_TIME / 1000
                    smsg.challenge_start = challenge_start
                    smsg.download_num    = user.get("download_num")
                    smsg.download_max    = const.DOWNLOAD_MAX
                    return auth.pack(smsg)

        # 创建游客
        userid = global_data.get_counter(const.TAG_USER_COUNT)                                           
        global_data.set_userid(platform,                                                                
                               const.YOUKE_ACCOUNT.format(userid),
                               userid)
        sig = user_data.create_visitor(userid,                                                         
                                       cmsg.nationality, 
                                       cmsg.channel)

        # 创角日志
        global_data.log(const.TAG_INFO_LOG, od = const.YOUKE_ACCOUNT.format(userid), ud = userid, op = 1,
                        pt = cmsg.channel, dt = util.now_time())

        smsg = smsg_login()
        smsg.openid         = const.YOUKE_ACCOUNT.format(userid)
        smsg.openkey        = "empty"
        smsg.sig            = sig
        smsg.userid         = userid
        smsg.nationality    = cmsg.nationality
        smsg.name           = "empty"
        smsg.visitor        = 1
        smsg.head           = 0
        smsg.level          = 1
        smsg.exp            = 0
        smsg.jewel          = 0
        smsg.life           = 20
        smsg.life_time      = 0
        smsg.upload         = 0
        smsg.testify        = 0
        smsg.exp_time       = 0
        smsg.guide          = 0
        smsg.mapid          = 10000001
        smsg.support        = 1
        smsg.server_time    = util.now_time()
        smsg.review         = can_login
        smsg.notify_uri     = notify_addr
        smsg.init_life      = const.LIFE_MAX 
        smsg.life_per_time  = const.LIEF_TIME / 1000
        smsg.challenge_start = 1
        smsg.download_num    = 0
        smsg.download_max    = const.DOWNLOAD_MAX
        return auth.pack(smsg)


        

            
