# -*- coding: utf-8 -*-

import random
import math

class LevelTemplate(object):
    """等级模板"""

    def __init__(self):
        self._exp       = 0
        self._support   = 0
        self._life_max  = 0

    @property
    def exp(self):
        return self._exp

    @property
    def support(self):
        return self._support

    @property
    def life_max(self):
        return self._life_max

    @exp.setter
    def exp(self, val):
        if isinstance(val, str):
            if not val.isdigit():
                raise ValueError("str val can not invert ot int")
            self._exp = int(val)
        else:
            raise ValueError("set value should from .txt file")

    @support.setter
    def support(self, val):
        if isinstance(val, str):
            if not val.isdigit():
                raise ValueError("str val can not invert ot int")
            self._support = int(val)
        else:
            raise ValueError("set value should from .txt file")

    @life_max.setter
    def life_max(self, val):
        if isinstance(val, str):
            if not val.isdigit():
                raise ValueError("str val can not invert ot int")
            self._life_max = int(val)
        else:
            raise ValueError("set value should from .txt file")


class MasterTemplate(object):
    """工匠模板"""

    def __init__(self):
        self._exp = 0
        self._upload = 0

    @property
    def exp(self):
        return self._exp

    @property
    def upload(self):
        return self._upload

    @exp.setter
    def exp(self, val):
        if isinstance(val, str):
            if not val.isdigit():
                raise ValueError("str val can not invert ot int")
            self._exp = int(val)
        else:
            raise ValueError("set value should from .txt file")


    @upload.setter
    def upload(self, val):
        if isinstance(val, str):
            if not val.isdigit():
                raise ValueError("str val can not invert ot int")
            self._upload = int(val)
        else:
            raise ValueError("set value should from .txt file")


class ShopTemplate(object):
    """商店模板"""

    def __init__(self):
        self._id    = -1
        self._type  = -1
        self._price = -1
        self._arg   = -1

    @property
    def id(self):
        return self._id

    @property
    def type(self):
        return self._type

    @property
    def price(self):
        return self._price

    @property
    def arg(self):
        return self._arg


    @id.setter
    def id(self, val):
        if isinstance(val, str):
            if not val.isdigit():
                raise ValueError("str val can not invert ot int")
            self._id = int(val)
        else:
            raise ValueError("set value should from .txt file")

    @type.setter
    def type(self, val):
        if isinstance(val, str):
            if not val.isdigit():
                raise ValueError("str val can not invert ot int")
            self._type = int(val)
        else:
            raise ValueError("set value should from .txt file")


    @price.setter
    def price(self, val):
        if isinstance(val, str):
            if not val.isdigit():
                raise ValueError("str val can not invert ot int")
            self._price = int(val)
        else:
            raise ValueError("set value should from .txt file")

    @arg.setter
    def arg(self, val):
        if isinstance(val, str):
            if not val.isdigit():
                raise ValueError("str val can not invert ot int")
            self._arg = int(val)
        else:
            raise ValueError("set value should from .txt file")


class MapTemplate(object):
    """地图模板"""
    
    def __init__(self):
        self._id = 0
        self._support = 0
        self._nextid = 0
        self._hard = 0


    @property
    def id(self):
        return self._id

    @property
    def support(self):
        return self._support

    @property
    def nextid(self):
        return self._nextid

    @property
    def hard(self):
        return self._hard


    @id.setter
    def id(self, val):
        if isinstance(val, str):
            if not val.isdigit():
                raise ValueError("str val can not invert ot int")
            self._id = int(val)
        else:
            raise ValueError("set value should from .txt file")

    @support.setter
    def support(self, val):
        if isinstance(val, str):
            if not val.isdigit():
                raise ValueError("str val can not invert ot int")
            self._support = int(val)
        else:
            raise ValueError("set value should from .txt file")


    @nextid.setter
    def nextid(self, val):
        if isinstance(val, str):
            if not val.isdigit():
                raise ValueError("str val can not invert ot int")
            self._nextid = int(val)
        else:
            raise ValueError("set value should from .txt file")


    @hard.setter
    def hard(self, val):
        if isinstance(val, str):
            if not val.isdigit():
                raise ValueError("str val can not invert ot int")
            self._hard = int(val)
        else:
            raise ValueError("set value should from .txt file")


class MissionTemplate(object):
    '''百人模板'''

    def __init__(self):
        self._life  = 0
        self._jewel = 0


    @property
    def life(self):
        return self._life

    @property
    def jewel(self):
        return self._jewel

    @life.setter
    def life(self, val):
        if isinstance(val, str):
            if not val.isdigit():
                raise ValueError("str val can not invert ot int")
            self._life = int(val)
        else:
            raise ValueError("set value should from .txt file")

    @jewel.setter
    def jewel(self, val):
        if isinstance(val, str):
            if not val.isdigit():
                raise ValueError("str val can not invert ot int")
            self._jewel = int(val)
        else:
            raise ValueError("set value should from .txt file")


class DownloadMapTemplate(object):
    '''下载地图模板'''

    def __init__(self):
        self._price = 0

    @property
    def price(self):
        return self._price

    @price.setter
    def price(self, val):
        if isinstance(val, str):
            if not val.isdigit():
                raise ValueError("str val can not invert ot int")
            self._price = int(val)
        else:
            raise ValueError("set value should from .txt file")



