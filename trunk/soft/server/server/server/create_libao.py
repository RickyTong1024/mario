# -*- coding: utf-8 -*-

import time
import random
import redis
import msgpack

client = redis.StrictRedis(host = "192.168.2.66", port = 6379, db = 14)


has_userd_libao_pici    = []
has_userd_libao_reward  = []
has_userd_libao_rewards = []

libao_pici = "01"
libao_type = "0"
libao_reward = "00"
libao_shuliang = 1
libao_rewards = {"life":50}

suffix = ['0','1','2','3','4','5','6','7','8','9',
          'A','B','C','D','E','F','G',
          'H','I','J','K','L','M','N',
          'O','P','Q','R','S','T',
          'U','V','W','X','Y','Z']

has_create = set()

def create_libao():
    t = time.strftime("%y-%m-%d-%H-%M")
    f = open("libao" + t + ".txt", "w")
    prefix = libao_pici + libao_type + libao_reward

    while len(has_create) < libao_shuliang:
        code = prefix
        for i in range(6):
            code += suffix[random.randint(0, len(suffix) - 1)]

        has_create.add(code)

    for s in has_create:
        client.sadd("libao:" + libao_pici, s)
        print >> f, s

    #client.hset("libao-to-reward", libao_reward, msgpack.dumps(libao_rewards))

    f.close()


if __name__ == "__main__":
    create_libao()
