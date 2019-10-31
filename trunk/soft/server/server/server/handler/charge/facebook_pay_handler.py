# -*- coding: utf-8 -*-

import logging
import json
import urllib
import base64
import hmac
import hashlib
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

class FacebookPayHandler(async_handler.AsyncHandler):
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
                                cmsg.signed_request,
                                shop_template)

        if res != 0:
            self.response(res)
        else:
            self.response(res, None, cmsg.common.userid)


    @tornado.gen.coroutine
    def verify(self, userid, signed_request, shop_template):
        LOGGER.info(signed_request)

        try:
            encode_sig, payload = signed_request.split(".", 2)
            sig = util.base64_url_decode(encode_sig)
            expected_sig = hmac.new("hhha", payload, hashlib.sha256).hexdigest()
            data = json.loads(util.base64_url_decode(payload))
        except Exception, e:
            LOGGER.error("facebook@%s", e)
            raise tornado.gen.Return(const.ERROR_PAY)

        LOGGER.info(data)

        if sig != expected_sig:
            LOGGER.error("facebook@wrong sig")
            raise tornado.gen.Return(const.ERROR_PAY)

        if data.get("status", None) != "completed":
            LOGGER.error("facebook@payment is not completed")
            raise tornado.gen.Return(const.ERROR_PAY)

        paymentid = data.get("payment_id", None)
        if paymentid is None:
            LOGGER.error("facebook@paymentid is invalid")
            raise tornado.gen.Return(const.ERROR_PAY)

        url = 'http://23.252.167.50:8060/facebook_pay'
        params = {"payment_id":paymentid,
                  "userid":userid}
        url += "?" + urllib.urlencode(params)
        http_client = AsyncHTTPClient()
        try:
            respone = yield http_client.fetch(url, method = 'GET')
        except Exception, e:
            LOGGER.error("facebook@%s", e)
            raise tornado.gen.Return(const.ERROR_PAY)

        if respone.error:
            raise tornado.gen.Return(const.ERROR_PAY)

        if respone.body != "ok":
            raise tornado.gen.Return(const.ERROR_PAY)

        is_duplicate = True
        try:
            duplicate_order = self.application.db.get("select id from pay_t where orderid = %s and pt = 'FACEBOOK'", paymentid)
            if not duplicate_order:
                is_duplicate = False
                openid = user_data.get_user_attr(userid, "openid") or "empty"
                self.application.db.insert("insert into pay_t (id, openid, userid, itemid, orderid, amount, res, pt, dt) values (0, %s, %s, %s, %s, %s, %s, %s, NOW())",
                                        openid, userid, shop_template.id, paymentid, shop_template.arg * 10, 0, 'FACEBOOK')
        except Exception, e:
            LOGGER.error("facebook@%s", e)
            raise tornado.gen.Return(const.ERROR_PAY)

        if is_duplicate:
            LOGGER.error("facebook@duplicate order")
            raise tornado.gen.Return(const.ERROR_PAY)

        user_data.inc_user_attr(userid, "jewel", shop_template.arg)
        raise tornado.gen.Return(0)