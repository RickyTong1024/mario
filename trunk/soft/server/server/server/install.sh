#!/bin/bash

set -e

#####################################################
# root privilege
#####################################################
if [[ $EUID -ne 0 ]]; then
    echo "ERROR: Must be run with root."
    exit 1
fi


#####################################################
# install env
#####################################################
set -x

yum groupinstall "Development tools"
yum install zlib-devel bzip2-devel openssl-devel ncurses-devel sqlite-devel mysql-devel

wget https://www.python.org/ftp/python/2.7.11/Python-2.7.11.tar.xz
xz -d Python-2.7.11.tar.xz
tar xvf Python-2.7.11.tar
cd Python-2.7.11
./configure --prefix=/usr/local
make
make altinstall

cd ../
wget https://bootstrap.pypa.io/ez_setup.py
python2.7 ez_setup.py
easy_install-2.7 pip

wget https://pypi.python.org/packages/source/m/mmseg/mmseg-1.3.0.tar.gz
tar xzf mmseg-1.3.0.tar.gz
cd mmseg-1.3.0
python2.7 setup.py install

cd ../
git clone https://github.com/jiedan/redis-search-py.git
cd redis-search-py
python2.7 setup.py install

cd ../


pip2.7 install tornado
pip2.7 install MySQL-python
pip2.7 install pyyaml 
pip2.7 install redis
pip2.7 install hiredis
pip2.7 install pycrypto
pip2.7 install futures
pip2.7 install msgpack-python
pip2.7 install supervisor
pip2.7 install torndb
pip2.7 install rsa
pip2.7 install requests

cat <<CONCLUSION
successful!!!!!!
complete install python
CONCLUSION