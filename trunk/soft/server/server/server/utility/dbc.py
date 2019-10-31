# -*- coding: utf-8 -*-

'''dbc文件解析'''

import struct

class dbc(object):
    def __init__(self):
        self.x = 0
        self.y = 0
        self.data = []
        self.records = []
        self.name = ""
    
    def load(self, path, name):
        self.name = name
        with open(path + name , "r") as f:
            i = 0
            for byte in f.read():
                self.data.append(byte)
                if byte == '\n':
                    self.y = self.y + 1
                if (byte == '\n' or byte == '\t') and self.y > 1:
                    self.records.append(i + 1)
                i = i + 1
        
            i = 0
            f.seek(0, 0)
            for byte in f.read():
                if byte == '\t':
                    self.x = self.x + 1
                elif byte == '\n':
                    break

        self.x = self.x + 1
        self.y = self.y - 2

    def get(self, x, y):
        index = y * self.x + x
        if index >= len(self.records):
            return None

        begin = self.records[index]
        end = begin
        while True:
            if self.data[end] != '\t' and self.data[end] != '\n' and self.data[end] != '\r':
                end = end + 1
            else:
                break

        if begin == end:
            return '0'

        out, = struct.unpack("%ds" % (end - begin), ('').join(self.data[begin:end]))
        out = out.replace("{nn}", "\n")
        return out

    def get_x(self):
        return self.x

    def get_y(self):
        return self.y
