# -*- coding: utf-8 -*-

'''根据id分片的客户端'''


import time
import binascii
import functools
import redis
from backend import settings


_REDIS_CONNECTION   = {}
_REDIS_COMPONET     = {}

_REDIS_COMPONET["global"] = len(settings.REDIS_GLOBAL)
for i in range(len(settings.REDIS_GLOBAL)):
    item = settings.REDIS_GLOBAL[i]
    conn_pool = redis.ConnectionPool(host = item[0], port = item[1], db = item[2])
    conn = redis.StrictRedis(connection_pool = conn_pool)
    conn.ping()
    _REDIS_CONNECTION["global:" + str(i)] = conn

    import platform
    if platform.system() != "Windows":
        import redis_search.util
        redis_search.util.redis     = redis.Redis(connection_pool = conn_pool)

_REDIS_COMPONET["map"] = len(settings.REDIS_MAP)
for i in range(len(settings.REDIS_MAP)):
    item = settings.REDIS_MAP[i]
    conn = redis.StrictRedis(host = item[0], port = item[1], db = item[2])
    conn.ping()
    _REDIS_CONNECTION["map:" + str(i)] = conn

_REDIS_COMPONET["user"] = len(settings.REDIS_USER)
for i in range(len(settings.REDIS_USER)):
    item = settings.REDIS_USER[i]
    conn = redis.StrictRedis(host = item[0], port = item[1], db = item[2])
    conn.ping()
    _REDIS_CONNECTION["user:" + str(i)] = conn

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


@redis_client("global")
def get_userid(conn, openid, pt):
    platform = "openid-to-userid"
    if pt == "360":
        platform += "-qihu360"
    elif pt == "yymoon":
        pass
    else:
        return None

    shared_key = _shared_key(platform, openid, 256)
    return conn.hget(shared_key, openid)


@redis_client("global")
def get_counter(conn, key):
    '''
    获取计数
    '''
    return conn.incr(key)

@redis_client("global")
def add_version(conn, type, version):
    '''
    添加版本
    '''
    if type == "1":
        conn.sadd("game:version", version)
    elif type == "2":
        conn.sadd("game:invalid:version", version)

@redis_client("global")
def remove_version(conn, type, version):
    '''
    移除版本
    '''
    if type == "1":
        conn.srem("game:version", version)
    elif type == "2":
        conn.srem("game:invalid:version", version)

@redis_client("user")
def get_user_att(conn, userid, att):
    return conn.hget("user:{0}".format(userid), att)

@redis_client("user")
def set_user_att(conn, userid, att, val):
    return conn.hset("user:{0}".format(userid), att, val)

@redis_client("user")
def inc_user_att(conn, userid, att, val):
    return conn.hincrby("user:{0}".format(userid), att, val)


@redis_client("map")
def exist_map(conn, mapid):
    return conn.exists("map:{0}".format(mapid))

@redis_client("map")
def get_map_attrs(conn, mapid, *attrs):
    return conn.hmget("map:{0}".format(mapid), attrs)

@redis_client("map")
def get_challenge_map(conn, key):
    return conn.lrange(key, 0, -1)

@redis_client("map")
def add_challenge_map(conn, key, mapid):
    conn.rpush(key, mapid)


@redis_client("map")
def upload_map(conn, mapid, upload, **kwargs):
    if kwargs.has_key("passs"):
        val = kwargs.pop("passs")
        kwargs["pass"] = val

    if upload:
        pipe = conn.pipeline()
        pipe.hmset("map:{0}".format(mapid), kwargs)
        pipe.zadd("rank:popular", 0, mapid)
        pipe.zadd("rank:upload", long(time.time() * 1000), mapid)
        pipe.sadd("set:primary", mapid)
        pipe.execute()

        import platform
        if platform.system() == "Windows":
            return 0

        import redis_search.index
        map_index = redis_search.index.index("map:search", mapid, kwargs.get("name"))
        map_index.save()
        return 0

    if not conn.exists("map:{0}".format(mapid)):
        return -2
    conn.hmset("map:{0}".format(mapid), kwargs)
    return 0


@redis_client("map")
def update_map_version(conn, version):
    old_version = version - 1
    rank_most_popular = "rank:popular:version:{0}".format(old_version)
    rank_newer_upload = "rank:upload:version:{0}".format(old_version)
    rank_recent_host = "rank:hot:version:{0}".format(old_version)
    rank_primary_hard = "rank:primary:version:{0}".format(old_version)
    rank_middle_hard = "rank:middle:version:{0}".format(old_version)
    rank_senior_hard = "rank:senior:version:{0}".format(old_version)
    rank_master_hard = "rank:master:version:{0}".format(old_version)
    rank_most_popular_ex = "rank:popular:ex:version:{0}".format(old_version)


    conn.zunionstore(rank_most_popular, ["rank:popular"])
    conn.zunionstore(rank_newer_upload, ["rank:upload"])
    conn.zunionstore(rank_recent_host, ["rank:hot"])
    conn.zunionstore(rank_primary_hard, ["rank:primary"])
    conn.zunionstore(rank_middle_hard, ["rank:middle"])
    conn.zunionstore(rank_senior_hard, ["rank:senior"])
    conn.zunionstore(rank_master_hard, ["rank:master"])
    conn.zunionstore(rank_most_popular_ex, ["rank:popular:ex"])

    t = 7 * 24 * 60 * 60
    conn.expire(rank_most_popular, t)
    conn.expire(rank_newer_upload, t)
    conn.expire(rank_recent_host, t)
    conn.expire(rank_primary_hard, t)
    conn.expire(rank_middle_hard, t)
    conn.expire(rank_senior_hard, t)
    conn.expire(rank_master_hard, t)
    conn.expire(rank_most_popular_ex, t)

    conn.set("version:mapset", version)


@redis_client("map")
def create_rank(conn, rank):
    if rank == "":
        return

    if rank == "rank:popular:ex":
        all_rank = conn.zrange("rank:popular", 0, -1)

        for mapid in all_rank:
            if conn.exists("map:{0}".format(mapid)):
                amount, pas, favourite, comment, like = conn.hmget("map:{0}".format(mapid), "amount",
                                                                   "pass",
                                                                   "favorite",
                                                                   "comment",
                                                                   "like")
                if comment is None:
                    comment = 0
                if like is None:
                    like = 0

                amount = int(amount)
                pas = int(pas)
                favourite = int(favourite)
                comment = int(comment)
                like = int(like)

                score = amount + pas * 10 + comment * 20 + favourite * 50 + like * 100

                conn.zadd("rank:popular:ex", score, mapid)

            



