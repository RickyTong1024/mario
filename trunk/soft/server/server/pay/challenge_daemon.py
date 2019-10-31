# -*- coding: utf-8 -*-

import time
import datetime
import redis
import msgpack
import torndb
import logging
import yaml
import logging.config
import tornado
from tornado.options import define, options, parse_command_line

LOGGER = logging.getLogger(__name__)

def main():
    db = torndb.Connection(
        host="192.168.2.66", database= "maliobackend",
        user="root", password="root")

    now_time = time.strftime("%Y-%m-%d")
    challenge_data = db.get("select * from game_challengedata where start_date_str = %s", now_time)
    if not challenge_data:
        db.close()
        return

    globalclient = redis.StrictRedis(host = "192.168.2.66", 
                                     port = 6379,
                                     db   = 14)
    globalclient.ping()

    mapclient = redis.StrictRedis(host = "192.168.2.66", 
                                     port = 6379,
                                     db   = 14)
    mapclient.ping()

    start_log = "**********************ver:{0}***********************************".format(challenge_data.get("id"))
    LOGGER.info(start_log)
    start_date  = challenge_data.get("start_date").strftime("%Y-%m-%d")
    end_date    = challenge_data.get("end_date").strftime("%Y-%m-%d")
    data = dict(version = challenge_data.get("id"),
                exp     = challenge_data.get("exp"),
                jewel   = challenge_data.get("jewel"),
                date    = start_date + "~" + end_date,
                cn_subject = challenge_data.get("cn_subject"),
                en_subject = challenge_data.get("en_subject"))
    data = msgpack.dumps(data)
    globalclient.set("challenge:info", data)
    globalclient.ltrim("challenge:map:list", -1, 0)
    globalclient.zremrangebyrank("rank:challenge", 0, -1)
    globalclient.zremrangebyrank("rank:challenge:current", 0, -1)
    for i in range(8):
        mapid = challenge_data.get("map" + str(i + 1))
        maptag = "map:{0}".format(mapid)
        if mapclient.exists(maptag):
            mapinfo = mapclient.hmget(maptag, "id", "name", "ownername", "ownerid", "country", "head", "data")
            mapinfo = msgpack.dumps(mapinfo)
            globalclient.rpush("challenge:map:list", mapinfo)
    LOGGER.info("**********************************************************")

if __name__ == "__main__":
    tornado.options.parse_command_line()
    logging.config.dictConfig(yaml.load(open("logging.yaml", 'r')))
    main()