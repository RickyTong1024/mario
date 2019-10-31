# -*- coding: utf-8 -*-

import os
import sys
import logging
import yaml
import logging.config
import ConfigParser
import tornado.httpserver
import tornado.ioloop
import tornado.web
from app import application
from handler.account import *
from handler.edit import *
from handler.play import *
from handler.charge import *
from handler.huodong import *
from handler.challenge import *


config_file_path = os.path.join(os.path.dirname(os.path.abspath(__file__)), "settings.conf")

class CrossdomainHandle(tornado.web.RequestHandler):
    def get(self):
        self.write('<?xml version="1.0" encoding="UTF-8"?><cross-domain-policy><allow-access-from domain="*"/></cross-domain-policy>')

def main():
    reload(sys)
    sys.setdefaultencoding('utf8')

    logging.config.dictConfig(yaml.load(open("logging.yaml", 'r')))

    conf = ConfigParser.ConfigParser()
    conf.read(config_file_path)
    http_port = conf.getint("http", "port")

    app = application.Application([
            (r"/crossdomain.xml", CrossdomainHandle),
            (r"/10001", login_handler.LoginHandle),
            (r"/10002", bind_handler.BindHandle),
            (r"/10003", change_handler.ChangeHandle),
            (r"/10004", map_handler.ViewMapHandler),
            (r"/10005", comment_handler.ViewCommentHandler),
            (r"/10006", play_handler.PlayHandler),
            (r"/10007", complete_handler.CompleteHandler),
            (r"/10008", search_handler.SearchMapHandler),
            (r"/10009", favourite_handler.FavouriteMapHandler),
            (r"/10010", play_handler.RePlayHandler),
            (r"/10011", view_handler.ViewAllHandler),
            (r"/10012", create_handler.CreateMapHandler),
            (r"/10013", edit_handler.EditHandler),
            (r"/10014", save_handler.SaveHandler),
            (r"/10015", name_handler.ChangeNameHandler),
            (r"/10016", upload_handler.UploadHandler),
            (r"/10017", delete_handler.DeleteMapHandler),
            (r"/10018", comment_handler.CommentHandler),
            (r"/10019", rank_handler.ViewRankHandler),
            (r"/10020", video_handler.ViewVideoHandler),
            (r"/10021", view_handler.ViewSingleHandler),
            (r"/10022", shop_handler.ShopBuyHandler),
            (r"/10023", player_handler.ViewPlayerHandler),
            (r"/10024", guide_handler.GuideHandler),
            (r"/10031", login_platform_handler.LoginPlatformHandler),
            (r"/10032", change_name_handler.ChangeNameHandler),
            (r"/10033", pay_handler.PayHandler),
            (r"/10034", libao_handler.LibaoHandler),
            (r"/10035", challenge_view_handler.ViewHandler),
            (r"/10036", challenge_start_handler.StartHandler),
            (r"/10037", challenge_start_handler.StartHandler),
            (r"/10038", challenge_replay_handler.RePlayHandler),
            (r"/10039", challenge_fail_handler.FailHandler),
            (r"/10040", challenge_success_handler.SuccessHandler),
            (r"/10041", challenge_rank_handler.RankHandler),
            (r"/10042", challenge_buy_handler.BuyHandler),
            (r"/10043", google_pay_handler.GooglePayHandler),
            (r"/10044", like_handler.MapLikeHandler),
            (r"/10045", download_handler.DownloadHandler),
        ], conf)

    http_server = tornado.httpserver.HTTPServer(app)
    http_server.listen(http_port)
    tornado.ioloop.IOLoop.instance().start()
            
if __name__ == '__main__':
    main()