# -*- coding: utf-8 -*-

import redis
import tornado.web
import torndb
from utility import script, client, executer, msg_factory
from model import game_data
from constant.tag import const

class Application(tornado.web.Application):  
    def __init__(self, handlers, config, **settings):
        self._init_pay(config)
        self._init_script(config)
        self._init_game_data(config)
        self._init_thread_pool(config)
        self._init_db(config)
        self._init_other()
        
        
        tornado.web.Application.__init__(self, handlers, **settings)

    def _init_pay(self, config):
        """
        初始化支付
        """

        self.notify_addr = config.get("pay", "notify_addr")

        pay_host = config.get("pay", "host")
        pay_db = config.get("pay", "db")
        pay_user = config.get("pay", "user")
        pay_pwd = config.get("pay", "pwd")

        self.db = torndb.Connection(host=pay_host,
                                    database=pay_db, 
                                    user=pay_user, 
                                    password=pay_pwd, 
                                    time_zone="+8:00")

    def _init_script(self, config):
        """
        初始化脚本
        """
        self.get_comment_script     = redis.client.Script(None, script.LUA_GET_COMMENT)
        self.create_comment_script  = redis.client.Script(None, script.LUA_CREATE_COMMENT)
        self.lock_script            = redis.client.Script(None, script.LUA_ACQUIRE_LOCK)
        self.unlock_script          = redis.client.Script(None, script.LUA_RELEASE_LOCK)
        self.update_map_script      = redis.client.Script(None, script.LUA_COMPLETE_MAP)
        self.get_map_script         = redis.client.Script(None, script.LUA_GET_MAP)
        self.get_random_map_script  = redis.client.Script(None, script.LUA_GET_RANDOM_MAP)
        self.get_usermap_script     = redis.client.Script(None, script.LUA_GET_USERMAP_SET)
        self.challenge_rank_script  = redis.client.Script(None, script.LUA_CHALLENGE_RANK)
        self.challenge_rank_finish_script = redis.client.Script(None, script.LUA_CHALLENGE_RANK_CURRENT)
        self.like_script            = redis.client.Script(None, script.LUA_LIKE_MAP)


    def _init_game_data(self, config):
        """
        初始化游戏静态数据
        """
        path = config.get("config", "conf")

        self.static_data = game_data.GameData(path)
        self.static_data.parse()

    def _init_thread_pool(self, config):
        """
        初始化线程池
        """

        num_works = config.get("thread", "num")
        if num_works == 0:
            from multiprocessing import cpu_count
            num_works = cpu_count()
        self.executer = executer.AsyncExecutor(num_works)

    def _init_db(self, config):
        """
        初始化数据库
        """

        global_db_num = max(1, config.getint("redis", "global_db_num"))
        client._REDIS_COMPONET["global"] = global_db_num
        host    = config.get("global_db_0", "host")
        port    = config.get("global_db_0", "port")
        db      = config.get("global_db_0", "db")
        map_id  = config.get("global_db_0", "map_init_id")
        conn_pool = redis.ConnectionPool(host = host, port = port, db = db)
        conn = redis.StrictRedis(connection_pool = conn_pool)
        conn.ping()
        conn.setnx(const.TAG_MAP_COUNT, map_id)
        client._REDIS_CONNECTION["global:0"] = conn

        import platform
        if platform.system() != "Windows":
            import redis_search.util
            redis_search.util.redis     = redis.Redis(connection_pool = conn_pool)

        
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

        log_db_num = config.getint("redis", "log_db_num")
        client._REDIS_COMPONET["log"] = log_db_num
        for i in range(log_db_num):
            title = "log_db_" + str(i)
            host  = config.get(title, "host")
            port  = config.get(title, "port")
            db    = config.get(title, "db")
            conn  = redis.StrictRedis(host = host, port = port, db = db)
            conn.ping()
            client._REDIS_CONNECTION['log:' + str(i)] = conn


    def _init_other(self):
        '''初始化额外'''

        self.msg_factory = msg_factory.MsgFactory()

    


    