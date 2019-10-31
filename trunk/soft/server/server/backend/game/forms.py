# -*- coding: utf-8 -*-

from django import forms
from django.forms.formsets import BaseFormSet, formset_factory
from django.contrib.auth.forms import AuthenticationForm
from django.utils.translation import ugettext_lazy as _
from models import *
from django.forms import Widget
from bootstrap3_datetime.widgets import DateTimePicker

class LoginForm(AuthenticationForm):
    '''登录表单'''

    username = forms.CharField(max_length=254,
                               widget=forms.TextInput({
                                   'class': 'form-control',
                                   'placeholder': 'User name'}))
    password = forms.CharField(label=_("Password"),
                                    widget=forms.PasswordInput({
                                   'class': 'form-control',
                                   'placeholder':'Password'}))


class ViewUserForm(forms.Form):
    '''查看用户属性表单'''


class ModifyUserForm(forms.Form):
    '''修改用户属性表单'''

    user_pt = forms.ChoiceField(
        required=True,
        label='平台',
        choices=(
            ("1", "yymoon"),
            ("2", "360"),
        ),
    )

    user_account = forms.CharField(
        required=True,
        label='账号',
        max_length=64,
    )

    user_option = forms.ChoiceField(
        required=True,
        label='操作',
        choices=(
            ("1", "增加"),
            ("2", "设置"),
        ),
     )

    user_attr = forms.ModelChoiceField(
        required=True,
        label='属性',
        queryset=UserOption.objects.all(),
    )

    user_value = forms.IntegerField(
        required=True,
        label='属性值',
        min_value = 0,
    )

class ChooseChallengeForm(forms.Form):
    '''编辑选择挑战关卡'''

    map_id1 = forms.IntegerField(
        required=True,
        label='关卡编号1',
        min_value = 10010000,
    )

    map_id2 = forms.IntegerField(
        required=True,
        label='关卡编号2',
        min_value = 10010000,
    )

    map_id3 = forms.IntegerField(
        required=True,
        label='关卡编号3',
        min_value = 10010000,
    )

    map_id4 = forms.IntegerField(
        required=True,
        label='关卡编号4',
        min_value = 10010000,
    )

    map_id5 = forms.IntegerField(
        required=True,
        label='关卡编号5',
        min_value = 10010000,
    )

    map_id6 = forms.IntegerField(
        required=True,
        label='关卡编号6',
        min_value = 10010000,
    )

    map_id7 = forms.IntegerField(
        required=True,
        label='关卡编号7',
        min_value = 10010000,
    )

    map_id8 = forms.IntegerField(
        required=True,
        label='关卡编号8',
        min_value = 10010000,
    )

    map_exp = forms.IntegerField(
        required=True,
        label='挑战经验',
        min_value = 1,
    )

    map_jewel = forms.IntegerField(
        required=True,
        label='挑战钻石',
        min_value = 0,
        max_value = 1000,
    )

    map_cn_subject = forms.CharField(
        required=True,
        label='中文主题',
        max_length=64,
    )

    map_en_subject = forms.CharField(
        required=True,
        label='英文主题',
        max_length=64,
    )

    map_start_date = forms.DateField(
        required=True,
        label='开始时间',
        widget=DateTimePicker(options={"format": "yyyy-mm-dd",
                                       "pickTime": False}),
    )

    map_end_date = forms.DateField(
        required=True,
        label='结束时间',
        widget=DateTimePicker(options={"format": "yyyy-mm-dd",
                                       "pickTime": False}),
    )



class UploadMapForm(forms.Form):
    '''上传地图表单'''


    map_id = forms.IntegerField(
        required=True,
        label='地图编号',
        min_value = 0,
        help_text = "填0上传地图，否则更新地图"
        )

    map_name = forms.CharField(
        required=False,
        label='地图名字',
        max_length=8,
        )

    map_url = forms.FileField(
        required=False,
        label='地图URL',
     )

    map_content = forms.FileField(
        required=False,
        label='地图数据',
     )

class VersionForm(forms.Form):
    """游戏版本号"""

    game_version = forms.CharField(
        required=True,
        label='版本号',
        max_length=16,
        )

    game_type = forms.ChoiceField(
        required=True,
        label='类型',
        choices=(
            ("1", "审核"),
            ("2", "屏蔽"),
        ),
     )


class MapVersionForm(forms.Form):
    """地图版本号"""

    map_version = forms.IntegerField(
        required=True,
        label='地图版本号',
        min_value = 0,
        )


class CreateRankForm(forms.Form):
    """地图版本号"""

    rank_type = forms.ChoiceField(
        required=True,
        label='排行榜类型',
        choices=(
            ("1", "最受欢迎排行榜"),
        ),
     )



