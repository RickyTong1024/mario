# -*- coding: utf-8 -*-

import logging
import urllib
import tornado.web
import tornado.gen
from tornado import escape
from tornado.httpclient import AsyncHTTPClient
import base_handler
import util

LOGGER = logging.getLogger(__name__)

class NotifyHandler(base_handler.BaseHandler):
    @tornado.web.asynchronous
    @tornado.gen.coroutine
    def get(self):
        all_args = {}
        all_args["app_key"]         = self.get_argument("app_key", None)
        all_args["product_id"]      = self.get_argument("product_id", None)
        all_args["amount"]          = self.get_argument("amount", None)
        all_args["app_uid"]         = self.get_argument("app_uid", None)
        all_args["app_ext1"]        = self.get_argument("app_ext1", None)
        all_args["app_ext2"]        = self.get_argument("app_ext2", None)
        all_args["user_id"]         = self.get_argument("user_id", None)
        all_args["order_id"]        = self.get_argument("order_id", None)
        all_args["gateway_flag"]    = self.get_argument("gateway_flag", None)
        all_args["sign_type"]       = self.get_argument("sign_type", None)
        all_args["app_order_id"]    = self.get_argument("app_order_id", None)
        sign_return                 = self.get_argument("sign_return", None)
        sign                        = self.get_argument("sign", None)
        
        args = {}
        for k, v in all_args.items():
            if v is not None:
                args[k] = v

        if util.make_sign(args, self.settings["app_360_secret"]) != sign:
            LOGGER.error("360@sign is not valid")
            self.response("error")
            return 
        
        if args.get("gateway_flag") != "success":
            LOGGER.error("360@gateway_flag is not success")
            self.response("error")
            return

        try:
            pay = self.db.get("select id from pay_t where orderid = %s and pt = '360'", args.get("order_id"))
            if pay:
                LOGGER.warning("360@repeat send")
                self.response("ok")
                return
        except Exception, e:
            LOGGER.error("360@%s", e)
            self.response("error")
            return

        userid = args.pop("user_id")
        args.pop("gateway_flag")
        args["sign_return"] = sign_return
        args["sign"] = util.make_sign(args, self.settings["app_360_secret"])
        check = yield self.order_verify(args)
        if check == 0:
            try:
                self.db.insert("insert into pay_t (id, openid, userid, itemid, orderid, amount, res, pt, dt) values (0, %s, %s, %s, %s, %s, %s, %s, NOW())",
                                            userid, 0, args.get("product_id"), args.get("order_id"), args.get("amount"), 0, "360")
            except Exception, e:
                LOGGER.error("360@%s", e)
                self.response("error")
                return
        else:
            LOGGER.error("360@verify error")
            self.response("error")
            return

        self.response("ok")

    @tornado.gen.coroutine
    def order_verify(self, args):
        url = "http://mgame.360.cn/pay/order_verify.json"
        url += "?" + urllib.urlencode(args)

        http_client = AsyncHTTPClient()
        try:
            response = yield http_client.fetch(url)
        except:
            raise tornado.gen.Return(-1)

        if response.error:
            raise tornado.gen.Return(-1)

        try:
            data = escape.json_decode(response.body)
        except:
            raise tornado.gen.Return(-1)

        if data.get("ret") != "verified":
            raise tornado.gen.Return(-1)

        raise tornado.gen.Return(0)