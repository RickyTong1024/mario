# -*- coding: utf-8 -*-

import paramiko
import argparse

def upload(host, user, passwd, site, path1, path2):
    ssh = paramiko.Transport((host, 22))
    ssh.connect(username = user, password = passwd)
    sftp = paramiko.SFTPClient.from_transport(ssh)
    remote_dir = rootdir + str(site) + path2
    print remote_dir
    try:
        sftp.chdir(remote_dir)
    except Exception,e:
        print remote_dir, "not exsit"
        return
    for root, dirs, files in os.walk(path1):
        print root, len(dirs), "dirs", len(files), "files"
        for filespath in files:
            local_file = os.path.join(root,filespath)
            a = local_file.replace(path1, '')
            remote_file = os.path.join(remote_dir, a)
            remote_file = remote_file.replace('\\', '/')
            try:
                sftp.put(local_file, remote_file)
            except Exception,e:
                sftp.mkdir(os.path.split(remote_file)[0])
                sftp.put(local_file,remote_file)
        for name in dirs:
            local_path = os.path.join(root, name)
            a = local_path.replace(path1, '')
            remote_path = os.path.join(remote_dir, a)
            remote_path = remote_path.replace('\\', '/')
            try:
                sftp.mkdir(remote_path)
            except Exception,e:
                print e
    ssh.close()
    print "upload end", host, site


if __name__ == "__main__":
    parse = argparse.ArgumentParser()
    parse.add_argument('-s', '--server', help = "server host")
    parse.add_argument('-u', '--user', help = "server name")
    parse.add_argument('-p', '--pwd', help = "server passward")
    parse.add_argument('-n', '--num', help = "server num")
    parse.add_argument('-r', '--source', help = "source path")
    parse.add_argument('-d', '--destination', help = "dest path")


