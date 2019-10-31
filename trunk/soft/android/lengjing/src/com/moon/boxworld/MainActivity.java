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
	 * ��ʼ��
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
	 * ��½�ӿ�˵����
	 * 
	 * @param activity
	 *            ��ǰactivity
	 * @param customObject
	 *            �û��Զ���������ڵ�½�ص���ԭ������
	 */
	@Override
	public void login() {
		GameProxy.getInstance().login(MainActivity.this, "boxmaker");
	}
	
	@Override
	public void onLoginSuccess(final XMUser user, Object customParams) {
		showToast("��¼�ɹ�");
		
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
		showToast("��¼ʧ��");
		UnityPlayer.UnitySendMessage("LJSDK", "login_callback_failed", "");
	}

	/**
	 * ����֧���ӿ�˵��
	 * 
	 * @param context
	 *            ������Activity
	 * @param total
	 *            ����֧���ܽ���λΪ����ҷ�
	 * @param unitName
	 *            ��Ϸ�����ƣ����ҡ���ʯ��
	 * @param count
	 *            ������Ʒ��������100��ʯ������100��10ħ��ʯ������10
	 * @param callBackInfo
	 *            ��Ϸ�������Զ����ֶΣ�����֧�����һ��֪ͨ����Ϸ����������Ϸ��������ͨ�����ֶ��жϽ��׵���ϸ���ݣ���� ��ɫ�ȣ�
	 * @param callBackUrl
	 *            ֧�����֪ͨ��ַ��֧����ɺ��ҷ���̨����õ�ַ����֧��֪ͨ
	 * @param payCallBack
	 *            ֧���ص��ӿ�
	 */
	@Override
	public void pay(String uid, int price, int pid, String name, String code, String callbackurl) { 
		XMPayParams params = new XMPayParams();
		params.setAmount(price);
		params.setItemName("��ʯ");
		params.setCount(1);
		params.setChargePointName(name);
		params.setCustomParam(String.valueOf(pid));
		params.setCallbackUrl(callbackurl);
		GameProxy.getInstance().pay(MainActivity.this, params, new PayCallBack() {

			@Override
			public void onSuccess(String sucessInfo) {
				// �˴��ص��������û��ѷ���֧���������������Ƿ��ֵ�ɹ��������ֵ�Ƿ������Է�������֪ͨΪ׼��
				// �ڴ˻ص�����Ϸ���ɿ�ʼ����Ϸ�������������󣬲鿴�����Ƿ���֧���ɹ�����֧���ɹ����͵��ߡ�
				UnityPlayer.UnitySendMessage("LJSDK", "pay_callback_failed", sucessInfo);
			}
			
			@Override
			public void onFail(String failInfo) {
				// �˴��ص������û��ѷ���֧�����������������ѯ��ֵ״̬
				UnityPlayer.UnitySendMessage("LJSDK", "pay_callback_failed", failInfo);
			}
		});
	}

	/**
	 * �ǳ��ӿ�˵����
	 * 
	 * @param activity
	 *            ��ǰactivity
	 * @param customObject
	 *            �û��Զ���������ڵ�½�ص���ԭ������
	 */
	@Override
	public void logout() {
		GameProxy.getInstance().logout(MainActivity.this, "boxmaker");
	}
	
	@Override
	public void onLogout(Object customParams) {
		// customObjectΪlogout�����д���Ĳ�����ԭ������
		UnityPlayer.UnitySendMessage("LJSDK", "logout_callback", "");
	}

	/**
	 * �˳��ӿ�˵����
	 * 
	 * @param context
	 *            ��ǰactivity
	 * @param callback
	 *            �˳��ص�
	 */
	@Override
	public void exit(int lang) {
		GameProxy.getInstance().exit(MainActivity.this, new LJExitCallback() {

			@Override
			public void onGameExit() {
				// �����������˳����棬��ٶ��ƶ���Ϸ�ȣ���ʱ���ڴ˴�������Ϸ�˳�ȷ�Ͻ��棬��������������˲�ͨ�����
				// ��Ϸ�����Լ����˳����� ��ʵ���˳��߼�
				showExitGameDialog();
			}

			@Override
			public void onChannelExit() {
				// �÷����������˳�ʱ����
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
		datas.put("zoneName", "������");
		datas.put("balance", balance);
		datas.put("vip", "0");
		datas.put("partyName", "�ް���");
		JSONObject obj = new JSONObject(datas);
		GameProxy.getInstance().setExtData(this, obj.toString());
	}

	/**
	 * ���ڻ�ȡ������ʶ����Ϸ�����߿������⴦���ø÷�����ȡ�����ֶΣ���������ա���������������е�������ChannelLabel���ձ�
	 * 
	 * @return
	 */
	@Override
	public String getChanelLabel() {
		return XMUtils.getChannelLabel(this);
	}
}
