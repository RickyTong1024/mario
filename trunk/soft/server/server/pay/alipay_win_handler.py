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
    def post(self):
        all_args = {}
        all_args["notify_time"]             = self.get_argument("notify_time", None)
        all_args["notify_type"]             = self.get_argument("notify_type", None)
        all_args["notify_id"]               = self.get_argument("notify_id", None)
        all_args["out_trade_no"]            = self.get_argument("out_trade_no", None)
        all_args["subject"]                 = self.get_argument("subject", None)
        all_args["payment_type"]            = self.get_argument("payment_type", None)
        all_args["trade_no"]                = self.get_argument("trade_no", None)
        all_args["trade_status"]            = self.get_argument("trade_status", None)
        all_args["seller_id"]               = self.get_argument("seller_id", None)
        all_args["seller_email"]            = self.get_argument("seller_email", None)
        all_args["buyer_id"]                = self.get_argument("buyer_id", None)
        all_args["buyer_email"]             = self.get_argument("buyer_email", None)
        all_args["total_fee"]               = self.get_argument("total_fee", None)
        all_args["quantity"]                = self.get_argument("quantity", None)
        all_args["price"]                   = self.get_argument("price", None)
        all_args["body"]                    = self.get_argument("body", None)
        all_args["gmt_create"]              = self.get_argument("gmt_create", None)
        all_args["gmt_payment"]             = self.get_argument("gmt_payment", None)
        all_args["gmt_close"]               = self.get_argument("gmt_close", None)
        all_args["is_total_fee_adjust"]     = self.get_argument("is_total_fee_adjust", None)
        all_args["use_coupon"]              = self.get_argument("use_coupon", None)
        all_args["discount"]                = self.get_argument("discount", None)
        all_args["refund_status"]           = self.get_argument("refund_status", None)
        all_args["gmt_refund"]              = self.get_argument("gmt_refund", None)
        all_args["extra_common_param"]      = self.get_argument("extra_common_param", None)
        all_args["out_channel_type"]        = self.get_argument("out_channel_type", None)
        all_args["out_channel_amount"]      = self.get_argument("out_channel_amount", None)
        all_args["out_channel_inst"]        = self.get_argument("out_channel_inst", None)
        all_args["business_scene"]          = self.get_argument("business_scene", None)
        sign_type                           = self.get_argument("sign_type", None)
        sign                                = self.get_argument("sign", None)
        
        args = {}
        for k, v in all_args.items():
            if v is not None:
                args[k] = v.encode('utf-8')

        if util.make_alipy_sign(args, self.settings["alipay_secret"]) != sign:
            LOGGER.error("win_yymoon@sign is not valid")
            self.response("error")
            return
        
        if args.get("trade_status") != "TRADE_SUCCESS" and args.get("trade_status") != "TRADE_FINISHED":
            LOGGER.error("win_yymoon@trade_status is not success")
            self.response("error")
            return

        try:
            pay = self.db.get("select id from pay_t where orderid = %s and pt = 'win_yymoon'", args.get("trade_no"))
            if pay:
                LOGGER.warning("win_yymoon@repeat send")
                self.response("success")
                return
        except Exception, e:
            LOGGER.error("win_yymoon@%s", e)
            self.response("error")
            return

        check = yield self.order_verify(args.get("notify_id"))
        if check == 0:
            try:
                self.db.insert("insert into pay_t (id, openid, userid, itemid, orderid, amount, res, pt, dt) values (0, %s, %s, %s, %s, %s, %s, %s, NOW())",
                                            args.get("extra_common_param"), 0, 0, args.get("trade_no"), int(float(args.get("total_fee")) * 100), 0, "win_yymoon")
            except Exception, e:
                LOGGER.error("win_yymoon@%s", e)
                self.response("error")
                return
        else:
            LOGGER.error("win_yymoon@verify error")
            self.response("error")
            return

        self.response("success")

    @tornado.gen.coroutine
    def order_verify(self, notifyid):
        url = "https://mapi.alipay.com/gateway.do"
        args = {}
        args["service"] = "notify_verify"
        args["partner"] = "2088811685407064"
        args["notify_id"]  = notifyid
        url += "?" + urllib.urlencode(args)

        http_client = AsyncHTTPClient()
        try:
            response = yield http_client.fetch(url)
        except:
            raise tornado.gen.Return(-1)

        if response.error:
            raise tornado.gen.Return(-1)

        if response.body != "true":
            LOGGER.error("win_yymoon@%s", response.body)
            raise tornado.gen.Return(-1)

        raise tornado.gen.Return(0)