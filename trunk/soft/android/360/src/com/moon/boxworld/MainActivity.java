package com.moon.boxworld;

import java.util.HashMap;
import java.util.Map;
import android.content.Intent;
import android.os.Bundle;
import com.moon.boxworld.BaseActivity;
import com.qihoo.gamecenter.sdk.activity.ContainerActivity;
import com.qihoo.gamecenter.sdk.common.IDispatcherCallback;
import com.qihoo.gamecenter.sdk.matrix.Matrix;
import com.qihoo.gamecenter.sdk.protocols.ProtocolConfigs;
import com.qihoo.gamecenter.sdk.protocols.ProtocolKeys;

import org.json.JSONObject;

import com.unity3d.player.UnityPlayer;

public class MainActivity extends BaseActivity {
	
	@Override
	protected void onDestroy() {
		super.onDestroy();
		Matrix.destroy(this); 
	}

	@Override
	public void init() {
		Matrix.init(this);
		UnityPlayer.UnitySendMessage("LJSDK", "init_callback_success", "");
	}

	@Override
	public void login() {
		Intent intent = new Intent(this, ContainerActivity.class);
		intent.putExtra(ProtocolKeys.IS_SCREEN_ORIENTATION_LANDSCAPE, true);
        intent.putExtra(ProtocolKeys.FUNCTION_CODE, ProtocolConfigs.FUNC_CODE_LOGIN);
		IDispatcherCallback callback = mLoginCallback;
		Matrix.execute(this, intent, callback);
	}
	
	private IDispatcherCallback mLoginCallback = new IDispatcherCallback() {

        @Override
        public void onFinished(String data) {
        	try
        	{
        		JSONObject jo = new JSONObject(data);
        		int errno = jo.getInt("errno");
        		if (errno == 0)
        		{
        			JSONObject joData = jo.getJSONObject("data");
        			String token = joData.getString("access_token");
        			showToast("��¼�ɹ�");
        			Map<String, String> datas = new HashMap<String, String>();
        			datas.put("username", "");
        			datas.put("uid", "");
        			datas.put("token", token);
        			datas.put("channelCode", "360");
        			datas.put("channelUserId", "");
        			datas.put("channellabel", "360");
        			JSONObject obj = new JSONObject(datas);
        			UnityPlayer.UnitySendMessage("LJSDK", "login_callback_success", obj.toString());
            		return;
        		}
        	}
        	catch (Exception e) {}
    		UnityPlayer.UnitySendMessage("LJSDK", "login_callback_failed", "");
        }
    };
   
    @Override
	public void pay(String uid, int price, int pid, String name, String code, String callbackurl) { 
		Bundle bundle = new Bundle();
        bundle.putBoolean(ProtocolKeys.IS_SCREEN_ORIENTATION_LANDSCAPE, true);
        // ���������360�˺�id��������
        bundle.putString(ProtocolKeys.QIHOO_USER_ID, uid);
        // �����������������Ʒ���, �Է�Ϊ��λ�������ڵ���100�֣�360SDK���ж���֧�����̣� �����Ϊ0��360SDK���в�����֧�����̡�
        bundle.putString(ProtocolKeys.AMOUNT, String.valueOf(price * 100));
        // �����������������Ʒ���ƣ�Ӧ��ָ�����������ģ����10�������֡�
        bundle.putString(ProtocolKeys.PRODUCT_NAME, name);
        // ���������������Ʒ����Ʒid��Ӧ��ָ�������16�ַ���
        bundle.putString(ProtocolKeys.PRODUCT_ID, String.valueOf(pid));
        // ���������Ӧ�÷��ṩ��֧�����֪ͨuri�����255�ַ���360����������֧���ӿڻص�����uri������Э����鿴�ĵ��У�֧�����֪ͨ�ӿڨCӦ�÷������ṩ�ӿڡ�
        bundle.putString(ProtocolKeys.NOTIFY_URI, callbackurl);
        // �����������Ϸ��Ӧ�����ƣ����16�����֡�
        bundle.putString(ProtocolKeys.APP_NAME, "boxmaker");
        // ���������Ӧ���ڵ��û���������Ϸ��ɫ���� ��Ӧ���ڰ�360�˺ź�Ӧ���˺ţ������360�û��������16�����֡�����ֵ����������
        // �䵽ͳһ���û��˻�����������ɫ����ʹ�ã���
        bundle.putString(ProtocolKeys.APP_USER_NAME, "1");
        // ���������Ӧ���ڵ��û�id��
        // ��Ӧ���ڰ�360�˺ź�Ӧ���˺ţ���ֵ�����������䵽ͳһ���û��˻�����������ɫ����ʹ�ã������360�û�ID���32�ַ���
        bundle.putString(ProtocolKeys.APP_USER_ID, "1");
        bundle.putInt(ProtocolKeys.FUNCTION_CODE, ProtocolConfigs.FUNC_CODE_PAY);

        Intent intent = new Intent(this, ContainerActivity.class);
        intent.putExtras(bundle);
        intent.putExtra(ProtocolKeys.FUNCTION_CODE, ProtocolConfigs.FUNC_CODE_PAY);
        Matrix.invokeActivity(this, intent, mPayCallback);
	}
	
	protected IDispatcherCallback mPayCallback = new IDispatcherCallback() {

        @Override
        public void onFinished(String data) {
            JSONObject jsonRes;
            try {
                jsonRes = new JSONObject(data);
                int errorCode = jsonRes.optInt("error_code");
                if (errorCode == 0)
                {
                	UnityPlayer.UnitySendMessage("LJSDK", "pay_callback_success", "");
                }
                else
                {
                	UnityPlayer.UnitySendMessage("LJSDK", "pay_callback_failed", "");
                }
            } catch (Exception e) {
            }
        }
    };

    @Override
	public void logout() {
		Intent intent = new Intent(this, ContainerActivity.class);
		intent.putExtra(ProtocolKeys.IS_SCREEN_ORIENTATION_LANDSCAPE, true);
        intent.putExtra(ProtocolKeys.FUNCTION_CODE, ProtocolConfigs.FUNC_CODE_SWITCH_ACCOUNT);
		IDispatcherCallback callback = mLogoutCallback;
		Matrix.invokeActivity(this, intent, callback);
	}
	
	private IDispatcherCallback mLogoutCallback = new IDispatcherCallback() {

        @Override
        public void onFinished(String data) {
        	try
        	{
        		JSONObject jo = new JSONObject(data);
        		int errno = jo.getInt("errno");
        		if (errno == 0)
        		{
            		UnityPlayer.UnitySendMessage("LJSDK", "logout_callback", "");
        			JSONObject joData = jo.getJSONObject("data");
        			String token = joData.getString("access_token");
        			showToast("��¼�ɹ�");
        			Map<String, String> datas = new HashMap<String, String>();
        			datas.put("username", "");
        			datas.put("uid", "");
        			datas.put("token", token);
        			datas.put("channelCode", "360");
        			datas.put("channelUserId", "");
        			datas.put("channellabel", "360");
        			JSONObject obj = new JSONObject(datas);
        			UnityPlayer.UnitySendMessage("LJSDK", "login_callback_success", obj.toString());
        		}
        		return;
        	}
        	catch (Exception e) {}
        	showToast("��¼ʧ��");
    		UnityPlayer.UnitySendMessage("LJSDK", "login_callback_failed", "");
        }
    };

    @Override
	public void exit(int lang) {
		Bundle bundle = new Bundle(); 
		bundle.putBoolean(ProtocolKeys.IS_SCREEN_ORIENTATION_LANDSCAPE, true);
        bundle.putInt(ProtocolKeys.FUNCTION_CODE, ProtocolConfigs.FUNC_CODE_QUIT);
        Intent intent = new Intent(this, ContainerActivity.class);
        intent.putExtras(bundle);
        Matrix.invokeActivity(this, intent, mQuitCallback);
	}
	
	private IDispatcherCallback mQuitCallback = new IDispatcherCallback() {
	    @Override
		public void onFinished(String data) {
	    	try
        	{
        		JSONObject jo = new JSONObject(data);
        		int which = jo.getInt("which");
        		if (which == 2)
        		{
        			kill();
        		}
        	}
	    	catch (Exception e) {}  	
		}
	};

	@Override
	public String getChanelLabel() {
		return "360";
	}
}
