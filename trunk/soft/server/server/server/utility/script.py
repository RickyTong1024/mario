# -*- coding: utf-8 -*-

'''redis脚本'''

# 获取评论
LUA_GET_COMMENT = """
local map_comment_list = redis.call('lrange', KEYS[1], 0, -1)
if not map_comment_list then
    return false
end
local comments = {}
for i, v in ipairs(map_comment_list) do
    local key = string.format('comment:%s', v)
    comments[i] = redis.call('hgetall', key)
end
return comments
"""

# 发表评论
LUA_CREATE_COMMENT = """
local id = redis.call('incr', KEYS[1])
local key = string.format('comment:%s', id)
redis.call('hmset', key, unpack(ARGV))
redis.call('lpush', KEYS[2], id)
local len = redis.call('llen', KEYS[2])
if len > 10 then
    local delete_id = redis.call('rpop', KEYS[2])
    redis.call('del', string.format('comment:%s', delete_id))
end
redis.call('hincrby', KEYS[4], "comment", 1)
redis.call('del', KEYS[3])
"""

# 获取锁
LUA_ACQUIRE_LOCK = """
if redis.call('exists', KEYS[1]) == 0 then             
    return redis.call('setex', KEYS[1], unpack(ARGV))
end
"""

# 释放锁
LUA_RELEASE_LOCK = """
if redis.call('get', KEYS[1]) == ARGV[1] then          
    return redis.call('del', KEYS[1]) or true
end
"""

# 完成地图
LUA_COMPLETE_MAP = """
local amount = redis.call('hincrby', KEYS[1], 'amount', 0)
local pass_count = (ARGV[1] == '0' or ARGV[1] == '3') and 0 or 1
local pass = redis.call('hincrby', KEYS[1], 'pass', pass_count)
local mapattrs = redis.call('hmget', KEYS[1], 'hard', 'template', 'date', "favorite", "comment", "like")
if mapattrs[2] == '0' then
    local tgl = pass / amount
    local maphard = tonumber(mapattrs[1])
    local hard = 1
    local hard_key = {'rank:primary', 'rank:middle', 'rank:senior', 'rank:master'}
    if amount > 10000 then
        if tgl < 0.0005 then
            hard = 4
        elseif tgl < 0.005 then
            hard = 3
        elseif tgl < 0.05 then
            hard = 2
        end
    elseif amount > 1000 then
        if tgl < 0.005 then
            hard = 3
        elseif tgl < 0.05 then
            hard = 2
        end
    elseif amount > 100 then
        if tgl < 0.05 then
            hard = 2
        end
    else
        hard = 0
    end
    if maphard ~= hard then
        redis.call('hset', KEYS[1], 'hard', hard)
        if maphard ~= 0 then
            redis.call('zrem', hard_key[maphard], ARGV[2])
        end
    end
    if hard ~= 0 then
        redis.call('zadd', hard_key[hard], tgl, ARGV[2])
    end
end
local rank = -1
if ARGV[1] == '1' then
    local score = redis.call('zscore', KEYS[2], ARGV[3])
    if not score or tonumber(ARGV[4]) < tonumber(score) then
        redis.call('zadd', KEYS[2], ARGV[4], ARGV[3])
        redis.call('zremrangebyrank', KEYS[2], 50, -1)
    end
    rank = redis.call('zrank', KEYS[2], ARGV[3])
    if rank == false then
        rank = -1
    end
elseif ARGV[1] == '0' or ARGV[1] == '3' then 
    local num = redis.call('zincrby', KEYS[2], 1, ARGV[3])
    if tonumber(num) <= 10 then
        redis.call('lpush', KEYS[3], ARGV[4])
    end   
end    
return {amount, pass, rank + 1, mapattrs[2], mapattrs[3], mapattrs[4], mapattrs[5], mapattrs[6]}
"""

# 获取地图集合，并返回地图集合长度
LUA_GET_MAP = """
local cmd = 'zrange'
if KEYS[4] == '1' then
    cmd = 'zrevrange'
end
local ver = redis.call('get', 'version:mapset')
local rank_key = KEYS[1]
if ver and tonumber(ARGV[1]) < tonumber(ver) then
    rank_key = string.format('%s:version:%s', rank_key, ARGV[1])
end
local map_list = redis.call(cmd, rank_key, KEYS[2], KEYS[3])
local maps = {}
for i, v in ipairs(map_list) do
    local key = string.format('map:%s', v)
    maps[i] = redis.call('hmget', key, "id", "name", "url", "amount", "pass", "template", "hard", "like")
end
local map_list_len = redis.call('zcard', rank_key)
return {map_list_len, maps}
"""

# 从地图难度集合中随机获取地图集合
LUA_GET_RANDOM_MAP = """
local hard_key = {'set:primary', 'set:middle', 'set:senior', 'set:master'}
local hard = tonumber(KEYS[1])
local count = (hard >= 3) and 20 or 12
local hard_set = {}
while (hard > 1) do
    local num = redis.call('scard', hard_key[hard])
    if num < count then
        hard_set[hard] = num
        count = count - num
    else
        break
    end
    hard = hard - 1
end
hard_set[hard] = count
local maps = {}
local indexs = 1
for k, v in pairs(hard_set) do
    if v ~= 0 then
        local map_list = redis.call('srandmember', hard_key[k], v)
        for i, iv in ipairs(map_list) do
            local key = string.format('map:%s', iv)
            maps[indexs] = redis.call('hmget', key, unpack(ARGV))
            indexs = indexs + 1
        end
    end
end
return maps
"""

# 获取用户地图集合
LUA_GET_USERMAP_SET = """
local map_list = redis.call('lrange', KEYS[1], 0, -1)
if not map_list then
    return false
end
local maps = {}
local key = nil
for k, v in ipairs(map_list) do
    if v ~= '0' then
        key = string.format('user:%s:map:%s', ARGV[1], v)
        maps[k] = redis.call('hmget', key, 'id', 'name', 'url', 'date', 'upload')
    else
        maps[k] = {}
    end
end
return maps 
"""

# 挑战排行榜
LUA_CHALLENGE_RANK = """
local score = redis.call('zscore', 'rank:challenge', ARGV[1])
if ARGV[3] == '0' or not score or tonumber(ARGV[2]) > tonumber(score) then
    redis.call('zadd', 'rank:challenge', ARGV[2], ARGV[1])
    redis.call('zremrangebyrank', 'rank:challenge', 0, -51)
end
"""

# 本次结束排行榜
LUA_CHALLENGE_RANK_CURRENT = """
redis.call('zadd', 'rank:challenge:current', ARGV[2], ARGV[1])
redis.call('zremrangebyrank', 'rank:challenge:current', 0, -51)
local rank = redis.call('zrevrank', 'rank:challenge:current', ARGV[1])
if rank == false then
    rank = -1
end
return rank + 1
"""

# 点赞地图
LUA_LIKE_MAP = """
local like = redis.call('sismember', KEYS[1], ARGV[1])
local score = redis.call('zscore', KEYS[2], ARGV[1])
if like == 0 and score then
    redis.call('sadd', KEYS[1], ARGV[1])
    return 1
end
return 0
"""



