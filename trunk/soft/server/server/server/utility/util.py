# -*- coding: utf-8 -*-

'''一些工具函数'''


import hashlib
import base64
import time
import datetime
import math
import re

def make_sig(openid):
    '''生成sig'''

    m = hashlib.md5()            
    m.update(openid)
    m.update(time.strftime("%Y-%m-%d %H:%M%S")) 
    return m.hexdigest()[8:-8]

def create_passwd(openkey):
    '''生成密码'''

    salt = "$" + str(long(time.time())) + "$"
    passwd = openkey + salt
    m = hashlib.md5()
    m.update(passwd)
    return (m.hexdigest(), salt)  

def check_passwd(openkey, salt, passwd):
    '''校验密码'''

    psd = openkey + salt
    m = hashlib.md5()
    m.update(psd)
    return m.hexdigest() == passwd


def check_input(input, max, check_space = False, check_numalp = False):
    '''检测输入'''

    l = len(input)
    if l == 0 or l > max:
        return False
    if check_space:
        if input.find(" ") != -1:
            return False
    if check_numalp:
        if not re.match('^[a-zA-Z0-9]+$', input):
            return False
    return True

def check_input_nospace(input, max, check_numalp = False):
    '''检测输入不包括空格不能全是空格'''

    l = len(input)
    if l == 0 or l > max:
        return False
    
    if input.find(" ") == 0 or input.rfind(" ") == l -1:
        return False

    if check_numalp:
        if not re.match('^[a-zA-Z0-9]+$', input):
            return False
    return True

def now_time():
    '''获取当前时间(单位毫秒)'''

    return long(time.time() * 1000)

def now_date_str():
    '''获取当前日期字符表示'''

    return time.strftime("%Y-%m-%d %H:%M")

def sec_to_str(secs):
    '''秒数转换为字符表示'''

    return time.strftime("%Y-%m-%d %H:%M", time.localtime(secs))

def msc_to_str(msecs):
    '''毫秒数转换为字符表示'''

    sec =  msecs / 1000.0
    return sec_to_str(sec)

def trigger_day(msecs):
    '''从上次登录到现在登录是否过一天'''

    now = datetime.datetime.now()
    last_login = datetime.datetime.fromtimestamp(msecs / 1000.0)
    dif = now - last_login
    if dif.days >= 1:
        return True
    if now.year != last_login.year or now.month != last_login.month or now.day != last_login.day:
        return True
    return False

def hot(date, num):
    '''
    计算热度:
    权重为时间和总数量
    '''

    td = datetime.datetime.strptime(date, "%Y-%m-%d %H:%M") - datetime.datetime(1970, 1, 1)
    t = td.days * 86400 + td.seconds + (float(td.microseconds) / 1000000)
    secs = t - 1445702400.0
    return round(max(math.log10(num), 1) + secs / 45000, 7)

def popular(play_num, pass_num, collect_num, comment_num, like_num):
    '''
    计算欢迎
    1.0:简单线性叠加
    '''

    # 兼容
    if comment_num is None:
        comment_num = 0
    if like_num is None:
        like_num = 0

    collect_num = int(collect_num)
    comment_num = int(comment_num)
    like_num = int(like_num)

    return play_num + pass_num * 10 + comment_num * 20 + collect_num * 50 + like_num * 100


def base64_url_decode(inp):
    return base64.urlsafe_b64decode(str(inp + '=' * (4 - len(inp) % 4)))