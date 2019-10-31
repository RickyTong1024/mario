# -*- coding: utf-8 -*-

import sys
import yaml
import logging
import rsa
import torndb
import logging.config
import tornado.httpserver
import tornado.ioloop
from tornado.options import define, options, parse_command_line
import qihu360_handler
import alipay_web_handler, alipay_phone_handler, alipay_win_handler

define('port', default=8080)
define("mysql_host", default="127.0.0.1:3306")
define("mysql_database", default="malio_pay")
define("mysql_user", default="root")
define("mysql_password", default="root")

class Application(tornado.web.Application):
    def __init__(self):
        handlers = [
            (r"/notify360", qihu360_handler.NotifyHandler),
            (r"/notifyweb_yymoon", alipay_web_handler.NotifyHandler),
            (r"/notifywin_yymoon", alipay_win_handler.NotifyHandler),
            (r"/notifyyymoon", alipay_phone_handler.NotifyHandler),
        ]
        settings = dict(
            app_360_secret = "728bd8306a5fc212a830f72576945b70",
            alipay_secret = "bb2eehymwcvhntc7zloxi1ne4bx4zxbm"
        )
        tornado.web.Application.__init__(self, handlers, **settings)

        self.db = torndb.Connection(
            host=options.mysql_host, database=options.mysql_database,
            user=options.mysql_user, password=options.mysql_password)

        with open("rsa_public_key.pem") as f:
            p = f.read()
            self.alipay_pubkey = rsa.PublicKey.load_pkcs1_openssl_pem(p)
        with open("rsa_private_key.pem") as f:
            p = f.read()
            self.alipay_prikey = rsa.PrivateKey._load_pkcs1_pem(p)

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