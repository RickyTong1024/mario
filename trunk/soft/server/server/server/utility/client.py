# -*- coding: utf-8 -*-

'''根据id分片的客户端'''


import binascii
import functools


_REDIS_CONNECTION   = {}
_REDIS_COMPONET     = {}

def _shared_key(componet, key, shared_size):
    """获取共享key"""

    shared_id = binascii.crc32(str(key)) % shared_size
    return "{0}:{1}".format(componet, shared_id)

def _get_shared_connection(componet, key):
    """获取共享连接"""

    id = _shared_key(componet, key, _REDIS_COMPONET[componet])
    return _REDIS_CONNECTION.get(id)

def redis_client(componet):
    def wrapper(function):
        @functools.wraps(function)
        def call(key, *args, **kwargs):
            conn = _get_shared_connection(componet, key)
            return function(conn, key, *args, **kwargs)
        return call
    return wrapper


            



