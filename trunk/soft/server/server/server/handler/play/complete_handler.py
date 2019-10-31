# -*- coding: utf-8 -*-

import tornado.web
from constant.error import const
from constant.tag import const
from constant.game import const
from utility import util, client, auth
from utility.msg_pb2 import *
from model import user_data, global_data, map_data

class CompleteHandler(tornado.web.RequestHandler):
    @auth.authenticated
    def post(self, cmsg):
        userattrs = user_data.get_user_attrs(cmsg.common.userid, 
                                             "level", 
                                             "exp", 
                                             "exp_time", 
                                             "support",
                                             "mapid")
        userattrs = filter(None, userattrs)
        if not userattrs or len(userattrs) != 5:
            return auth.pack(None, const.ERROR_SYSTEM)
        mapid = userattrs[4]

        # 更新地图数据
        amount, pas, rank, template_id  = map_data.complete_map(mapid, 
                              cmsg.common.userid, 
                              cmsg.suc, 
                              cmsg.x, 
                              cmsg.y, 
                              cmsg.time,
                              self.application.update_map_script
                              )

        exp_inc = 0
        exp_etra = 0
        support = 0
        hard = 0
        next_mapid = 0
        next_templateid = 0
        if cmsg.suc == 0:
            # 考试地图
            if template_id != '0':
                map_template = self.application.static_data.get_map_template(int(template_id))
                if map_template:
                    if map_template.support > int(userattrs[3]):
                        support = map_template.support
                    next_templateid = map_template.nextid
                    next_map_template = self.application.static_data.get_map_template(next_templateid)
                    if next_map_template:
                        hard        =  next_map_template.hard
                        next_mapid  = next_templateid
                    else:
                        hard        = 0
                        next_mapid  = -1
            # 经验
            exp_inc = 2
            if not user_data.is_pass_map(cmsg.common.userid, mapid):
                exp_inc = self._get_exp(amount, pas)
            if util.now_time() < long(userattrs[2]):
                exp_etra = exp_inc
        
        # 升级
        level, exp, shengji = self._check_levelup(int(userattrs[0]), exp_inc + exp_etra + int(userattrs[1]))
        user_life_time1 = None
        if shengji:
            level_template = self.application.static_data.get_level_template(level)
            if level_template:
                user_life, user_life_time = user_data.get_user_attrs(cmsg.common.userid, "life", "life_time")
                if int(user_life) < level_template.life_max and long(user_life_time) < util.now_time():
                    user_life_time1 = util.now_time() + const.LIEF_TIME
        user_data.complete_map(cmsg.common.userid, 
                                mapid, 
                                cmsg.suc, 
                                cmsg.point, 
                                cmsg.video, 
                                level, 
                                exp, 
                                rank,
                                support,
                                next_templateid,
                                user_life_time1,
                                cmsg.time)
        

        smsg = smsg_complete_map()
        smsg.exp        = exp_inc
        smsg.rank       = rank
        smsg.testify    = support
        smsg.extra_exp  = exp_etra
        smsg.mapid      = next_mapid
        smsg.support    = hard
        return auth.pack(smsg, 0, cmsg.common.userid)

    def _check_levelup(self, level, exp):
        next_level = level + 1
        shengji = False
        while True:
            level_template = self.application.static_data.get_level_template(next_level)
            if not level_template:
                break
            if exp >= level_template.exp:
                exp -= level_template.exp
                level += 1
                next_level += 1
                shengji = True
            else:
                break

        return (level, exp, shengji)

    def _get_exp(self, amount, pas):
        if amount < 100:
            return 2
        rate = float(pas) / float(amount)
        if rate < 0.001:
            return 11
        elif rate < 0.01:
            return 9
        elif rate < 0.05:
            return 7
        elif rate < 0.1:
            return 6
        elif rate < 0.2:
            return 5
        elif rate < 0.4:
            return 4
        elif rate < 0.5:
            return 3
        else:
            return 2
        


            