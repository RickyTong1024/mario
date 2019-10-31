# -*- coding: utf-8 -*-

import os
import sys
import yaml
import logging
import rsa
import json
import urllib
import time
import datetime
import requests
import torndb
import logging.config
import tornado.httpserver
import tornado.ioloop
import tornado.web
import tornado.gen
from tornado.options import define, options, parse_command_line
from tornado.httpclient import AsyncHTTPClient, HTTPRequest

LOGGER = logging.getLogger(__name__)

define('port', default=8060)

app_id = ""
app_secret = ""
app_access_toke = None
verify_token = None


def get_app_access_token():
    global app_id
    global app_secret
    global app_access_toke

    return 0

    url = "https://graph.facebook.com/oauth/access_token"
    data = dict(
        client_id = app_id,
        client_secret = app_secret,
        grant_type = "client_credentials"
        )

    try:
        rsp = requests.get(url, data = data)
        jdata = rsp.json()
    except Exception, e:
        LOGGER.error("facebook@%s", e)
        return -1

    LOGGER.info(jdata)

    if 'access_token' in jdata:
        app_access_toke = jdata["access_token"]
        return 0

    return -1


class FacebookNotifyHandler(tornado.web.RequestHandler):
    def get(self):
        global verify_token

        hubmode = self.get_argument("hub.mode")
        hubchallenge = self.get_argument("hub.challenge")
        hubtoken = self.get_argument("hub.verify_token")

        if hubmode != "subscribe":
            self.write("error")
        if hubtoken != verify_token:
            self.write("error")

        self.write(hubchallenge)

    def post(self):
        try:
            data = json.loads(self.request.body)
        except Exception, e:
            LOGGER.error("facebook@%s", e)
            self.write("error")

        LOGGER.info(data)

        self.write("200")
            

class PayVerifyHandler(tornado.web.RequestHandler):
    @tornado.web.asynchronous
    @tornado.gen.coroutine
    def get(self):
        paymentid = self.get_argument("payment_id")
        userid = self.get_argument("userid")

        res = yield self.verify(userid, paymentid)
        self.write(res)
        self.finish()

    @tornado.gen.coroutine
    def verify(self, userid, paymentid):
        global app_access_toke

        LOGGER.info("%s %s %s", time.strftime("%Y-%m-%d %H:%M:%S"), userid, paymentid)

        try:
            url = 'https://graph.facebook.com/' + str(paymentid)
            params = {"access_token":app_access_toke}
            url += "?" + urllib.urlencode(params)
            http_client = AsyncHTTPClient()
            respone = yield http_client.fetch(url)
        except Exception, e:
            LOGGER.error("facebook@%s", e)
            raise tornado.gen.Return("error")

        try:
            data = json.loads(respone.body)
        except Exception, e:
            LOGGER.error("facebook@%s", e)
            raise tornado.gen.Return("error")

        LOGGER.info(data)

        action_type = ""
        action_status = ""
        action_amount = ""
        actions = data.get("actions", None)
        if actions and isinstance(actions, list):
            for action in actions:
                if isinstance(action, dict):
                    if action.has_key("type"):
                        action_type = action.get("type")
                    if action.has_key("status"):
                        action_status = action.get("status")
                    if action.has_key("amount"):
                        action_amount = action.get("amount")

        item_product = ""
        items = data.get("items", None)
        if items and isinstance(items, list):
            for item in items:
                if isinstance(item, dict):
                    if item.has_key("product"):
                        item_product = item.get("product")


        if action_type != "charge":
            raise tornado.gen.Return("error")
        if action_status != "completed":
            raise tornado.gen.Return("error")

        raise tornado.gen.Return("ok")


class Application(tornado.web.Application):
    def __init__(self):
        handlers = [
            (r"/facebook_pay", PayVerifyHandler),
            (r"/facebook_notify", FacebookNotifyHandler),
        ]
        tornado.web.Application.__init__(self, handlers)


def main():
    reload(sys)
    sys.setdefaultencoding('utf8')
    tornado.options.parse_command_line()
    logging.config.dictConfig(yaml.load(open("logging.yaml", 'r')))

    if get_app_access_token() == 0:
        http_server = tornado.httpserver.HTTPServer(Application(), ssl_options = {
            "certfile": os.path.join(os.path.abspath("."), "server.crt"),
            "keyfile": os.path.join(os.path.abspath("."), "server.pem")
           }
                                                    )
        http_server.listen(options.port)
        tornado.ioloop.IOLoop.instance().start()
            
if __name__ == '__main__':
    main()


        