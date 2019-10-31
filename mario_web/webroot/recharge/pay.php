<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html>
<head>
	<meta http-equiv="Content-Type" content="text/html; charset=utf-8">
	<title>BoxMaker支付接口</title>
</head>
<?php
	//必填，不能修改
	//服务器异步通知页面路径
	$notify_url = $_GET['WINotify_url'];
	//商户订单号
	$out_trade_no = $_GET['WIDout_trade_no'];
	//订单名称
	$subject = $_GET['WIDsubject'];
	//付款金额
	$total_fee = $_GET['WIDtotal_fee'];
	//公共用户信息
    $extra_common_param = $_GET['WIextra_common_param'];
    
	echo $notify_url;
	echo $out_trade_no;
	echo $subject;
	echo $total_fee;
	echo $extra_common_param;
	
	echo "
	<form style='display:none;' id='form1' name='form1' method='post' action='alipayapi.php'>
		<input name='WINotify_url' type='text' value='{$notify_url}' />
		<input name='WIDout_trade_no' type='text' value='{$out_trade_no}'/>
		<input name='WIDsubject' type='text' value='{$subject}'/>
		<input name='WIDtotal_fee' type='text' value='{$total_fee}'/>
		<input name='WIextra_common_param' type='text' value='{$extra_common_param}'/>
	</form>
	<script type='text/javascript'>function load_submit(){document.form1.submit()}load_submit();</script>";

?>
</body>
</html>