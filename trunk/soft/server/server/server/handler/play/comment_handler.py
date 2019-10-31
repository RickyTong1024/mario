# -*- coding: utf-8 -*-

import tornado.web
from constant.error import const
from constant.tag import const
from constant.game import const
from utility import util, auth
from utility.msg_pb2 import *
from model import user_data, map_data

class ViewCommentHandler(tornado.web.RequestHandler):
    @auth.authenticated
    def post(self, cmsg):
        comments = map_data.view_comment(cmsg.id, self.application.get_comment_script)

        mapattrs = map_data.get_map_attrs(cmsg.id, "id", "name", "url", "ownerid", "ownername", "country", 
                                     "head", "favorite", "amount", "pass", "date", "template", "hard", "like")
        
        is_user_favourite, is_user_pass = user_data.is_favourite_and_pass_map(cmsg.common.userid, [cmsg.id])

        smsg = smsg_view_comment()
        smsg.infos.id           = int(mapattrs[0])
        smsg.infos.name         = mapattrs[1]
        smsg.infos.url          = mapattrs[2]
        smsg.infos.owner_id     = int(mapattrs[3])
        smsg.infos.owner_name   = mapattrs[4]
        smsg.infos.country      = mapattrs[5]
        smsg.infos.head         = int(mapattrs[6])
        smsg.infos.favorite     = int(mapattrs[7])
        smsg.infos.amount       = int(mapattrs[8])
        smsg.infos.pas          = int(mapattrs[9])
        smsg.infos.date         = mapattrs[10]
        smsg.infos.collect      = is_user_favourite
        smsg.infos.finish       = is_user_pass
        smsg.infos.difficulty   = int(mapattrs[12])
        smsg.infos.like         = int(mapattrs[13]) if mapattrs[13] else 0
        if comments:
            try:
                for comment in comments:
                    data_dict = {}
                    for index in range(0, len(comment), 2):
                        data_dict[comment[index]] = comment[index + 1]
                    data = smsg.comments.add()
                    data.head       = int(data_dict.get("head"))
                    data.name       = data_dict.get("name") 
                    data.country    = data_dict.get("country")
                    data.visitor    = int(data_dict.get("visitor"))
                    data.text       = data_dict.get("text")
                    data.date       = data_dict.get("date")
                    data.userid     = int(data_dict.get("userid"))
            except Exception, e:
                print e
                pass    
        return auth.pack(smsg, 0, cmsg.common.userid)

class CommentHandler(tornado.web.RequestHandler):
    @auth.authenticated
    def post(self, cmsg):
        if not util.check_input(cmsg.text, 512):
            return auth.pack(None, const.ERROR_INPUT)

        userattrs =  user_data.comment_map(cmsg.common.userid)
        keys = [
            const.TAG_COMMENT_COUNT,
            const.TAG_MAP_COMMENT.format(cmsg.id),
            const.TAG_COMMENT_CACHE.format(cmsg.id),
            const.TAG_MAP.format(cmsg.id)
                ]
        args = [
            'head', userattrs[0],
            'name', userattrs[1],
            'country', userattrs[2],
            'visitor', userattrs[3],
            'text', cmsg.text,
            'date', util.now_date_str(),
            'userid', cmsg.common.userid,
            ]

        map_data.comment_map(cmsg.id, self.application.create_comment_script, keys, args)

        smsg = smsg_comment()
        smsg.comment.head       = int(userattrs[0])
        smsg.comment.name       = userattrs[1]
        smsg.comment.country    = userattrs[2]
        smsg.comment.text       = cmsg.text
        smsg.comment.date       = util.now_date_str()
        smsg.comment.userid     = cmsg.common.userid
        smsg.comment.visitor    = int(userattrs[3])
        return auth.pack(smsg, 0, cmsg.common.userid)
