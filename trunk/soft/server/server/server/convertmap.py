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

global_map_path = "./globalmap/"
def export_global_map():
    global global_map_path
    max_value = global_data.get_counter_value(const.TAG_MAP_COUNT)
    max_value = int(max_value)

    for mapid in range(0 + 1, max_value + 1):
        mapdata = map_data.get_map_attr(mapid, "data")
        if mapdata is not None:
            with open(global_map_path + str(mapid) + ".data", "wb") as f:
                f.write(mapdata)

def import_global_map():
    global global_map_path
    files = os.listdir(global_map_path)  
    for f in files:
        print f
        mapid, sufix = f.split(".")
        if map_data.exist_map(mapid):
            if sufix == "data":
                with open(global_map_path + f, "rb") as f1:
                    data_content     = f1.read()
                    map_data.set_map_attr(mapid, "data", data_content)
            if sufix == "url":
                with open(global_map_path + f, "rb") as f2:
                    url_conent = f2.read()
                    map_data.set_map_attr(mapid, "url", url_conent)


test_map_path = "./test/"
def export_test_map():
    global test_map_path

    for mapid in range(10000001, 10000040 + 1):
        mapdata = map_data.get_map_attr(mapid, "data")
        if mapdata is not None:
            with open(test_map_path + str(mapid) + ".data", "wb") as f:
                f.write(mapdata)

def import_test_map():
    global test_map_path
    files = os.listdir(test_map_path)  
    for f in files:
        print f
        mapid, sufix = f.split(".")
        if map_data.exist_map(mapid):
            if sufix == "data":
                with open(test_map_path + f, "rb") as f1:
                    data_content     = f1.read()
                    map_data.set_map_attr(mapid, "data", data_content)
            if sufix == "url":
                with open(test_map_path + f, "rb") as f2:
                    url_conent = f2.read()
                    map_data.set_map_attr(mapid, "url", url_conent)

user_map_path = "./usermap/"
def export_user_map():
    global user_map_path
    max_value = global_data.get_counter_value(const.TAG_USER_COUNT)
    max_value = int(max_value)

    for userid in range(1, max_value + 1):
        usermaplist = user_data.get_usermap_list(userid)
        if usermaplist:
            for mapid in usermaplist:
                if mapid != '0':
                    mapdata = user_data.get_usermap_attr(userid, mapid, "data")
                    if mapdata is not None and mapdata != "":
                        with open(user_map_path + str(userid) + "." + str(mapid) + ".data", "wb") as f:
                            f.write(mapdata)

def import_user_map():
    global user_map_path
    files = os.listdir(user_map_path)  
    for f in files:  
        print f
        userid, mapid, sufix = f.split(".")
        if user_data.exist_usermap(userid, mapid):
            if sufix == "data":
                with open(user_map_path + f, "rb") as f1:
                    data_content     = f1.read()
                    user_data.set_usermap_attr(userid, mapid, "data", data_content)
            if sufix == "url":
                with open(user_map_path + f, "rb") as f2:
                    url_conent = f2.read()
                    user_data.set_usermap_attr(userid, mapid, "url", url_conent)


if __name__ == "__main__":
    init_db()
    import_user_map()