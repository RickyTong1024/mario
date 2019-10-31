# -*- coding: utf-8 -*-

'''使用线程异步执行任务'''

from concurrent.futures import ThreadPoolExecutor
from tornado.ioloop import IOLoop
from tornado.concurrent import run_on_executor

class AsyncExecutor:
    def __init__(self, num_workers):
        self.io_loop    = IOLoop.current()
        self.executor   = ThreadPoolExecutor(num_workers)

    @run_on_executor
    def cmd(self, func, *args, **kwargs):
        """
        在线程中执行自定义函数func
        注意：func必须是线程安全的
        """

        res = func(*args, **kwargs)
        return res