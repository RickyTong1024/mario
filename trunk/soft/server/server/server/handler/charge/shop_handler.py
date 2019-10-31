# -*- coding: utf-8 -*-

import logging
import json
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

class ShopBuyHandler(async_handler.AsyncHandler):
    @tornado.web.asynchronous
    @tornado.gen.coroutine
    def post(self):
        cmsg = yield self.parse()
        if not cmsg:
            return

        shop_template = self.application.static_data.get_shop_template(cmsg.id)
        if not shop_template:
            self.response(const.ERROR_SYSTEM)
            return

        buy_result = 0
        if shop_template.type == 1:
            buy_result = yield self.pay_apple(cmsg.receipt, False, cmsg.common.userid, shop_template)
            if buy_result != 0:
                buy_result = yield self.pay_apple(cmsg.receipt, True, cmsg.common.userid, shop_template)

            openid = user_data.get_user_attr(cmsg.common.userid, "openid") or "empty"
            try:
                self.application.db.insert("insert into pay_t (id, openid, userid, itemid, orderid, amount, res, pt, dt) values (0, %s, %s, %s, %s, %s, %s, %s, NOW())",
                                        openid, cmsg.common.userid, cmsg.id, '0', shop_template.arg * 10, buy_result, 'IOS')
            except Exception, e:
                LOGGER.error("apple@%s", e)
            if buy_result != 0:
                buy_result = const.ERROR_PAY
        elif shop_template.type == 2:
            buy_result = yield self.buy_life(cmsg.common.userid, shop_template)
        elif shop_template.type == 3:
            buy_result = yield self.buy_support(cmsg.common.userid, shop_template)
        else:
            buy_result = yield self.buy_exp(cmsg.common.userid, shop_template)

        self.response(buy_result, None, cmsg.common.userid)

    @tornado.gen.coroutine
    def buy_life(self, userid, shop_template):
        jewel = user_data.get_user_attr(userid, "jewel")
        if not jewel or int(jewel) < shop_template.price:
            raise tornado.gen.Return(const.ERROR_NO_JEWEL)

        user_data.inc_user_attrs(userid, [("jewel", -(shop_template.price)), ("life", shop_template.arg)])
        raise tornado.gen.Return(0)

    @tornado.gen.coroutine
    def buy_support(self, userid, shop_template):
        jewel, support = user_data.get_user_attrs(userid, "jewel", "support")
        if not jewel or not support or int(jewel) < shop_template.price or int(support) >= shop_template.arg:
            raise tornado.gen.Return(const.ERROR_NO_JEWEL)
             
        user_data.inc_user_attr(userid, "jewel", -(shop_template.price))
        user_data.set_user_attr(userid, "support", shop_template.arg)
        raise tornado.gen.Return(0)

    @tornado.gen.coroutine
    def buy_exp(self, userid, shop_template):
        jewel, exp_time = user_data.get_user_attrs(userid, "jewel", "exp_time")
        if not jewel or not exp_time or int(jewel) < shop_template.price:
            raise tornado.gen.Return(const.ERROR_NO_JEWEL)

        exp_next_time = long(exp_time)
        if exp_next_time < util.now_time():
            exp_next_time = util.now_time() + shop_template.arg * 24 * 60 * 60 * 1000
        else:
            exp_next_time = exp_next_time + shop_template.arg * 24 * 60 * 60 * 1000

        user_data.inc_user_attr(userid, "jewel", -(shop_template.price))
        user_data.set_user_attr(userid, "exp_time", exp_next_time)
        raise tornado.gen.Return(0)

    @tornado.gen.coroutine
    def pay_apple(self, receipt, issandbox, userid, shop_template):
        headers = {"Content-type": "application/json"}  
        body = json.dumps({"receipt-data":receipt})
        http_client = AsyncHTTPClient()
        url = "https://sandbox.itunes.apple.com/verifyReceipt" if issandbox else "https://buy.itunes.apple.com/verifyReceipt"
        http_request = HTTPRequest(url, method = 'POST', headers = headers, body = body)
        try:
            respone = yield http_client.fetch(http_request)
        except Exception, e:
            LOGGER.error("apple@%s", e)
            raise tornado.gen.Return(const.ERROR_PAY)

        try:
            data = json.loads(respone.body)
        except:
            raise tornado.gen.Return(const.ERROR_PAY)

        LOGGER.info(data)

        if data.get("status") == 0:
            user_data.inc_user_attr(userid, "jewel", shop_template.arg)

        raise tornado.gen.Return(data.get("status"))
