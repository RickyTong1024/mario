# -*- coding: utf-8 -*-

import logging
import tornado.web
from constant.error import const
from constant.tag import const
from constant.game import const
from utility import util, auth
from utility.msg_pb2 import *
from model import user_data


LOGGER = logging.getLogger(__name__)

class PayHandler(tornado.web.RequestHandler):
    @auth.authenticated
    def post(self, cmsg):
        openid = user_data.get_user_attr(cmsg.common.userid, "openid")
        if openid is None:
            return auth.pack(None, const.ERROR_SYSTEM)

        jewel = 0
        try:
            rechargs = self.application.db.query("select id, amount from pay_t where userid = 0 and openid = %s and pt = %s", openid, cmsg.channel)
            for recharge in rechargs:
                jewel += recharge["amount"] / 10
                self.application.db.update("update pay_t set userid = %s where id = %s", cmsg.common.userid, recharge["id"])
        except Exception, e:
            LOGGER.error("%s@%s", cmsg.channel, e)
            return auth.pack(None, const.ERROR_PAY)

        if jewel == 0:
            return auth.pack(None, -1, cmsg.common.userid)

        user_data.inc_user_attr(cmsg.common.userid, "jewel", jewel)
        
        smsg = smsg_pay()
        smsg.jewel = jewel
        return auth.pack(smsg, 0, cmsg.common.userid)


