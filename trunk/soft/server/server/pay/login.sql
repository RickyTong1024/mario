DROP TABLE IF EXISTS `login_t`;
CREATE TABLE  `login_t` (
  `id` int(11) auto_increment NOT NULL,
  `openid` varchar(128) DEFAULT NULL,
  `userid` bigint(20) NOT NULL,
  `pt` varchar(64) DEFAULT NULL,
  `login_time` bigint(20) NOT NULL,
  `op` int(11) NOT NULL,
  PRIMARY KEY (`id`),
  KEY `userid` (`userid`),
  KEY `login_time` (`login_time`),
  KEY `pt`	(`pt`(16))
) ENGINE=InnoDB DEFAULT CHARSET=utf8; 



