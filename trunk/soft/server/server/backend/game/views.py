# -*- coding: utf-8 -*-

import binascii
import redis
import time

from django.contrib import messages
from django.template import RequestContext
from django.shortcuts import render_to_response
from django.contrib.auth.decorators import login_required
from django.views.generic import FormView, View, TemplateView
from models import *
from forms import *
import client
from django.template.loaders.filesystem import Loader
from django.contrib.auth.mixins import LoginRequiredMixin



class Index(TemplateView):
    template_name = "index.html"


class ViewUser(LoginRequiredMixin, FormView):
    login_url = '/login/'
    redirect_field_name = 'next'
    template_name = "modifyuser.html"
    form_class = ModifyUserForm


class ModifyUser(LoginRequiredMixin, FormView):
    login_url = '/login/'
    redirect_field_name = 'next'
    template_name = "modifyuser.html"
    form_class = ModifyUserForm

    def form_valid(self, form):
        user_pt      = form.cleaned_data["user_pt"]
        user_account = form.cleaned_data["user_account"]
        user_option  = form.cleaned_data["user_option"]
        user_attr    = form.cleaned_data["user_attr"]
        user_value   = form.cleaned_data["user_value"]

        option = UserOption.objects.get(name = user_attr)

        pt = None
        if user_pt == "1":
            pt = "yymoon"
        elif user_pt == "2":
            pt = "360"

        userid = client.get_userid(user_account, pt)
        if userid is None:
            return render_to_response(self.template_name, RequestContext(self.request, {
            "form":form,
            "result":"用户账号不存在，请检查用户帐户与平台是否正确",
        }))

        
        current_val = client.get_user_att(userid, option.flag)
        if current_val is None:
            return render_to_response(self.template_name, RequestContext(self.request, {
            "form":form,
            "result":"用户账号所关联的用户没有相应数据",
        }))

        result = ""
        if user_option == "1":
            res = client.inc_user_att(userid, option.flag, user_value)
            result = option.name.encode("utf-8") + "成功增加了{0}点,当前值为{1}点".format(user_value, res)
        else:
            res = client.set_user_att(userid, option.flag, user_value)
            result = option.name.encode("utf-8") + "成功设置为了{0}点".format(user_value)
        
        return render_to_response(self.template_name, RequestContext(self.request, {
            "form":form,
            "result":result,
        }))


class StatisticsMixin(object):
    def get_context_data(self, **kwargs):
        context = super(StatisticsMixin, self).get_context_data(**kwargs)

        all_login_data = LoginStatistics.objects.all()
        create_statistics = []
        login_statistics = []
        total_create_statistics = []
        ld = {}
        kd = {}
        cd = {}
        for login_data in all_login_data:
            if ld.has_key(login_data.pingtai):
                ld[login_data.pingtai][login_data.date.strftime("%Y-%m-%d")] = login_data.create_num
                kd[login_data.pingtai][login_data.date.strftime("%Y-%m-%d")] = login_data.login_num
                cd[login_data.pingtai] += login_data.create_num
            else:
                ld[login_data.pingtai] = {login_data.date.strftime("%Y-%m-%d"):login_data.create_num}
                kd[login_data.pingtai] = {login_data.date.strftime("%Y-%m-%d"):login_data.login_num}
                cd[login_data.pingtai] = login_data.create_num
        for k, v in ld.items():
            temp_data = {'data':v,
                         'name':k}
            create_statistics.append(temp_data)

        for k, v in kd.items():
            temp_data = {'data':v,
                         'name':k}
            login_statistics.append(temp_data)

        for k, v in cd.items():
            temp_data = [k,v]
            total_create_statistics.append(temp_data)

        context["login_create"] = create_statistics
        context["login_login"]  = login_statistics
        context["create_total"]  = total_create_statistics
        return context


class StatisticsView(StatisticsMixin, LoginRequiredMixin, TemplateView):
    template_name = "statistics.html"
    login_url = '/login/'
    redirect_field_name = 'next'


class MapStatisticsMixin(object):
    def get_context_data(self, **kwargs):
        context = super(MapStatisticsMixin, self).get_context_data(**kwargs)

        map_data = MapStatistics.objects.all()
        for map in map_data:
            pass

        context["map_id"] = None

class MapStatisticsView(MapStatisticsMixin, LoginRequiredMixin, TemplateView):
    template_name = "mapstatistics.html"
    login_url = '/login/'
    redirect_field_name = 'next'


class ChooseChallengeView(LoginRequiredMixin, FormView):
    login_url = '/login/'
    redirect_field_name = 'next'
    template_name = "choosechallenge.html"
    form_class = ChooseChallengeForm

    def get_context_data(self, **kwargs):
        context = super(ChooseChallengeView, self).get_context_data(**kwargs)

        data = ChallengeData.objects.order_by("-id")[0:5]
        context["lines"] = data
        
        return context


    def form_valid(self, form):
        map_id1      = form.cleaned_data["map_id1"]
        map_id2      = form.cleaned_data["map_id2"]
        map_id3      = form.cleaned_data["map_id3"]
        map_id4      = form.cleaned_data["map_id4"]
        map_id5      = form.cleaned_data["map_id5"]
        map_id6      = form.cleaned_data["map_id6"]
        map_id7      = form.cleaned_data["map_id7"]
        map_id8      = form.cleaned_data["map_id8"]
        map_exp      = form.cleaned_data["map_exp"]
        map_jewel    = form.cleaned_data["map_jewel"]
        map_cn_subject  = form.cleaned_data["map_cn_subject"]
        map_en_subject  = form.cleaned_data["map_en_subject"]
        map_start_date  = form.cleaned_data["map_start_date"]
        map_end_date    = form.cleaned_data["map_end_date"]

        if map_start_date > map_end_date:
            return render_to_response(self.template_name, RequestContext(self.request, {
                "form":form,
                "result":"开始时间不能大于结束时间",
            }))

        data = ChallengeData.objects.order_by("-id")[0:1]
        if data:
            if map_start_date <= data[0].end_date:
                return render_to_response(self.template_name, RequestContext(self.request, {
                "form":form,
                "result":"活动时间不能有相互交叉",
            }))


        for i in range(8):
            mapid = form.cleaned_data["map_id" + str(i + 1)]
            if not client.exist_map(mapid):
                return render_to_response(self.template_name, RequestContext(self.request, {
                "form":form,
                "result":"编号为{0}的地图不存在！！！".format(mapid),
            }))

        challenge_data = ChallengeData(map1 = map_id1,
                                       map2 = map_id2,
                                       map3 = map_id3,
                                       map4 = map_id4,
                                       map5 = map_id5,
                                       map6 = map_id6,
                                       map7 = map_id7,
                                       map8 = map_id8,
                                       exp = map_exp,
                                       jewel = map_jewel,
                                       cn_subject = map_cn_subject,
                                       en_subject = map_en_subject,
                                       start_date = map_start_date,
                                       end_date = map_end_date,
                                       start_date_str = map_start_date.strftime("%Y-%m-%d"))
        challenge_data.save()

        return render_to_response(self.template_name, RequestContext(self.request, {
                "form":form,
                "result":"添加成功！！！",
                "lines":ChallengeData.objects.order_by("-id")[0:5]
            }))



class UploadMapView(LoginRequiredMixin, FormView):
    login_url = '/login/'
    redirect_field_name = 'next'
    template_name = "uploadmap.html"
    form_class = UploadMapForm

    def form_valid(self, form):
       map_id   = form.cleaned_data["map_id"]
       map_name = form.cleaned_data["map_name"]
       map_url  = form.cleaned_data["map_url"]
       map_data = form.cleaned_data["map_content"]

       res = -1
       if map_id == 0:
           if map_name == "":
               return render_to_response(self.template_name, RequestContext(self.request, {
                "form":form,
                "result":"上传地图名字不能为空",
                }))

           if map_url is None:
               return render_to_response(self.template_name, RequestContext(self.request, {
                "form":form,
                "result":"上传地图url不能为空",
                }))

           if map_data is None:
               return render_to_response(self.template_name, RequestContext(self.request, {
                "form":form,
                "result":"上传地图data不能为空",
                }))

           url_content  = map_url.read()
           data_content = map_data.read()

           map_id = client.get_counter("map:count")
           res = client.upload_map(map_id,
                             True,
                             id = map_id,
                             name = map_name,
                             url = url_content,
                             ownerid = 0,
                             ownername = "BoxMaker",
                             country = "US",
                             head = 1,
                             favorite = 0,
                             amount = 0,
                             passs = 0,
                             template = 0,
                             date = time.strftime("%Y-%m-%d %H:%M"),
                             hard = 0,
                             data = data_content)

           if res == 0:
               upload_map_data = UploadMapData(mapid = map_id,
                                               mapname = map_name)
               upload_map_data.save()

       else:
            args = {}
            if map_name != "":
                args.update({"name":map_name})
            if map_url is not None and map_data is not None:
                url_content  = map_url.read()
                data_content = map_data.read()
                args.update({"url":url_content,
                             "data":data_content})

            if args:
                res = client.upload_map(map_id,
                                  False,
                                  **args)
            else:
                return render_to_response(self.template_name, RequestContext(self.request, {
                    "form":form,
                    "result":"没有填写需要修改的地图参数",
                }))


       return_result = "系统错误"
       if res == 0:
           return_result = "上传地图或修改地图成功!!!"
       elif res == -2:
           return_result = "编号为{0}的题图不存在!!!".format(map_id)

       return render_to_response(self.template_name, RequestContext(self.request, {
                    "form":form,
                    "result":return_result,
                }))


class VersionView(LoginRequiredMixin, FormView):
    login_url = '/login/'
    redirect_field_name = 'next'
    template_name = "version.html"
    form_class = VersionForm

    def get_context_data(self, **kwargs):
        context = super(VersionView, self).get_context_data(**kwargs)

        context["lines"] = Version.objects.all()
        
        return context


    def form_valid(self, form):
        game_version      = form.cleaned_data["game_version"]
        game_type         = form.cleaned_data["game_type"]


        client.add_version(game_type, game_version)

        version = Version(version = game_version,
                          ttype = int(game_type))
        version.save()
        

        return render_to_response(self.template_name, RequestContext(self.request, {
                "form":form,
                "result":"添加成功！！！",
                "lines":Version.objects.all()
            }))


class MapVersionView(LoginRequiredMixin, FormView):
    login_url = '/login/'
    redirect_field_name = 'next'
    template_name = "mapversion.html"
    form_class = MapVersionForm

    def get_context_data(self, **kwargs):
        context = super(MapVersionView, self).get_context_data(**kwargs)

        context["lines"] = MapVersion.objects.order_by("-id")[0:5]
        
        return context


    def form_valid(self, form):
        map_version      = form.cleaned_data["map_version"]

        has_version = None
        try:
            has_version = MapVersion.objects.get(version = map_version)
        except:
            has_version = None

        if has_version:
            return render_to_response(self.template_name, RequestContext(self.request, {
                "form":form,
                "result":"版本已经存在！！！请重新确认地图版本号",
                "lines":MapVersion.objects.order_by("-id")[0:5]
            }))

        client.update_map_version(map_version)

        version = MapVersion(version = map_version)
        version.save()
        
        return render_to_response(self.template_name, RequestContext(self.request, {
                "form":form,
                "result":"设置成功！！！",
                "lines":MapVersion.objects.order_by("-id")[0:5]
            }))



class CreateRankView(LoginRequiredMixin, FormView):
    login_url = '/login/'
    redirect_field_name = 'next'
    template_name = "createrank.html"
    form_class = CreateRankForm


    def form_valid(self, form):
        rank_type      = form.cleaned_data["rank_type"]

        rank = ""
        if rank_type == "1":
            rank = "rank:popular:ex"
        else:
            return render_to_response(self.template_name, RequestContext(self.request, {
                "form":form,
                "result":"无效的排行榜类型！！！",
            }))


        client.create_rank(rank)
        
        return render_to_response(self.template_name, RequestContext(self.request, {
                "form":form,
                "result":"生成成功！！！",
            }))


        
        





