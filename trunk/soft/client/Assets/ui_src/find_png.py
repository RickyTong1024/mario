#! /usr/bin/env python
#coding=utf-8

import xml.dom.minidom
import sys
import os

root = 'D:/work/mario/trunk/soft/client/Assets/resources/ui/'
files = []

def do_file():
    global files
    fs = os.listdir (root)
    for f in fs:
        if os.path.splitext (f)[1] == '.prefab':
            ifs = open(root + f,'rb')
            content = ifs.read ().encode('hex')
            files.append(content)
            ifs.close()

def do_tps(s):
    ss = s + '.tps'
    dom = xml.dom.minidom.parse(ss)
    cc = dom.getElementsByTagName('filename')
    ccs = []
    for i in range(len(cc)):
        a = cc[i].childNodes[0].nodeValue
        index = a.rfind('/')
        if index != -1:
            a = a[index + 1:]
            index = a.rfind('.png')
            if index != -1:
                b = False
                a = a[0:index]
                bb = False
                for j in range(len(ccs)):
                    if ccs[j] == a:
                        bb = True
                        break
                if bb == False:
                    ccs.append(a)
                else:
                    continue
                aa = a.encode('hex')
                for f in files:
                    index1 = f.find(aa)
                    if index1 != -1:
                        b = True
                        break
                if b == False:
                    print a

if __name__ == '__main__':
    do_file()
    for i in range(1, len(sys.argv)):
        s = sys.argv[i]
        print s
        if len(s) < 4:
            continue
        if s[len(s) - 4:] != '.tps':
            continue
        s = s[:len(s) - 4]
        do_tps(s)
        
    os.system("pause")
