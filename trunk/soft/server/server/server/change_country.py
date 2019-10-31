# -*- coding: utf-8 -*-

import os
import ConfigParser
import redis
from model import user_data
from model import global_data
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


def main():
     for mapid in range(10000001, 10000040 + 1):
         if map_data.exist_map(mapid):
             map_data.set_map_attr(mapid, "country", "US")
         
     for mapid in range(10010001, 10010040 + 1):
         if map_data.exist_map(mapid):
             map_data.set_map_attr(mapid, "country", "US")


if __name__ == "__main__":
    init_db()
    main()