# -*- coding: utf-8 -*-

'''地图数据'''

import msgpack
from constant.tag import const
from utility import util, client

@client.redis_client("map")
def exist_map(conn, mapid):
    '''检查地图有效性'''

    return conn.exists(const.TAG_MAP.format(mapid))

@client.redis_client("map")
def get_map(conn, mapid):
    '''获取地图所有属性'''

    return conn.hgetall(const.TAG_MAP.format(mapid))

@client.redis_client("map")
def get_map_attr(conn, mapid, att):
    '''获取地图一个属性'''

    return conn.hget(const.TAG_MAP.format(mapid), att)

@client.redis_client("map")
def get_map_attrs(conn, mapid, *atts):
    '''获取地图多个属性'''

    return conn.hmget(const.TAG_MAP.format(mapid), *atts)

@client.redis_client("map")
def inc_map_attr(conn, mapid, att, val):
    '''增加地图属性'''

    return conn.hincrby(const.TAG_MAP.format(mapid), att, val)

@client.redis_client("map")
def inc_map_attrs(conn, mapid, atts_list):
    '''增加地图多个属性'''

    pipe = conn.pipeline()
    for item in atts_list:
        pipe.hincrby(const.TAG_MAP.format(mapid), item[0], item[1])
    pipe.execute()

@client.redis_client("map")
def set_map_attr(conn, mapid, att, val):
    '''设置地图一个属性'''

    conn.hset(const.TAG_MAP.format(mapid), att, val)

@client.redis_client("map")
def set_map_attrs(conn, mapid, mapping):
    '''设置地图多个属性'''

    conn.hmset(const.TAG_MAP.format(mapid), mapping)


@client.redis_client("map")
def upload_map(conn, mapid, name, url, data, userid, username, 
               usercountry, userhead, version, ttime = None):
    '''上传地图'''

    pipe = conn.pipeline()
    pipe.hmset(const.TAG_MAP.format(mapid),
           {"id":mapid,
            "name":name,
            "url":url,
            "ownerid":userid,
            "ownername":username,
            "country":usercountry,
            "head":userhead,
            "favorite":0,
            "amount":0,
            "pass":0,
            "comment":0,
            "like":0,
            "template":0,
            "date":util.now_date_str(),
            "hard":0,
            "data":data})
    pipe.zadd(const.TAG_RANK_MOST_POPULAR, 0, mapid)
    pipe.zadd(const.TAG_RANK_NEWER_UPLOAD, util.now_time(), mapid)
    if ttime:
        pipe.zadd(const.TAG_MAP_POINT.format(mapid), ttime, userid)
    pipe.execute()

@client.redis_client("map")
def play_map(conn, mapid):
    pipe = conn.pipeline()
    pipe.hincrby(const.TAG_MAP.format(mapid), "amount", 1)
    if int(mapid) >= 10010000:
        pipe.zincrby(const.TAG_RANK_MOST_POPULAR, mapid, 1)
    pipe.execute()

@client.redis_client("map")
def replay_map(conn, mapid):
    map_key = const.TAG_MAP.format(mapid)
    pipe = conn.pipeline()
    pipe.hincrby(map_key, "amount", 1)
    if int(mapid) >= 10010000:
        pipe.zincrby(const.TAG_RANK_MOST_POPULAR, mapid, 1)
    pipe.hget(map_key, "ownerid")
    return pipe.execute()[-1]

@client.redis_client("map")
def get_dead_coordinate(conn, mapid, play_count):
    '''获取死亡坐标'''

    if conn.exists(const.TAG_MAP_XY_SORT.format(mapid)):
        return msgpack.loads(conn.get(const.TAG_MAP_XY_SORT.format(mapid)))

    xys = conn.zrange(const.TAG_MAP_XY.format(mapid), 0, -1, True)
    pipe = conn.pipeline()
    for xy in xys:
        pipe.lrange(const.TAG_MAP_XY_SET.format(mapid, xy), 0, -1)
    result = pipe.execute()

    xylist = []
    for res in result:
        xylist.extend(res)
    xylist = xylist[:100]
    
    if play_count >= 500:
        conn.setex(const.TAG_MAP_XY_SORT.format(mapid), const.MAP_XY_TIME5, msgpack.dumps(xylist))
    elif play_count >= 100:
        conn.setex(const.TAG_MAP_XY_SORT.format(mapid), const.MAP_XY_TIME4, msgpack.dumps(xylist))
    elif play_count >= 50:
        conn.setex(const.TAG_MAP_XY_SORT.format(mapid), const.MAP_XY_TIME3, msgpack.dumps(xylist))
    else:
        pass

    return xylist

@client.redis_client("map")
def complete_map(conn, mapid, userid, suc, x, y, point, script):
    ''' 打完地图'''

    keyss = None
    argss = None
    if suc == -1:
        argss = [2, mapid, userid, point]
        keyss = [const.TAG_MAP.format(mapid), const.TAG_MAP_POINT.format(mapid)]
    elif suc == 0:
        argss = [1, mapid, userid, point]
        keyss = [const.TAG_MAP.format(mapid), const.TAG_MAP_POINT.format(mapid)]
    else:
        pos = x / 640 * 10000 + y / 640
        if suc == -2:
            argss = [3, mapid, pos, "{0}:{1}".format(x,y)]
        else:
            argss = [0, mapid, pos, "{0}:{1}".format(x,y)]
        keyss = [const.TAG_MAP.format(mapid), const.TAG_MAP_XY.format(mapid),
                 const.TAG_MAP_XY_SET.format(mapid, pos)]
    amount, pas, rank, template_id, date, fav, comment, like = script(keys = keyss, args = argss, client = conn)
    if template_id == '0':
        conn.zadd(const.TAG_RANK_RECENT_POPULAR, util.hot(date, amount), mapid)
        conn.zadd(const.TAG_RANK_MOST_POPULAR_EX, util.popular(amount, pas, fav, comment, like), mapid)
    return (amount, pas, rank, template_id)

@client.redis_client("map")
def view_comment(conn, mapid, script):
    '''查看地图评论'''

    if conn.exists(const.TAG_COMMENT_CACHE.format(mapid)):
        return msgpack.loads(conn.get(const.TAG_COMMENT_CACHE.format(mapid)))

    comments = script(keys = [const.TAG_MAP_COMMENT.format(mapid)], args = [], client = conn)
    conn.set(const.TAG_COMMENT_CACHE.format(mapid), msgpack.dumps(comments))

    return comments

@client.redis_client("map")
def comment_map(conn, mapid, script, keys = [], args = []):
   '''发表评论'''

   return script(keys = keys, args = args, client = conn)


@client.redis_client("map")
def get_rank_map_list(conn, key, start, end, desc):
    '''获取地图排行榜'''

    return conn.zrange(key, start, end, desc, True)

@client.redis_client("map")
def get_map_set(conn, key, map_list, *attr):
    '''根据map_list获取地图属性集合'''

    pipe = conn.pipeline()
    for mapid in map_list:
        pipe.hmget(const.TAG_MAP.format(mapid), *attr)
    return pipe.execute()

@client.redis_client("map")
def get_map_set_ex(conn, key, script, start, end, desc, ver):
    '''使用脚本获取从start到end某个榜单所有地图属性集合'''

    cmd = 0
    if desc:
        cmd = 1
    keys = [key, start, end, cmd]
    args = [ver]
    return script(keys = keys, args = args, client = conn)


@client.redis_client("map")
def get_random_hard_mapids(conn, key, hard, script):
    '''获取相应难度随机地图'''

    keys = [hard]
    args = ["id", "name", "ownername", "country", "head", "ownerid"]
    return script(keys = keys, args = args, client = conn)

