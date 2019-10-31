# -*- coding: utf-8 -*-

import msgpack
import os
import ConfigParser
import redis
from model import user_data
from model import global_data, cache_data
from model import map_data
from utility import client
from constant.tag import const

config_file_path = os.path.join(os.path.dirname(os.path.abspath(__file__)), "settings.conf")

def init_db():
    config = ConfigParser.ConfigParser()
    config.read(config_file_path)

    global_db_num = max(1, config.getint("redis", "global_db_num"))
    client._REDIS_COMPONET["global"] = global_db_num
    host    = config.get("global_db_0", "host")
    port    = config.get("global_db_0", "port")
    db      = config.get("global_db_0", "db")
    conn_pool = redis.ConnectionPool(host = host, port = port, db = db)
    conn = redis.StrictRedis(connection_pool = conn_pool)
    conn.ping()
    client._REDIS_CONNECTION["global:0"] = conn

        
    map_db_num = config.getint("redis", "map_db_num")
    client._REDIS_COMPONET["map"] = map_db_num
    for i in range(map_db_num):
        title = "map_db_" + str(i)
        host  = config.get(title, "host")
        port  = config.get(title, "port")
        db    = config.get(title, "db")
        conn  = redis.StrictRedis(host = host, port = port, db = db)
        conn.ping()
        client._REDIS_CONNECTION['map:' + str(i)] = conn

    user_db_num = config.getint("redis", "user_db_num")
    client._REDIS_COMPONET["user"] = user_db_num
    for i in range(user_db_num):
        title = "user_db_" + str(i)
        host  = config.get(title, "host")
        port  = config.get(title, "port")
        db    = config.get(title, "db")
        conn  = redis.StrictRedis(host = host, port = port, db = db)
        conn.ping()
        client._REDIS_CONNECTION['user:' + str(i)] = conn


mapids = [11, 11, 11, 11, 11, 11, 11, 11]

def main():
    exp = 200
    date = "2016-01-15~2016-01-25"
    data = dict(exp=200, date="2016-01-15~2016-01-25", version=9,
                cn_subject = "官方",
                en_subject = "official")
    data = msgpack.dumps(data)
    cache_data.set_cache(const.TAG_CHALLENGE_INFO, data)

    global_data.remove_challenge_map(const.TAG_CHALLENGE_MAP_LIST)
    for mapid in mapids:
        if map_data.exist_map(mapid):
            mapinfo = map_data.get_map_attrs(mapid, "id", "name", "ownername", "ownerid", "country", "head", "data")
            mapinfo = msgpack.dumps(mapinfo)
            global_data.add_challenge_map(const.TAG_CHALLENGE_MAP_LIST, mapinfo)

if __name__ == "__main__":
    init_db()
    main()