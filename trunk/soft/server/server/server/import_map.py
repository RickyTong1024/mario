# -*- coding: utf-8 -*-

import os
from utility import dbc, util
from constant.tag import const
import redis

config_file_path = os.path.join(os.path.dirname(os.path.abspath(__file__)),"settings.ini")
conf_path = "../../config/"
map_path  = "../../../../scheme/mission/"
client = redis.StrictRedis(host = "192.168.2.66", port = 6379, db = 14)

def import_map(template_id, name, url, data, hard):
    url_content = ""
    data_content = ""

    try:
        with open(map_path + url, "rb") as f:
            url_content     = f.read()
        with open(map_path + data, "rb") as f:
            data_content    = f.read()
    except:
        return

    client.hmset(const.TAG_MAP.format(template_id),
        {"id":template_id,
        "name":name,
        "url":url_content,
        "ownerid":0,
        "ownername":"BoxMaker",
        "country":"CN",
        "head":1,
        "favorite":0,
        "amount":0,
        "pass":0,
        "template":template_id,
        "date":util.now_date_str(),
        "hard":hard,
        "data":data_content})
    client.zadd(const.TAG_RANK_TEST_MAP, template_id, template_id)


def import_map_ex(template_id, name, url, data, hard):
    url_content = ""
    data_content = ""

    try:
        with open(map_path + url, "rb") as f:
            url_content     = f.read()
        with open(map_path + data, "rb") as f:
            data_content    = f.read()
    except:
        return

    mapid = client.incr(const.TAG_MAP_COUNT)
    client.hmset(const.TAG_MAP.format(mapid),
        {"id":mapid,
        "name":name,
        "url":url_content,
        "ownerid":0,
        "ownername":"BoxMaker",
        "country":"CN",
        "head":1,
        "favorite":0,
        "amount":0,
        "pass":0,
        "template":0,
        "date":util.now_date_str(),
        "hard":0,
        "data":data_content})
    client.zadd(const.TAG_RANK_MOST_POPULAR, 0, mapid)
    client.zadd(const.TAG_RANK_NEWER_UPLOAD, util.now_time(), mapid)
    client.sadd(const.TAG_PRIMARY_SET, mapid)

def main():
    dbcfile = dbc.dbc()
    dbcfile.load(conf_path, "t_map.txt")

    for i in range(dbcfile.get_y()):
        id = int(dbcfile.get(0, i))
        name = dbcfile.get(4, i)
        url = dbcfile.get(5,i)
        data = dbcfile.get(6,i)
        hard = dbcfile.get(3,i)

        import_map(id, name, url, data, hard)


if __name__ == "__main__":
    main()


