# -*- coding: utf-8 -*-

"""用户数据"""

from constant.tag import const
from constant.game import const
from utility import util, client

@client.redis_client("user")
def get_user(conn, userid):
    '''获取用户所有属性'''

    return conn.hgetall(const.TAG_USER.format(userid))

@client.redis_client("user")
def get_user_attr(conn, userid, att):
    '''获取用户一个属性'''

    return conn.hget(const.TAG_USER.format(userid), att)

@client.redis_client("user")
def get_user_attrs(conn, userid, *atts):
    '''获取用户多个属性'''

    return conn.hmget(const.TAG_USER.format(userid), *atts)

@client.redis_client("user")
def inc_user_attr(conn, userid, att, val):
    '''增加用户属性'''

    return conn.hincrby(const.TAG_USER.format(userid), att, val)

@client.redis_client("user")
def inc_user_attrs(conn, userid, atts_list):
    '''增加用户多个属性'''

    pipe = conn.pipeline()
    for item in atts_list:
        pipe.hincrby(const.TAG_USER.format(userid), item[0], item[1])
    pipe.execute()

@client.redis_client("user")
def set_user_attr(conn, userid, att, val):
    '''设置用户属性'''

    conn.hset(const.TAG_USER.format(userid), att, val)

@client.redis_client("user")
def set_user_attrs(conn, userid, mapping):
    '''设置用户多个属性'''

    conn.hmset(const.TAG_USER.format(userid), mapping)

@client.redis_client("user")
def exist_usermap(conn, userid, mapid):
    '''检查用户编辑地图有效性'''

    return conn.exists(const.TAG_USER_MAP.format(userid, mapid))

@client.redis_client("user")
def get_usermap_list(conn, userid):
    '''获取用户编辑列表'''

    return conn.lrange(const.TAG_USER_CREATE.format(userid), 0, -1)

@client.redis_client("user")
def create_usermap_list(conn, userid):
    '''创建用户编辑列表'''

    return conn.rpush(const.TAG_USER_CREATE.format(userid), 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0)

@client.redis_client("user")
def get_usermap_set(conn, userid, mapid_list, *attrs):
    '''获取用户编辑列表地图属性集合'''

    pipe = conn.pipeline()
    for mapid in mapid_list:
        if mapid != '0':
            pipe.hmget(const.TAG_USER_MAP.format(userid, mapid), *attrs)
    return pipe.execute()

@client.redis_client("user")
def get_usermap_set_with_script(conn, userid, script):
    '''使用脚本获取用户编辑列表地图属性集合'''
    keyss = [const.TAG_USER_CREATE.format(userid)]
    argss = [userid]
    return script(keys = keyss, args = argss, client = conn)

@client.redis_client("user")
def get_usermap(conn, userid, mapid):
    '''获取用户地图'''

    return conn.hgetall(const.TAG_USER_MAP.format(userid, mapid))

@client.redis_client("user")
def get_usermap_attr(conn, userid, mapid, attr):
    '''获取用户地图属性'''

    return conn.hget(const.TAG_USER_MAP.format(userid, mapid), attr)

@client.redis_client("user")
def get_usermap_attrs(conn, userid, mapid, *attrs):
    ''' 获取用户地图多个属性'''

    return conn.hmget(const.TAG_USER_MAP.format(userid, mapid), *attrs)

@client.redis_client("user")
def set_usermap_attr(conn, userid, mapid, attr, val):
    ''' 设置用户地图属性'''

    conn.hset(const.TAG_USER_MAP.format(userid, mapid), attr, val)

@client.redis_client("user")
def set_usermap_attrs(conn, userid, mapid, mapping):
    ''' 设置用户地图多个属性'''

    conn.hmset(const.TAG_USER_MAP.format(userid, mapid), mapping)

@client.redis_client("user")
def create_usermap(conn, userid, mapid, index, name = "empty", url = "", data = "", guide = 0, jewel = None):
    '''创建用户编辑地图'''

    pipe = conn.pipeline()
    pipe.lset(const.TAG_USER_CREATE.format(userid), index, mapid)
    pipe.hmset(const.TAG_USER_MAP.format(userid, mapid),
               {"id":mapid,
                "name":name, 
                "url":url, 
                "date":util.now_date_str(), 
                "upload":0, 
                "data":data
                   })
    if guide == 100:
        pipe.hset(const.TAG_USER.format(userid), "guide", guide)
        pipe.hincrby(const.TAG_USER.format(userid), "life", 20)
    if jewel is not None:
        pipe.hincrby(const.TAG_USER.format(userid), "jewel", jewel)
        if guide == 1:
            pipe.hincrby(const.TAG_USER.format(userid), "download_num", 1)
    pipe.execute()

@client.redis_client("user")
def delete_usermap(conn, userid, mapid, index):
    '''删除用户编辑地图'''

    pipe = conn.pipeline()
    pipe.lset(const.TAG_USER_CREATE.format(userid), index, 0)
    pipe.delete(const.TAG_USER_MAP.format(userid, mapid))
    pipe.execute()

@client.redis_client("user")
def check_life(conn, userid, static_data, life_need = 1):
    '''检查生命回复'''

    attr = conn.hmget(const.TAG_USER.format(userid), "life", "life_time", "level")
    now = util.now_time()
    user_life       = int(attr[0])
    user_life_time  = long(attr[1])

    level_template = static_data.get_level_template(int(attr[2]))
    if not level_template:
        return False

    if user_life >= level_template.life_max + const.LIFE_MAX  and user_life - life_need < level_template.life_max + const.LIFE_MAX :
        if user_life - life_need < 0:
            return False
        conn.hset(const.TAG_USER.format(userid), "life_time", now + const.LIEF_TIME)
        return True
    
    if user_life < level_template.life_max + const.LIFE_MAX  and now > user_life_time:
        num = (now - user_life_time) / const.LIEF_TIME + 1
        if user_life + num >= level_template.life_max + const.LIFE_MAX :
            conn.hmset(const.TAG_USER.format(userid),
                        {"life":level_template.life_max + const.LIFE_MAX ,
                        "life_time":now})
            user_life = level_template.life_max + const.LIFE_MAX 
        else:
            conn.hmset(const.TAG_USER.format(userid),
                        {"life":user_life + num,
                        "life_time":user_life_time + (num * const.LIEF_TIME)})
            user_life = user_life + num

    if user_life < life_need:
        return False

    return True


@client.redis_client("user")
def comment_map(conn, userid):
    '''评论地图'''

    pipe = conn.pipeline(False)
    pipe.hincrby(const.TAG_USER.format(userid), "comment", 1)
    pipe.hmget(const.TAG_USER.format(userid), 'head', 'name', 'country', 'visitor')
    return pipe.execute()[-1]


@client.redis_client("user")
def upload_usermap(conn, userid, usermapid, mapid, exp, level, video = ""):
    '''上传地图'''

    pipe = conn.pipeline(False)
    pipe.hmset(const.TAG_USER.format(userid), {"master_level":level, "master_exp":exp})
    pipe.hincrby(const.TAG_USER.format(userid), "upload", 1)
    pipe.hset(const.TAG_USER_MAP.format(userid, usermapid), "upload", 1)
    pipe.zadd(const.TAG_USER_UPLOAD.format(userid), util.now_time(), mapid)
    if video != "":
        pipe.hset(const.TAG_USER_VIDEO.format(userid), mapid, video)
    pipe.execute()


@client.redis_client("user")
def play_map(conn, userid, mapid, mode):
    '''玩地图'''

    user_key = const.TAG_USER.format(userid)
    pipe = conn.pipeline(False)
    pipe.hincrby(user_key, "play", 1)
    if mode == 0 and int(mapid) >= 10010000:
        pipe.hincrby(user_key,"exp", 1)
        pipe.hincrby(user_key,"life", -1)
    elif mode == 1:
        pipe.hincrby(user_key, "mission_life", -1)
        pipe.hincrby(user_key, "mission_total", 1)
    elif mode == 2:
        pipe.hincrby(user_key, "mission", 1)
    elif mode == 3:
        pipe.hincrby(user_key, "mission", 1)
        pipe.hincrby(user_key, "mission_hard", 1)
    pipe.hset(user_key,"mapid", mapid)
    pipe.zincrby(const.TAG_USER_AMOUNT.format(userid), mapid, 1)
    pipe.zadd(const.TAG_USER_TIME.format(userid), util.now_time(), mapid)
    pipe.execute()

@client.redis_client("user")
def complete_map(conn, userid, mapid, suc, point, video, level, exp, rank, support, nextid, usertimer, pass_time):
    '''完成地图'''

    pipe = conn.pipeline()
    if suc == 0:
        if rank != 0:
            pipe.lpush(const.TAG_USER_RECENT.format(userid), "{0}:{1}:{2}".format(util.now_time(), rank, mapid))
            pipe.ltrim(const.TAG_USER_RECENT.format(userid), 0, 4)

        score = conn.zscore(const.TAG_USER_PASS.format(userid), mapid)
        if not score or pass_time < score:
            pipe.zadd(const.TAG_USER_PASS.format(userid), pass_time, mapid)
            if rank >= 1 and rank <= 50:
                pipe.hset(const.TAG_USER_VIDEO.format(userid), mapid, video)

        pipe.hincrby(const.TAG_USER.format(userid), "pass", 1)
        pipe.hincrby(const.TAG_USER.format(userid), "point", point)
    if usertimer:
        pipe.hset(const.TAG_USER.format(userid), "life_time", usertimer)
    if support == 0 and nextid == 0:  
        pipe.hmset(const.TAG_USER.format(userid), {"level":level, "exp":exp})
    elif support != 0 and nextid == 0:
        pipe.hmset(const.TAG_USER.format(userid), {"level":level, "exp":exp, "support":support})
    elif support == 0 and nextid != 0:
        pipe.hmset(const.TAG_USER.format(userid), {"level":level, "exp":exp, "test":nextid})
    else:
        pipe.hmset(const.TAG_USER.format(userid), {"level":level, "exp":exp, "support":support, "test":nextid})
    pipe.execute()

@client.redis_client("user")
def watch_video_map(conn, userid, mapid, same):
    pipe = conn.pipeline(False)
    if same:
        pipe.hincrby(const.TAG_USER.format(userid), "video", 1)
    pipe.hincrby(const.TAG_USER.format(userid), "watched", 1)
    pipe.hget(const.TAG_USER_VIDEO.format(userid), mapid)
    return pipe.execute()[-1]

@client.redis_client("user")
def favourite_map(conn, userid, mapid, love):
    '''收藏地图'''
    if not love:
        conn.zadd(const.TAG_USER_FAFOURITE.format(userid), 1, mapid)
    else:
        conn.zrem(const.TAG_USER_FAFOURITE.format(userid), mapid)

@client.redis_client("user")
def is_favourite_map(conn, userid, mapid):
    '''是否是收藏地图'''

    score = conn.zscore(const.TAG_USER_FAFOURITE.format(userid), mapid)
    return 1 if score else 0

@client.redis_client("user")
def is_pass_map(conn, userid, mapid):
    '''是否通关地图'''

    score = conn.zscore(const.TAG_USER_PASS.format(userid), mapid)
    return 1 if score else 0

@client.redis_client("user")
def is_upload_map(conn, userid, mapid):
    '''是否上传地图'''

    score = conn.zscore(const.TAG_USER_UPLOAD.format(userid), mapid)
    return 1 if score else 0

@client.redis_client("user")
def is_favourite_and_pass_map(conn, userid, mapid_list):
    '''是否收藏通关地图'''

    favourite_key   = const.TAG_USER_FAFOURITE.format(userid)
    pass_key        = const.TAG_USER_PASS.format(userid)

    pipe = conn.pipeline(True)
    for mapid in mapid_list:
        pipe.zscore(favourite_key, mapid)
        pipe.zscore(pass_key, mapid)
    result = pipe.execute()
    return map(lambda x: 1 if x else 0, result)


@client.redis_client("user")
def like_pass_map(conn, userid, mapid, script):
    '''点赞地图'''
    
    keys = [const.TAG_USER_LIKE.format(userid),
            const.TAG_USER_PASS.format(userid)]
    args = [mapid]

    return script(keys = keys, args = args, client = conn)


@client.redis_client("user")
def get_video_data(conn, userid, mapid):
    '''获取通关录像'''

    return conn.hget(const.TAG_USER_VIDEO.format(userid), mapid)

@client.redis_client("user")
def get_range_map_list_with_score(conn, userid, key, start, end, desc):
    '''获取地图列表'''

    return conn.zrange(key, start, end, desc, True)

@client.redis_client("user")
def get_range_map_with_length(conn, userid, key, start, end, desc):
    '''获取地图列表'''

    pipe = conn.pipeline(True)
    pipe.zcard(key)
    pipe.zrange(key, start, end, desc)
    return pipe.execute()

@client.redis_client("user")
def get_recent_map_list(conn, userid):
    '''获取最近玩的地图'''

    return conn.lrange(const.TAG_USER_RECENT.format(userid), 0, -1)

@client.redis_client("user")
def login(conn, userid, user, static_data, jewel):
    '''用户登录'''

    last_login      = long(user.get("last_login", 0))
    upload          = int(user.get("upload", 0))
    life            = int(user.get("life", 20))
    life_time       = long(user.get("life_time", 0))
    level           = int(user.get("level", 1))
    exp             = int(user.get("exp", 0))
    master_level    = int(user.get("master_level", 1))
    master_exp      = int(user.get("master_exp", 0))
    jewel           += int(user.get("jewel", 0))
    test_mapid      = -1
    test_support    = 0
    download_num    = int(user.get("download_num", 0))

    now = util.now_time()

    # 过一天
    if util.trigger_day(last_login):
        upload = 0
        download_num = 0

    # 检查等级
    next_level = level + 1
    while True:
        level_template = static_data.get_level_template(next_level)
        if not level_template:
            exp = 0
            break
        if exp >= level_template.exp:
            exp -= level_template.exp
            level += 1
            next_level += 1
        else:
            break

    # 检查工匠等级
    next_level = master_level + 1
    while True:
        master_template = static_data.get_master_template(next_level)
        if not master_template:
            master_exp = 0
            break
        if master_exp >= master_template.exp:
            master_exp -= master_template.exp                                                                                                                                                                                                                                                                                          
            master_level += 1
            next_level += 1
        else:
            break

    # 检查生命回复
    level_template = static_data.get_level_template(level)
    if level_template:
        if life < level_template.life_max + const.LIFE_MAX  and now > life_time:
            num = (now - life_time) / const.LIEF_TIME + 1
            if life + num >= level_template.life_max + const.LIFE_MAX :
                life = level_template.life_max + const.LIFE_MAX 
                life_time = util.now_time()
            else:
                life = life + num
                life_time = life_time + (num * const.LIEF_TIME)

    # 检查考试地图
    map_template = static_data.get_map_template(int(user.get("test", 0)))
    if map_template:
        test_mapid = map_template.id
        test_support = map_template.hard


    sig = util.make_sig(str(userid))
    pipe = conn.pipeline(False)
    pipe.hmset(const.TAG_USER.format(userid), {"life":life, 
                                               "life_time":life_time, 
                                               "upload":upload,
                                               "exp":exp,
                                               "level":level,
                                               "jewel":jewel,
                                               "master_exp":master_exp,
                                               "master_level":master_level,
                                               "download_num":download_num,
                                               "index":0, 
                                               "last_login":util.now_time()})
    pipe.setex(const.TAG_USER_SESSION.format(userid), const.SIG_EXPIRE_TIME, sig)
    pipe.execute()

    user["upload"]      = upload
    user["life"]        = life
    user["life_time"]   = life_time
    user["exp"]         = exp
    user["level"]       = level
    user["jewel"]       = jewel
    user["sig"]         = sig
    user["test_mapid"]  = test_mapid
    user["test_maphard"] = test_support
    user["download_num"] = download_num

@client.redis_client("user")
def create_visitor(conn, userid, country, pt):
    '''创建游客'''

    passwd, salt = util.create_passwd("empty")
    sig = util.make_sig(str(userid))

    pipe = conn.pipeline(False)
    pipe.setex(const.TAG_USER_SESSION.format(userid), const.SIG_EXPIRE_TIME, sig)
    pipe.hmset(const.TAG_USER.format(userid),
                            {
                            "openid":const.YOUKE_ACCOUNT.format(userid),
                            "openkey":passwd,
                            "opensalt":salt,
                            "channel":pt,
                            "country":country,
                            "name":"empty",
                            "head":0,
                            "visitor":1,
                            "level":1, 
                            "exp":0, 
                            "jewel":0, 
                            "life":const.LIFE_MAX,
                            "life_time":util.now_time(),
                            "play":0,
                            "pass":0,
                            "point":0,
                            "comment":0,
                            "video":0,
                            "watched":0,
                            "upload":0,
                            "master_exp":0,
                            "master_level":1,
                            "support":0,
                            "exp_time":0,
                            "guide":0,
                            "test":10000001,
                            "mission":0,
                            "mission_life":10,
                            "mission_hard":0,
                            "mission_next":0,
                            "mission_total":0,
                            "mission_ver":0,
                            "mission_relive":0,
                            "download_num":0,
                            "index":0,
                            "last_login":util.now_time(),
                            "register":util.now_date_str(),
                                })

    pipe.execute()

    return sig


@client.redis_client("user")
def bind_account(conn, userid, openid, openkey, name, head, country):
    '''绑定账号'''

    passwd, salt = util.create_passwd(openkey)
    pipe = conn.pipeline(False)
    pipe.hmset(const.TAG_USER.format(userid),
                                 {
                                     "openid":openid,
                                     "openkey":passwd,
                                     "opensalt":salt,
                                     "name":name,
                                     "head":head,
                                     "country":country,
                                     "index":0,
                                     "visitor":0
                                  })
    pipe.hincrby(const.TAG_USER.format(userid), "life", 30)
    pipe.execute()


@client.redis_client("user")
def create_pt_account(conn, userid, openid, country, pt):
    '''创建平台账号'''

    sig = util.make_sig(str(userid))

    pipe = conn.pipeline(False)
    pipe.setex(const.TAG_USER_SESSION.format(userid), const.SIG_EXPIRE_TIME, sig)
    pipe.hmset(const.TAG_USER.format(userid),
                            {
                            "openid":openid,
                            "openkey":"",
                            "opensalt":"",
                            "channel":pt,
                            "country":country,
                            "name":"empty",
                            "head":0,
                            "visitor":1,
                            "level":1, 
                            "exp":0, 
                            "jewel":0, 
                            "life":50,
                            "life_time":util.now_time(),
                            "play":0,
                            "pass":0,
                            "point":0,
                            "comment":0,
                            "video":0,
                            "watched":0,
                            "upload":0,
                            "master_exp":0,
                            "master_level":1,
                            "support":0,
                            "exp_time":0,
                            "guide":0,
                            "test":10000001,
                            "mission":0,
                            "mission_life":10,
                            "mission_hard":0,
                            "mission_next":0,
                            "mission_total":0,
                            "mission_ver":0,
                            "mission_relive":0,
                            "download_num":0,
                            "index":0,
                            "last_login":util.now_time(),
                            "register":util.now_date_str(),
                                })

    pipe.execute()

    return sig

@client.redis_client("user")
def has_libao(conn, userid, libao):
    return conn.sismember(const.TAG_USER_LIBAO.format(userid), libao)

@client.redis_client("user")
def add_libao(conn, userid, libao):
    return conn.sadd(const.TAG_USER_LIBAO.format(userid), libao)


@client.redis_client("user")
def challenge_complete(conn, userid, exp, jewel, static_data):
    old_exp, old_level, old_jewel, old_life, old_life_time = conn.hmget(const.TAG_USER.format(userid), 
                                                             "exp", 
                                                             "level",
                                                             "jewel", 
                                                             "life", 
                                                             "life_time")
    old_exp         = int(old_exp) + exp
    old_level       = int(old_level)
    old_life        = int(old_life)
    old_life_time   = long(old_life_time)
    old_jewel       = int(old_jewel) + jewel

    next_level = old_level + 1
    shengji = False
    while True:
        level_template = static_data.get_level_template(next_level)
        if not level_template:
            break
        if old_exp >= level_template.exp:
            old_exp -= level_template.exp
            old_level += 1
            next_level += 1
            shengji = True
        else:
            break

    if shengji:
        level_template = static_data.get_level_template(old_level)
        if level_template:
            if old_life < level_template.life_max + const.LIFE_MAX  and old_life_time < util.now_time():
                old_life_time = util.now_time() + const.LIEF_TIME


    conn.hmset(const.TAG_USER.format(userid), {"exp":old_exp, 
                                               "level":old_level,
                                               "jewel":old_jewel,
                                               "life_time": old_life_time,
                                               "mission":8,
                                               "mission_hard":8})