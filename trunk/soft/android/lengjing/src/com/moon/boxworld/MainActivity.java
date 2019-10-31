package com.moon.boxworld;

import java.util.HashMap;
import java.util.Map;
import android.content.Intent;
import android.os.Bundle;
import com.moon.boxworld.BaseActivity;

import org.json.JSONObject;

import com.unity3d.player.UnityPlayer;
import com.xinmei365.game.proxy.GameProxy;
import com.xinmei365.game.proxy.PayCallBack;
import com.xinmei365.game.proxy.XMUser;
import com.xinmei365.game.proxy.XMUserListener;
import com.xinmei365.game.proxy.XMUtils;
import com.xinmei365.game.proxy.exit.LJExitCallback;
import com.xinmei365.game.proxy.init.XMInitCallback;
import com.xinmei365.game.proxy.pay.XMPayParams;

public class MainActivity extends BaseActivity implements XMUserListener {
	
	@Override
	protected void onCreate(Bundle savedInstanceState) {
		super.onCreate(savedInstanceState);
		GameProxy.getInstance().onCreate(this);
		GameProxy.getInstance().setUserListener(this, this);
	}

	@Override
	public void onStop() {
		super.onStop();
		GameProxy.getInstance().onStop(this);
	}

	@Override
	public void onDestroy() {
		super.onDestroy();
		GameProxy.getInstance().onDestroy(this);
	}

	@Override
	public void onResume() {
		super.onResume();
		GameProxy.getInstance().onResume(this);
	}

	@Override
	public void onPause() {
		super.onPause();
		GameProxy.getInstance().onPause(this);
	}

	@Override
	public void onRestart() {
		super.onRestart();
		GameProxy.getInstance().onRestart(this);
	}
	
	@Override
	protected void onActivityResult(int requestCode, int resultCode, Intent data) {
		super.onActivityResult(requestCode, resultCode, data);
		GameProxy.getInstance().onActivityResult(this, requestCode, resultCode,
				data);
	}

	@Override
	public void onNewIntent(Intent intent){
		super.onNewIntent(intent);
		GameProxy.getInstance().onNewIntent(intent);
	}
	
	/**
	 * 初始化
	 */
	@Override
	public void init() {
		GameProxy.getInstance().init(this, new XMInitCallback() {

			@Override
			public void onInitSuccess() {
				UnityPlayer.UnitySendMessage("LJSDK", "init_callback_success", "");
			}

			@Override
			public void onInitFailure(String msg) {
				UnityPlayer.UnitySendMessage("LJSDK", "init_callback_failed", msg);
			}
		});
	}

	/**
	 * 登陆接口说明：
	 * 
	 * @param activity
	 *            当前activity
	 * @param customObject
	 *            用户自定义参数，在登陆回调中原样返回
	 */
	@Override
	public void login() {
		GameProxy.getInstance().login(MainActivity.this, "boxmaker");
	}
	
	@Override
	public void onLoginSuccess(final XMUser user, Object customParams) {
		showToast("登录成功");
		
		Map<String, String> datas = new HashMap<String, String>();
		datas.put("username", user.getUsername());
		datas.put("uid", user.getUserID());
		datas.put("token", user.getToken());
		datas.put("channelCode", user.getChannelID());
		datas.put("channelUserId", user.getChannelUserId());
		datas.put("channellabel", user.getChannelLabel());
		JSONObject obj = new JSONObject(datas);
		UnityPlayer.UnitySendMessage("LJSDK", "login_callback_success", obj.toString());
	}

	@Override
	public void onLoginFailed(String reason, Object customParams) {
		showToast("登录失败");
		UnityPlayer.UnitySendMessage("LJSDK", "login_callback_failed", "");
	}

	/**
	 * 定额支付接口说明
	 * 
	 * @param context
	 *            上下文Activity
	 * @param total
	 *            定额支付总金额，单位为人民币分
	 * @param unitName
	 *            游戏币名称，如金币、钻石等
	 * @param count
	 *            购买商品数量，如100钻石，传入100；10魔法石，传入10
	 * @param callBackInfo
	 *            游戏开发者自定义字段，会与支付结果一起通知到游戏服务器，游戏服务器可通过该字段判断交易的详细内容（金额 角色等）
	 * @param callBackUrl
	 *            支付结果通知地址，支付完成后我方后台会向该地址发送支付通知
	 * @param payCallBack
	 *            支付回调接口
	 */
	@Override
	public void pay(String uid, int price, int pid, String name, String code, String callbackurl) { 
		XMPayParams params = new XMPayParams();
		params.setAmount(price);
		params.setItemName("钻石");
		params.setCount(1);
		params.setChargePointName(name);
		params.setCustomParam(String.valueOf(pid));
		params.setCallbackUrl(callbackurl);
		GameProxy.getInstance().pay(MainActivity.this, params, new PayCallBack() {

			@Override
			public void onSuccess(String sucessInfo) {
				// 此处回调仅代表用户已发起支付操作，不代表是否充值成功，具体充值是否到账需以服务器间通知为准；
				// 在此回调中游戏方可开始向游戏服务器发起请求，查看订单是否已支付成功，若支付成功则发送道具。
				UnityPlayer.UnitySendMessage("LJSDK", "pay_callback_failed", sucessInfo);
			}
			
			@Override
			public void onFail(String failInfo) {
				// 此处回调代表用户已放弃支付，无需向服务器查询充值状态
				UnityPlayer.UnitySendMessage("LJSDK", "pay_callback_failed", failInfo);
			}
		});
	}

	/**
	 * 登出接口说明：
	 * 
	 * @param activity
	 *            当前activity
	 * @param customObject
	 *            用户自定义参数，在登陆回调中原样返回
	 */
	@Override
	public void logout() {
		GameProxy.getInstance().logout(MainActivity.this, "boxmaker");
	}
	
	@Override
	public void onLogout(Object customParams) {
		// customObject为logout方法中传入的参数，原样返回
		UnityPlayer.UnitySendMessage("LJSDK", "logout_callback", "");
	}

	/**
	 * 退出接口说明：
	 * 
	 * @param context
	 *            当前activity
	 * @param callback
	 *            退出回调
	 */
	@Override
	public void exit(int lang) {
		GameProxy.getInstance().exit(MainActivity.this, new LJExitCallback() {

			@Override
			public void onGameExit() {
				// 渠道不存在退出界面，如百度移动游戏等，此时需在此处弹出游戏退出确认界面，否则会出现渠道审核不通过情况
				// 游戏定义自己的退出界面 ，实现退出逻辑
				showExitGameDialog();
			}

			@Override
			public void onChannelExit() {
				// 该方法必须在退出时调用
				kill();
			}
		});
	}
	
	@Override
	public void kill() {
		GameProxy.getInstance().applicationDestroy(MainActivity.this);
		MainActivity.this.finish();
		onDestroy();
		android.os.Process.killProcess(android.os.Process.myPid());
	}

	@Override
	public void doSetExtData(String uid, String name, String level, String balance) {
		Map<String, String> datas = new HashMap<String, String>();
		datas.put("_id", "enterServer");
		datas.put("roleId", uid);
		datas.put("roleName", name);
		datas.put("roleLevel", level);
		datas.put("zoneId", "1");
		datas.put("zoneName", "无区服");
		datas.put("balance", balance);
		datas.put("vip", "0");
		datas.put("partyName", "无帮派");
		JSONObject obj = new JSONObject(datas);
		GameProxy.getInstance().setExtData(this, obj.toString());
	}

	/**
	 * 用于获取渠道标识，游戏开发者可在任意处调用该方法获取到该字段，含义请参照《如何区分渠道》中的渠道与ChannelLabel对照表
	 * 
	 * @return
	 */
	@Override
	public String getChanelLabel() {
		return XMUtils.getChannelLabel(this);
	}
}
