# -*- coding: utf-8 -*-

'''消息工厂'''

from msg_pb2 import *

class MsgFactory(object):
    def __init__(self):
        self.msg = {
            "/10001":cmsg_login,
            "/10002":cmsg_register,
            "/10003":cmsg_change_account,
            "/10004":cmsg_view_map,
            "/10005":cmsg_view_comment,
            "/10006":cmsg_play_map,
            "/10007":cmsg_complete_map,
            "/10008":cmsg_search_map,
            "/10009":cmsg_favorite_map,
            "/10010":cmsg_replay_map,
            "/10011":cmsg_view_edit,
            "/10012":cmsg_create_map,
            "/10013":cmsg_play_edit_map,
            "/10014":cmsg_save_map,
            "/10015":cmsg_change_map_name,
            "/10016":cmsg_upload_map,
            "/10017":cmsg_delete_map,
            "/10018":cmsg_comment,
            "/10019":cmsg_view_map_point_rank,
            "/10020":cmsg_view_video,
            "/10021":cmsg_view_edit_single,
            "/10022":cmsg_shop_buy,
            "/10023":cmsg_view_player,
            "/10024":cmsg_complete_guide,
            #"/10025":cmsg_mission_view,
            #"/10026":cmsg_mission_start,
            #"/10027":cmsg_mission_play,
            #"/10028":cmsg_mission_play,
            #"/10029":cmsg_mission_complete,
            #"/10030":cmsg_mission_play,
            "/10031":cmsg_login_android,
            "/10032":cmsg_change_name,
            "/10033":cmsg_pay,
            "/10034":cmsg_libao,
            "/10035":cmsg_challenge_view,
            "/10036":cmsg_challenge_start,
            "/10037":cmsg_challenge_continue,
            "/10038":cmsg_challenge_replay,
            "/10039":cmsg_challenge_fail,
            "/10040":cmsg_challenge_success,
            "/10041":cmsg_challenge_rank,
            "/10042":cmsg_challenge_buy,
            "/10043":cmsg_google_pay,
            "/10044":cmsg_map_like,
            "/10045":cmsg_download_map,
            }


    def __call__(self, uri):
        msg = self.msg.get(uri)
        if msg:
            return msg()
        return None









