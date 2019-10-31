# -*- coding: utf-8 -*-

import hashlib
import rsa

def make_sign(args, appkey):
    sign = "#".join(args[k] for k in sorted(args.keys()))
    sign += "#" + appkey
    m = hashlib.md5()
    m.update(sign)
    return m.hexdigest()

def make_alipy_sign(args, appkey):
    sign = "&".join(k + "=" + args[k] for k in sorted(args.keys()))
    sign += appkey
    m = hashlib.md5()
    m.update(sign)
    return m.hexdigest()


def alipy_rsa_verity(args, pubkey, prikey):
    message = "&".join(k + "=" + args[k] for k in sorted(args.keys()))
    try:
        sign = rsa.sign(message, prikey, 'SHA-1')
        rsa.verify(message, sign, pubkey)
    except:
        return False
    return True
