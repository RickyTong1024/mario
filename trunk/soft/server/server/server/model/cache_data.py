# -*- coding: utf-8 -*-

"""缓存数据"""

import time
import msgpack
from utility import client, util

CACHE_DATA  = {}
CACHE_CHECK = {}

@client.redis_client("global")
def get_cache(conn, key, dt):
    '''
    间隔时间检查缓存是否失效
    所有的缓存都是经过msgpack.dumps的字典数据
    '''

    if CACHE_CHECK.get(key) < time.time() - dt:
        CACHE_CHECK[key] = time.time()
        cache = msgpack.loads(conn.get(key) or '\x80')
        cache = dict((str(k), cache[k]) for k in cache)
        old_cache = CACHE_DATA.get(key)

        if cache != old_cache:
            CACHE_DATA[key] = cache

    return CACHE_DATA.get(key)


@client.redis_client("global")
def set_cache(conn, key, data):
    conn.set(key, data)


