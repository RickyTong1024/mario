"""
Definition of urls for backend.
"""

from datetime import datetime
from django.conf.urls import patterns, url
from game.forms import LoginForm
from game.views import Index, ModifyUser, StatisticsView, ChooseChallengeView, UploadMapView, VersionView
from game.views import MapVersionView, CreateRankView

# Uncomment the next lines to enable the admin:
from django.conf.urls import include
from django.contrib import admin
admin.autodiscover()

urlpatterns = patterns('',
    url(r'^$', Index.as_view()),
    url(r'^modifyuser', ModifyUser.as_view(), name='modifyuser'),
    url(r'^statistics', StatisticsView.as_view(), name='statistics'),
    url(r'^choose', ChooseChallengeView.as_view(), name='choose'),
    url(r'^upload', UploadMapView.as_view(), name='uploadmap'),
    url(r'^version', VersionView.as_view(), name='version'),
    url(r'^mapversion', MapVersionView.as_view(), name='mapversion'),
    url(r'^createrank', CreateRankView.as_view(), name = 'createrank'),
    # Examples:
    #url(r'^$', 'app.views.home', name='home'),
    #url(r'^contact$', 'app.views.contact', name='contact'),
    #url(r'^about', 'app.views.about', name='about'),
    url(r'^login/$',
        'django.contrib.auth.views.login',
        {
            'template_name': 'login.html',
            'authentication_form': LoginForm,
            'extra_context':
            {
                'title':'登录',
                'year':datetime.now().year,
            }
        },
        name='login'),
    url(r'^logout$',
        'django.contrib.auth.views.logout',
        {
            'next_page': '/',
        },
        name='logout'),

    # Uncomment the admin/doc line below to enable admin documentation:
    # url(r'^admin/doc/', include('django.contrib.admindocs.urls')),

    # Uncomment the next line to enable the admin:
    url(r'^admin/', include(admin.site.urls)),
)
