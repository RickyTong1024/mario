# -*- coding: utf-8 -*-

from django.db import models

class UserOption(models.Model):
    name = models.CharField(max_length = 36)
    flag = models.CharField(max_length = 36)

    def __unicode__(self):
        return self.name

    class Meta:
        ordering = ["name"]


class LoginStatistics(models.Model):
    date        = models.DateField()
    create_num  = models.IntegerField()
    login_num   = models.IntegerField()
    pingtai     = models.CharField(max_length = 36)


class MapStatistics(models.Model):
    date       = models.DateField()
    map_id     = models.IntegerField()
    play_num   = models.IntegerField()
    player_num = models.IntegerField()


class ChallengeData(models.Model):
    map1       = models.IntegerField()
    map2       = models.IntegerField()
    map3       = models.IntegerField()
    map4       = models.IntegerField()
    map5       = models.IntegerField()
    map6       = models.IntegerField()
    map7       = models.IntegerField()
    map8       = models.IntegerField()
    exp        = models.IntegerField()
    jewel      = models.IntegerField()
    cn_subject = models.CharField(max_length = 128)
    en_subject = models.CharField(max_length = 128)
    start_date = models.DateField()
    end_date   = models.DateField()
    start_date_str = models.CharField(max_length = 64)

class UploadMapData(models.Model):
    mapid   = models.IntegerField()
    mapname = models.CharField(max_length = 128)

class Version(models.Model):
    version  = models.CharField(max_length = 32)
    ttype    = models.IntegerField()

class MapVersion(models.Model):
    version  = models.IntegerField()


    
