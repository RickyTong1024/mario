# -*- coding: utf-8 -*-

class _const():
    class ConstError(TypeError): pass
    class ConstUpperError(ConstError): pass

    def __setattr__(self, name, value):
        if self.__dict__.has_key(name):
            raise self.ConstError, "can't change const value.%s" % name
        if not name.isupper():
            raise self.ConstUpperError, 'const name."%s" is not uppercase' % name
        self.__dict__[name] = value


import sys
sys.modules[__name__] = _const()