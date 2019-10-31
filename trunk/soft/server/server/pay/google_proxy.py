# -*- coding: utf-8 -*-

import sys
import yaml
import logging
import rsa
import json
import urllib
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

define('port', default=8070)

client_id = '261628343777-5g3lpknuvisbpgq9u4k97h32hhrbmp20.apps.googleusercontent.com'
client_secret = 'AprHWV5oVwJu1eL0HThvUaoq'
access_token = None

refresh_token = '1/GlTkonMcM4f7KkqS22_prbMQg1admUhBQwOp48hjXFFIgOrJDtdun6zK6XiATCKT'
access_token_create_time = None
access_token_expire_time = None

class PayVerifyHandler(tornado.web.RequestHandler):
    @tornado.web.asynchronous
    @tornado.gen.coroutine
    def get(self):
        bill_id         = self.get_argument("bill_id", "empty")
        packageName     = self.get_argument("packageName", "empty")
        productId       = self.get_argument("productId", "empty")
        purchase_token  = self.get_argument("purchase_token", "empty")
        
        res = yield self.verify(bill_id, productId, purchase_token, packageName)

        self.write(res)
        self.finish()

    @tornado.gen.coroutine
    def verify(self, bill_id, product_id, purchase_token, package_name):
        url_fmt = 'https://www.googleapis.com/androidpublisher/v2/applications/{packageName}/purchases/products/{productId}/tokens/{token}'
        url = url_fmt.format(packageName = package_name,
                             productId = product_id,
                             token = purchase_token)
        access_tokenssss = yield self.get_token()
        params = {"access_token":access_tokenssss}
        url += "?" + urllib.urlencode(params)
        LOGGER.info(url)
        http_client = AsyncHTTPClient()
        try:
            respone = yield http_client.fetch(url)
        except Exception, e:
            LOGGER.error("google@%s", e)
            raise tornado.gen.Return("error")

        try:
            data = json.loads(respone.body)
        except Exception, e:
            LOGGER.error("google@%s", e)
            raise tornado.gen.Return("error")

        LOGGER.info(data)

        if data.get('purchaseState') != 0:
            raise tornado.gen.Return("error")

        raise tornado.gen.Return("ok")


    @tornado.gen.coroutine
    def get_token(self):
        global client_id
        global client_secret
        global access_token
        global refresh_token
        global access_token_create_time
        global access_token_expire_time

        need_get_access_token = False
        if access_token:
            now = datetime.datetime.now()
            if now >= access_token_expire_time:
                need_get_access_token = True
        else:
            need_get_access_token = True

        if not need_get_access_token:
            raise tornado.gen.Return(access_token)

        base_url = 'https://accounts.google.com/o/oauth2/token'
        data = dict(
            grant_type='refresh_token',
            client_id=client_id,
            client_secret=client_secret,
            refresh_token=refresh_token,
            )

        try:
            rsp = requests.post(base_url, data=data)
            jdata = rsp.json()
        except Exception, e:
            LOGGER.error("google@%s", e)
            raise tornado.gen.Return("error")

        if 'access_token' in jdata:
            access_token = jdata['access_token']
            access_token_create_time = datetime.datetime.now()
            access_token_expire_time = access_token_create_time + datetime.timedelta(
                seconds=jdata['expires_in'] * 2 / 3
            )
            raise tornado.gen.Return(access_token)
        else:
            LOGGER.error('no access_token: %s', rsp.text)
            raise tornado.gen.Return("error")

class Application(tornado.web.Application):
    def __init__(self):
        handlers = [
            (r"/google_pay", PayVerifyHandler),
        ]
        tornado.web.Application.__init__(self, handlers)

def main():
    reload(sys)
    sys.setdefaultencoding('utf8')
    tornado.options.parse_command_line()
    logging.config.dictConfig(yaml.load(open("logging.yaml", 'r')))
    http_server = tornado.httpserver.HTTPServer(Application())
    http_server.listen(options.port)
    tornado.ioloop.IOLoop.instance().start()
            
if __name__ == '__main__':
    main()