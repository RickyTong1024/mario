# -*- coding: utf-8 -*-

'''登录认证检查'''

import functools
import tornado.web
from Crypto.Cipher import DES
from constant.tag import const
from constant.error import const
from msg_pb2 import *
import client

BS = 8
pad     = lambda s: s + (BS - len(s) % BS) * chr(BS - len(s) % BS) 
unpad   = lambda s : s[:-ord(s[len(s)-1:])]


@client.redis_client("user")
def save(conn, userid, msg):
    '''保存用户消息'''

    pipe = conn.pipeline(False)
    pipe.hincrby(const.TAG_USER.format(userid), "index", 1)
    pipe.hset(const.TAG_USER.format(userid), "pck", msg)
    pipe.execute()

def pack(msg, res = 0, userid = None, error = None):
    '''打包消息'''

    response = msg_response()
    response.res = res
    if msg:
        response.msg = msg.SerializeToString()
    if error:
        response.error = error.SerializeToString()
    pck = response.SerializeToString()
    if (res == 0 or res == -1) and userid:
        save(userid, pck)
    return pck

def unpack(dest, src):
    '''解包消息'''

    try:
        codec = DES.new('tsjhtsjh', DES.MODE_CBC, '51478543')
        info = unpad(codec.decrypt(src))
        dest.ParseFromString(info)
        return True
    except:
        return False

@client.redis_client("user")
def check(conn, userid, usersig, userpck):
    '''检查用户消息'''

    pipe = conn.pipeline()
    pipe.get(const.TAG_USER_SESSION.format(userid))
    pipe.hget(const.TAG_USER.format(userid), "index")
    sig, index = pipe.execute()
    if sig != usersig:
        return const.ERROR_SIG
    index = int(index) if index else -100
    if index == userpck + 1:
        return conn.hget(const.TAG_USER.format(userid), "pck") or const.ERROR_SIG
    elif index == userpck:
        return 0
    else:
        return const.ERROR_SIG

def authenticated(method):
    @functools.wraps(method)
    def wrapper(self, *args, **kwargs):
        msg = self.application.msg_factory(self.request.uri)
        try:
            codec = DES.new('tsjhtsjh', DES.MODE_CBC, '51478543')
            body = unpad(codec.decrypt(self.request.body))
            msg.ParseFromString(body)
        except:
            return self.write(pack(None, const.ERROR_DECODE))    
        if msg.DESCRIPTOR.fields_by_name.has_key("common"):
            result = check(msg.common.userid, msg.common.sig, msg.common.pck_id)
            if isinstance(result, int):
                if result != 0:
                    return self.write(pack(None, result))
            else:
                return self.write(result)
        response = method(self, msg)
        self.write(response)
    return wrapper