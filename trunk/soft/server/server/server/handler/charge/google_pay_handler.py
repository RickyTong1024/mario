# -*- coding: utf-8 -*-

import logging
import json
import urllib
import tornado.web
import tornado.gen
from handler import async_handler
from tornado.httpclient import AsyncHTTPClient, HTTPRequest
from constant.error import const
from constant.tag import const
from constant.game import const
from utility import util
from utility.msg_pb2 import *
from model import user_data

LOGGER = logging.getLogger(__name__)

class GooglePayHandler(async_handler.AsyncHandler):
    @tornado.web.asynchronous
    @tornado.gen.coroutine
    def post(self):
        cmsg = yield self.parse()
        if not cmsg:
            return

        shop_template = self.application.static_data.get_shop_template(cmsg.id)
        if not shop_template:
            self.response(const.ERROR_PAY)
            return

        res = yield self.verify(cmsg.common.userid,
                                cmsg.id,
                                cmsg.product_id,
                                cmsg.package_name,
                                cmsg.purchase_token,
                                shop_template)
        openid = user_data.get_user_attr(cmsg.common.userid, "openid") or "empty"
        try:
            self.application.db.insert("insert into pay_t (id, openid, userid, itemid, orderid, amount, res, pt, dt) values (0, %s, %s, %s, %s, %s, %s, %s, NOW())",
                                    openid, cmsg.common.userid, cmsg.id, '0', shop_template.arg * 10, res, 'GOOGLE')
        except Exception, e:
            LOGGER.error("google@%s", e)

        if res != 0:
            self.response(res)
        else:
            self.response(res, None, cmsg.common.userid)


    @tornado.gen.coroutine
    def verify(self, userid, bill_id, product_id, package_name, purchase_token, shop_template):
        url = 'http://23.252.167.50:8070/google_pay'
        params = {"bill_id":bill_id,
                  "packageName":package_name,
                  "productId":product_id,
                  "purchase_token":purchase_token}
        url += "?" + urllib.urlencode(params)
        LOGGER.info(url)
        http_client = AsyncHTTPClient()
        try:
            respone = yield http_client.fetch(url, method = 'GET')
        except Exception, e:
            LOGGER.error("google@%s", e)
            raise tornado.gen.Return(const.ERROR_PAY)

        if respone.error:
            raise tornado.gen.Return(const.ERROR_PAY)

        if respone.body != "ok":
            raise tornado.gen.Return(const.ERROR_PAY)

        user_data.inc_user_attr(userid, "jewel", shop_template.arg)
        raise tornado.gen.Return(0)

    @tornado.gen.coroutine
    def get_token(self):
        pass