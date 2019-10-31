#! /usr/bin/env python
#coding=utf-8

import os
from shutil import copy

fdir = "../../soft/client/Assets/Resources/config/"
fdir1 = "../../soft/server/config/"

def conv_UTF8():
    files = os.listdir ('./')

    for f in files:
        if os.path.splitext (f)[1] == '.txt':
            print f
            ifs = open(f,'rb')
            try:
                content = ifs.read ().decode('gbk').encode('utf8')
                ifs.close()
                content = content.replace('"', '')
                
                if len (content) > 0:
                    try:
                        ofs = open(fdir + f, 'wb')
                        ofs.write(content)
                        ofs1 = open(fdir1 + f, 'wb')
                        ofs1.write(content)
                    finally:
                        ofs.close ()
                        ofs1.close ()

            finally:
                ifs.close ()
        elif os.path.splitext (f)[1] == '.xml':
            print f
            copy(f, fdir + f)

if __name__ == '__main__':
    conv_UTF8 ()

