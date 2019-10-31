#! /usr/bin/env python
#coding=utf-8

import os
import shutil

folderA = "D:/work/mario/tags/client/1.1/".lower()
folderB = "D:/work/mario/tags/client/1.0/".lower()
folderC = "D:/work/mario/tags/update/1.1/".lower()

filePathsA = {}
filePathsB = {}

def mkdir(path):
    path = path.strip()
    path = path.rstrip("/")
    isExists = os.path.exists(path)
    if not isExists:
        os.makedirs(path)
        return True
    else:
        return False

def main():
    for root,dirs,files in os.walk(folderA):
        for fileName in files:
            filePathsA[(root + "/" + fileName).lower()] = 1

    for root,dirs,files in os.walk(folderB):
        for fileName in files:
            filePathsB[(root + "/" + fileName).lower()] = 1

    addedFilePath = []
    for filePathA in filePathsA:
        folderALen = len(folderA)
        filePathB = folderB + filePathA[folderALen:]

        idx = filePathA.rfind(".")
        if idx == -1:
            continue
        ext = filePathA[idx + 1:]
        ext = ext.lower()

        if filePathB not in filePathsB:
            addedFilePath.append(filePathA)
            continue

        text_file = open(filePathA, "r")
        textA = text_file.read()
        text_file.close()

        text_file = open(filePathB, "r")
        textB = text_file.read()
        text_file.close()

        if textA != textB:     
            addedFilePath.append(filePathA)

    print addedFilePath
    
    for filePath in addedFilePath:
        s = filePath.replace(folderA, "")
        s = folderC + s
        idx = s.rfind("/")
        if idx != -1:
            ss = s[0:idx]
            mkdir(ss)
        shutil.copyfile(filePath, s)

if __name__ == "__main__":
    main()
