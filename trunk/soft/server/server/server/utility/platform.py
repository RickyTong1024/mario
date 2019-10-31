# -*- coding: utf-8 -*-

'''平台登录认证Mixin'''

import urllib
import functools
from tornado import escape
from tornado.httpclient import AsyncHTTPClient
from tornado.auth import OAuth2Mixin, _auth_return_future, AuthError

class PlatformAuthMixin(OAuth2Mixin):
    @_auth_return_future
    def qihu360_request(self, path, access_token, callback = None, **args):
        url = "https://openapi.360.cn" + path
        all_args = {}
        all_args["access_token"] = access_token
        all_args["fields"] = "id"
        if args:
            all_args.update(args)

        if all_args:
            url += "?" + urllib.urlencode(all_args)
        http_callback = functools.partial(self._on_qihu360_request, callback)
        http = AsyncHTTPClient()
        http.fetch(url, callback=http_callback)

    def _on_qihu360_request(self, future, response):
        if response.error:
            future.set_exception(AuthError(
                "Error response %s fetching %s" % (response.error,
                                                   response.request.url)))
            return
        future.set_result(escape.json_decode(response.body))

    @_auth_return_future
    def lengjing_request(self, path, access_token, userid, pt, callback = None, **args):
        url = "http://gameproxy.xinmei365.com" + path
        all_args = {}
        all_args["userId"]      = userid
        all_args["channel"]     = pt
        all_args["productCode"] = "test"
        all_args["token"]       = access_token
        if args:
            all_args.update(args)

        if all_args:
            url += "?" + urllib.urlencode(all_args)
        http_callback = functools.partial(self._on_lengjing_request, callback)
        http = AsyncHTTPClient()
        http.fetch(url, callback=http_callback)


    def _on_lengjing_request(self, future, response):
        if response.error:
            future.set_exception(AuthError(
                "Error response %s fetching %s" % (response.error,
                                                   response.request.url)))
            return
        future.set_result(response.body)


