# -*- coding: utf-8 -*-

"""全局数据"""

import time
import math
import uuid
import msgpack
from constant.tag import const
from utility import client, util

@client.redis_client("global")
def get_counter(conn, key):
    '''
    获取计数
    '''
    return conn.incr(key)

@client.redis_client("global")
def get_userid(conn, key, openid):
    '''
    获取用户ID
    '''
    shared_key = client._shared_key(key, openid, 256)
    return conn.hget(shared_key, openid)

@client.redis_client("global")
def get_userid_and_version(conn, key, openid, version):
    '''
    获取用户ID和版本
    '''
    
    shared_key = client._shared_key(key, openid, 256)
    pipe = conn.pipeline(True)
    pipe.hget(shared_key, openid)
    pipe.sismember(const.TAG_GAME_VERSION, version)
    pipe.sismember(const.TAG_GAME_INVALID_VERSION, version)
    return pipe.execute()

@client.redis_client("global")
def set_userid(conn, key, openid, userid):
    '''
    保存用户ID
    '''
    shared_key  = client._shared_key(key, openid, 256)
    conn.hset(shared_key, openid, userid)

@client.redis_client("global")
def set_userid_and_log(conn, key, openid, userid, pt):
    '''
    保存用户ID并日志
    '''
    shared_key  = client._shared_key(key, openid, 256)
    pack = dict(od = openid, ud = userid, op = 1, pt = pt, dt = util.now_time())
    pack = msgpack.dumps(pack)
    pipe = conn.pipeline(True)
    pipe.hset(shared_key, openid, userid)
    pipe.rpush(const.TAG_INFO_LOG, pack)
    pipe.execute()

@client.redis_client("global")
def bind_openid(conn, key, openid, userid):
    '''
    绑定用户账号
    '''
    old_account = const.YOUKE_ACCOUNT.format(userid)
    new_key = client._shared_key(key, openid, 256)
    old_key = client._shared_key(key, old_account, 256)
    pipe = conn.pipeline()
    pipe.hdel(old_key, old_account)
    pipe.hset(new_key, openid, userid)
    pipe.execute()

@client.redis_client("global")
def acquire_lock(conn, key, lockname, script, acquire_timeout = 5, lock_timeout = 5):
    '''
    获取全局锁
    '''

    lock_id = str(uuid.uuid4())
    lockname = 'lock:' + lockname
    lock_timeout = int(math.ceil(lock_timeout))
    acquire = False
    end = time.time() + acquire_timeout
    while time.time()  < end and not acquire:
        acquire = ('OK' == script(keys = [lockname], args = [lock_timeout, lock_id], client = conn))
        time.sleep(.001 * (not acquire))

    return acquire and lock_id

@client.redis_client("global")
def release_lock(conn, key, lockname, lock_id, script):
    '''
    释放全局锁
    '''

    lockname = 'lock:' + lockname
    return script(keys = [lockname], args = [lock_id], client = conn)


@client.redis_client("log")
def log(conn, key, **content):
    '''
    运营日志
    '''
    logpack = msgpack.dumps(content)
    conn.rpush(key, logpack)


@client.redis_client("global")
def get_counter_value(conn, key):
    '''
    获取计数
    '''
    return conn.get(key)


@client.redis_client("global")
def get_libao_reward(conn, key, code):
    '''
    获取礼包奖励
    '''
    
    libao = conn.hget(key, code)
    try:
        reward = msgpack.loads(libao)
    except:
        return None
    return reward

@client.redis_client("global")
def has_libao(conn, key, code):
    '''
    是否存在礼包
    '''

    return conn.sismember(key, code)

@client.redis_client("global")
def remove_libao(conn, key, code):
    '''
    删除礼包
    '''
    return conn.srem(key, code)


@client.redis_client("global")
def get_challenge_mapid(conn, key, index):
    return conn.lindex(key, index)

@client.redis_client("global")
def add_challenge_map(conn, key, mapinfo):
    '''
    添加挑战地图
    '''
    conn.rpush(key, mapinfo)

@client.redis_client("global")
def remove_challenge_map(conn, key):
    '''
    删除所有挑战地图
    '''
    conn.ltrim(key, -1, 0)

@client.redis_client("global")
def get_challenge_map(conn, key, start, end):
    return conn.lrange(key, start, end)


@client.redis_client("global")
def get_map_list(conn, key, start, end, desc):
    '''
    获取地图列表
    '''
    return conn.zrange(key, start, end, desc, True)

@client.redis_client("global")
def set_map_point(conn, key, mapid, point):
    '''
    设置地图分数
    '''
    conn.zadd(key, point, mapid)


@client.redis_client("global")
def check_cache(conn, key):
    '''
    设置地图分数
    '''
    conn.zadd(key, point, mapid)


@client.redis_client("global")
def challenge_rank(conn, key, userid, point, script, first):
    args = [userid, point, first]
    script(keys = [], args = args, client = conn)

@client.redis_client("global")
def challenge_finish_rank(conn, key, userid, point, script):
    args = [userid, point]
    return script(keys = [], args = args, client = conn)

@client.redis_client("global")
def remove_rank(conn, key, userid):
    conn.zrem(key, userid)