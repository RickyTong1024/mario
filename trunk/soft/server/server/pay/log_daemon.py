# -*- coding: utf-8 -*-

import redis
import msgpack
import torndb
import logging
import yaml
import logging.config
import tornado
from tornado.options import define, options, parse_command_line

define("redis_host", default="127.0.0.1")
define("redis_port", default=6379, type=int)
define("redis_db", default=0, type=int)

define("mysql_host", default="127.0.0.1:3306")
define("mysql_database", default="malio_pay1")
define("mysql_user", default="root")
define("mysql_password", default="root")


LOGGER = logging.getLogger(__name__)

def main():
    client = redis.StrictRedis(host = options.redis_host, 
                               port = options.redis_port,
                               db   = options.redis_db)
    client.ping()

    db = torndb.Connection(
        host=options.mysql_host, database=options.mysql_database,
        user=options.mysql_user, password=options.mysql_password)

    while True:
        pack = client.blpop(["log:info"], 30)
        if not pack:
            continue
        try:
            data = msgpack.loads(pack[1])
        except Exception, e:
            LOGGER.error("%s",e)
            continue

        op = data.get("op")
        if op == 1 or op == 2:
            try:
                db.insert("insert into login_t (id, openid, userid, pt, login_time, op, dt) values (0, %s, %s, %s, %s, %s, NOW())",
                                data.get("od"),data.get("ud"),data.get("pt"),data.get("dt"),data.get("op"))
            except Exception, e:
                LOGGER.error("%s", e)

if __name__ == "__main__":
    tornado.options.parse_command_line()
    logging.config.dictConfig(yaml.load(open("logging.yaml", 'r')))
    main()
        