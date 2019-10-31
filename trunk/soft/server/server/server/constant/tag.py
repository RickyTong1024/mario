# -*- coding: utf-8 -*-

import const

"""
全局数据
"""
# 用户ID计数器 ->str
const.TAG_USER_COUNT        = "user:count"
# 用户地图计数器 ->str
const.TAG_USERMAP_COUNT     = "usermap:count"
# 地图计数器 ->str
const.TAG_MAP_COUNT         = "map:count"
# 评论计数器 ->str
const.TAG_COMMENT_COUNT     = "comment:count"
# 账户映射用户ID ->hash
const.TAG_OPENID_TO_USERID  = "openid-to-userid"
# 地图搜索
const.TAG_MAP_SEARCH        = "map:search"
# INFO日志 ->list
const.TAG_INFO_LOG          = "log:info"
# 版本号 ->set
const.TAG_GAME_VERSION      = "game:version"
# 版本号非法 ->set
const.TAG_GAME_INVALID_VERSION = "game:invalid:version"
# 礼包 ->set
const.TAG_GAME_LIBAO        = "libao:"
# 礼包奖励 ->hash
const.TAG_GAME_LIBAO_REWARD = "libao-to-reward"
# 挑战地图列表 ->list
const.TAG_CHALLENGE_MAP_LIST = "challenge:map:list"
# 挑战排行->zset
const.TAG_RANK_CHALLENGE     = "rank:challenge"
# 本次挑战排行->zset
const.TAG_RANK_CHALLENGE_CUR = "rank:challenge:current"
# 挑战信息->str
const.TAG_CHALLENGE_INFO     = "challenge:info"


"""
用户数据
"""
# 用户信息 ->hash
const.TAG_USER              = "user:{0}"
# 用户session ->str
const.TAG_USER_SESSION      = "user:session:{0}"
# 用户编辑地图 ->hash
const.TAG_USER_MAP          = "user:{0}:map:{1}"
# 用户编辑的地图 ->list{成员：用户地图ID}
const.TAG_USER_CREATE       = "user:create:{0}"
# 用户收藏的地图 ->zset{成员：地图ID, 分数：0}
const.TAG_USER_FAFOURITE    = "user:fafourite:{0}"
# 用户点赞的地图 ->set{成员：地图ID}
const.TAG_USER_LIKE         = "user:like:{0}"
# 用户上传的地图 ->zset{成员：地图ID，分数：时间}
const.TAG_USER_UPLOAD       = "user:upload:{0}"
# 用户最近玩地图 ->zset{成员：地图ID，分数：时间}
const.TAG_USER_TIME         = "user:time:{0}"
# 用户玩的最多地图 ->zset{成员：地图ID，分数：次数}
const.TAG_USER_AMOUNT       = "user:amount:{0}"
# 用户通关的地图 ->zset{成员：地图ID，分数：积分}
const.TAG_USER_PASS         = "user:pass:{0}"
# 用户最近玩的地图->list
const.TAG_USER_RECENT       = "user:recent:{0}"
# 用户玩的地图录像->hash
const.TAG_USER_VIDEO        = "user:video:{0}"
# 用户百人斩地图列表->list{成员：全局地图ID}
const.TAG_USER_MISSION      = "user:mission:{0}"
# 用户礼包集合 ->set
const.TAG_USER_LIBAO        = "user:libao:{0}"


"""
地图数据
"""
# 最受欢迎排行榜 ->zset:{成员:地图ID, 分数:地图玩的次数}
const.TAG_RANK_MOST_POPULAR_EX  = "rank:popular:ex"
# 最受欢迎排行榜 ->zset:{成员:地图ID, 分数:地图玩的次数}
const.TAG_RANK_MOST_POPULAR     = "rank:popular"
# 最新上传排行榜 ->zset:{成员:地图ID， 分数:时间}
const.TAG_RANK_NEWER_UPLOAD     = "rank:upload"
# 近期热门排行榜 ->zset:{成员:地图ID， 分数:评分}
const.TAG_RANK_RECENT_POPULAR   = "rank:hot"
# 初级难度排行榜 ->zset:{成员:地图ID， 分数:通关率}
const.TAG_RANK_PRIMARY_HARD     = "rank:primary"
# 中级难度排行 ->zset:{成员:地图ID， 分数:通关率}
const.TAG_RANK_MIDDLE_HARD      = "rank:middle"
# 高级难度排行 ->zset:{成员:地图ID， 分数:通关率}
const.TAG_RANK_SENIOR_HARD      = "rank:senior"
# 大师难度排行 ->zset:{成员:地图ID， 分数:通关率}
const.TAG_RANK_MASTER_HARD      = "rank:master"
# 考试地图集合 -zset:{成员:地图模板ID， 分数:地图ID}
const.TAG_RANK_TEST_MAP         = "rank:test"
# 地图版本集合
const.TAG_MAP_VERSION_SET  = "version:mapset"

# 地图 ->hash
const.TAG_MAP           = "map:{0}"
# 地图死亡坐标集合 ->zset
const.TAG_MAP_XY        = "map:xy:{0}"
# 地图死亡坐标集合计数->list
const.TAG_MAP_XY_SET    = "map:{0}:xy:{1}"
# 地图死亡坐标集合临时
const.TAG_MAP_XY_SORT   = "map:xys:{0}"
# 地图评论 ->list
const.TAG_MAP_COMMENT   = "map:comment:{0}"
# 地图最高分积分榜->zset{成员：玩家id, 分数：地图分数}
const.TAG_MAP_POINT     = "map:point:{0}"
# 地图评论信息 ->hash
const.TAG_COMMENT       = "comment:{0}"
# 地图评论信息缓存
const.TAG_COMMENT_CACHE = "comment:cache:{0}"


"""
# 地图status mutable次数->zset
favorite次数
amount次数
pass次数
comment次数
like次数
"""
const.TAG_MAP_STATUS_MUTABLE = "map:status:mutable"
