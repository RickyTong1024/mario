# -*- coding: utf-8 -*-

"""游戏静态数据"""

import template
from utility import dbc

class GameData(object):
    def __init__(self, path):
        self._path      = path
        self._levels    = {}
        self._shops     = {}
        self._masters   = {}
        self._maps      = {}
        self._missions  = {}
        self._downloads = {}
        self._download_max = 0


    def parse(self):
        self._parse_level()
        self._parse_shop()
        self._parse_master()
        self._parse_map()
        self._parse_mission()
        self._parse_download()

    def get_level_template(self, level):
        return self._levels.get(level, None)

    def get_shop_template(self, id):
        return self._shops.get(id, None)

    def get_master_template(self, level):
        return self._masters.get(level, None)

    def get_map_template(self, id):
        return self._maps.get(id, None)

    def get_mission_template(self, life):
        return self._missions.get(life, None)

    def get_download_template(self, count):
        count = count + 1
        if count > self._download_max:
            count = self._download_max
        return self._downloads.get(count, None)

    def _parse_level(self):
        dbc_file = dbc.dbc()
        dbc_file.load(self._path, "t_exp.txt")

        for i in range(dbc_file.get_y()):
            data = template.LevelTemplate()
            data.exp        = dbc_file.get(1, i)
            data.support    = dbc_file.get(2, i)
            data.life_max   = dbc_file.get(4, i)
            self._levels[int(dbc_file.get(0,i))] = data


    def _parse_shop(self):
        dbc_file = dbc.dbc()
        dbc_file.load(self._path, "t_shop.txt")

        for i in range(dbc_file.get_y()):
            data = template.ShopTemplate()
            data.id     = dbc_file.get(0, i)
            data.type   = dbc_file.get(3, i)
            data.price  = dbc_file.get(4, i)
            data.arg    = dbc_file.get(8, i)
            self._shops[data.id] = data


    def _parse_master(self):
        dbc_file = dbc.dbc()
        dbc_file.load(self._path, "t_job_exp.txt")

        for i in range(dbc_file.get_y()):
            data = template.MasterTemplate()
            data.exp    = dbc_file.get(1, i)
            data.upload = dbc_file.get(2, i)
            self._masters[int(dbc_file.get(0,i))] = data


    def _parse_map(self):
        dbc_file = dbc.dbc()
        dbc_file.load(self._path, "t_map.txt")

        for i in range(dbc_file.get_y()):
            data = template.MapTemplate()
            data.id         = dbc_file.get(0, i)
            data.support    = dbc_file.get(1, i)
            data.nextid     = dbc_file.get(2, i)
            data.hard       = dbc_file.get(3, i)
            self._maps[data.id] = data


    def _parse_mission(self):
        dbc_file = dbc.dbc()
        dbc_file.load(self._path, "t_br.txt")

        for i in range(dbc_file.get_y()):
            data = template.MissionTemplate()
            data.life       = dbc_file.get(0, i)
            data.jewel      = dbc_file.get(1, i)
            self._missions[int(dbc_file.get(0,i))] = data


    def _parse_download(self):
        dbc_file = dbc.dbc()
        dbc_file.load(self._path, "t_download.txt")

        for i in range(dbc_file.get_y()):
            data = template.DownloadMapTemplate()
            self._download_max = int(dbc_file.get(0, i))
            data.price = dbc_file.get(1, i)
            self._downloads[int(dbc_file.get(0, i))] = data