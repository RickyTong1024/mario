DROP TABLE IF EXISTS `pay_t`;
CREATE TABLE  `pay_t` (
  `id` int(11) auto_increment NOT NULL,
  `openid` varchar(128) DEFAULT NULL,
  `userid` bigint(20) NOT NULL,
  `itemid` int(11) NOT NULL,
  `orderid` varchar(128) DEFAULT NULL,
  `amount` int(11) NOT NULL,
  `res` int(11) NOT NULL,
  `pt` varchar(64) DEFAULT NULL,
  `dt` timestamp NOT NULL DEFAULT CURRENT_TIMESTAMP,
  PRIMARY KEY (`id`),
  KEY `userid` (`userid`),
  KEY `openid` (`openid`(32)),
  KEY `orderid` (`orderid`(32)),
  KEY `pt`	(`pt`(16))
) ENGINE=InnoDB DEFAULT CHARSET=utf8; 



