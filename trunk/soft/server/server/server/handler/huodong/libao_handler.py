# -*- coding: utf-8 -*-

import tornado.web
from constant.error import const
from constant.tag import const
from utility import auth, util
from utility.msg_pb2 import *
from model import user_data, global_data

class LibaoHandler(tornado.web.RequestHandler):
    @auth.authenticated
    def post(self, cmsg):
        if not util.check_input(cmsg.code, 16, True, True):
            return auth.pack(None, const.ERROR_INPUT)

        try:
            libao_pici = cmsg.code[:2]
            libao_type = cmsg.code[2:3]
            libao_reward = cmsg.code[3:5]
        except:
            return auth.pack(None, const.ERROR_LIBAO)

        if libao_type == '0':
            if user_data.has_libao(cmsg.common.userid, cmsg.code):
                return auth.pack(None, const.ERROR_LIBAO)

            key = const.TAG_GAME_LIBAO + libao_pici
            if not global_data.has_libao(key, cmsg.code):
                return auth.pack(None, const.ERROR_LIBAO)

        else:
            if user_data.has_libao(cmsg.common.userid, libao_pici):
                return auth.pack(None, const.ERROR_LIBAO)

            key = const.TAG_GAME_LIBAO + libao_pici
            if not global_data.has_libao(key, cmsg.code):
                return auth.pack(None, const.ERROR_LIBAO)

        reward = global_data.get_libao_reward(const.TAG_GAME_LIBAO_REWARD, libao_reward)
        life = reward.get("life", 0) if reward else 0
        if life > 0:
            user_data.inc_user_attr(cmsg.common.userid, "life", life)

        if libao_type == '0':
            user_data.add_libao(cmsg.common.userid, cmsg.code)
        else:
            user_data.add_libao(cmsg.common.userid, libao_pici)
            key = const.TAG_GAME_LIBAO + libao_pici
            global_data.remove_libao(key, cmsg.code)

        smsg = smsg_libao()
        smsg.life = life
        return auth.pack(smsg, 0, cmsg.common.userid)

