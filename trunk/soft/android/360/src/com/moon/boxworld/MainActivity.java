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
        			showToast("登录成功");
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
        // 必需参数，360账号id，整数。
        bundle.putString(ProtocolKeys.QIHOO_USER_ID, uid);
        // 必需参数，所购买商品金额, 以分为单位。金额大于等于100分，360SDK运行定额支付流程； 金额数为0，360SDK运行不定额支付流程。
        bundle.putString(ProtocolKeys.AMOUNT, String.valueOf(price * 100));
        // 必需参数，所购买商品名称，应用指定，建议中文，最大10个中文字。
        bundle.putString(ProtocolKeys.PRODUCT_NAME, name);
        // 必需参数，购买商品的商品id，应用指定，最大16字符。
        bundle.putString(ProtocolKeys.PRODUCT_ID, String.valueOf(pid));
        // 必需参数，应用方提供的支付结果通知uri，最大255字符。360服务器将把支付接口回调给该uri，具体协议请查看文档中，支付结果通知接口–应用服务器提供接口。
        bundle.putString(ProtocolKeys.NOTIFY_URI, callbackurl);
        // 必需参数，游戏或应用名称，最大16中文字。
        bundle.putString(ProtocolKeys.APP_NAME, "boxmaker");
        // 必需参数，应用内的用户名，如游戏角色名。 若应用内绑定360账号和应用账号，则可用360用户名，最大16中文字。（充值不分区服，
        // 充到统一的用户账户，各区服角色均可使用）。
        bundle.putString(ProtocolKeys.APP_USER_NAME, "1");
        // 必需参数，应用内的用户id。
        // 若应用内绑定360账号和应用账号，充值不分区服，充到统一的用户账户，各区服角色均可使用，则可用360用户ID最大32字符。
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
        			showToast("登录成功");
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
        	showToast("登录失败");
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
