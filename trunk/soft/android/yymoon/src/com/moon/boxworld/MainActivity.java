package com.moon.boxworld;

import java.io.UnsupportedEncodingException;
import java.net.URLEncoder;
import java.text.SimpleDateFormat;
import java.util.Date;
import java.util.Locale;
import java.util.Random;

import android.os.Handler;
import android.os.Message;
import android.text.TextUtils;
import android.widget.Toast;

import com.alipay.sdk.app.PayTask;
import com.moon.boxworld.BaseActivity;
import com.unity3d.player.UnityPlayer;

public class MainActivity extends BaseActivity {
	
	// �̻�PID
		public static final String PARTNER = "2088811685407064";
		// �̻��տ��˺�
		public static final String SELLER = "yymoonyymoon@163.com";
		// �̻�˽Կ��pkcs8��ʽ
		public static final String RSA_PRIVATE = "MIICeQIBADANBgkqhkiG9w0BAQEFAASCAmMwggJfAgEAAoGBANUgFdpaMTHD9VxyZWHtj0k//YYZKLA7UecLpmp0nUybxv0p8tYEnCg+N7OOUS1vBlTZ8/c9YEjOQNMcE7CCKoAEzYXsQ2TnthXY1llv405LMFD99C5JVbhfnOrBhjxUy4rzBo2j0ep52+nHihQYONvbkaLftfzYtsYrA8jnrAH7AgMBAAECgYEAj0ujZcx+hxdaQW4o/E7dqEJ+E0uXL+ayisYqfikqGfgjMtShkYRH+kba7L8jlYiwmRxyDCoYMtt6enbGRkc+itqL4ETz0r+zhBK2/oe1ghjpPG8fhP+EeRecLDUrro2DDIt1ckCTs6RVcIglo+syQINY5cVOlQKuBe8Nn2tgeAECQQDtPJQ8lyJUVlL0ExPIEk8PNKarAvq/q+ehUIikPjHrA76PZdD/jsz5WZuw4EA5mFcMGeaVFdiiljLfi895d81hAkEA5ftQxmxa0Q9s92Hws5kqp1+/YFnwPlBvWTmVgtYOK8ULVC+HtwEvZVAsc9yyfmHMcl1IeqiKXMd5SMCU5KFQ2wJBAJHuo9kptupeN6UAXzjBWnSmFKg5qvlgy5mFqqgtwhMpOH2JSikKNLhmEMmbCKblORaukBLBX4OUFTNCE5xKv8ECQQCMgSDVSQem59yzuoNaxj3X6dg63le3Sl7szRB50ZrwxL5qHWu1s6SoKfYuhuwJ6GgGitUoEuYNTtTbTMTqdV+3AkEA6dxfj2Ai7/fY2AUt5c/iuZtatAfUXoDnRZYXBl+B/fkHia4QU1LliZISybpUp2yNeerg8Yh+ebFsXvJyQY+hsQ==";
		// ֧������Կ
		public static final String RSA_PUBLIC = "MIGfMA0GCSqGSIb3DQEBAQUAA4GNADCBiQKBgQCnxj/9qwVfgoUh/y2W89L6BkRAFljhNhgPdyPuBV64bfQNN1PjbCzkIM6qRdKBoLPXmKKMiFYnkd6rAoprih3/PrQEB/VsW8OoM8fxn67UDYuyBTqA23MML9q1+ilIZwBC2AQ2UBVOrFXfFl75p6/B5KsiNG9zpgmLCUYuLkxpLQIDAQAB";
		private static final int SDK_PAY_FLAG = 1;

		private Handler mHandler = new Handler() {
			public void handleMessage(Message msg) {
				switch (msg.what) {
				case SDK_PAY_FLAG: {
					PayResult payResult = new PayResult((String) msg.obj);
					// ֧�������ش˴�֧���������ǩ�������֧����ǩ����Ϣ��ǩԼʱ֧�����ṩ�Ĺ�Կ����ǩ
					String resultStatus = payResult.getResultStatus();
					// �ж�resultStatus Ϊ��9000�������֧���ɹ�������״̬�������ɲο��ӿ��ĵ�
					if (TextUtils.equals(resultStatus, "9000")) {
						// ֧���ɹ�
						UnityPlayer.UnitySendMessage("LJSDK", "pay_callback_success", "");
	                }
	                else
	                {
	                	UnityPlayer.UnitySendMessage("LJSDK", "pay_callback_failed", "");
					}
					break;
				}
				default:
					break;
				}
			};
		};
	/**
	 * call alipay sdk pay. ����SDK֧��
	 * 
	 */
	@Override
	public void pay(String uid, int price, int pid, String name, String code, String callbackurl)
	{
		// ����
		String orderInfo = "partner=" + "\"" + PARTNER + "\"";

		// ǩԼ����֧�����˺�
		orderInfo += "&seller_id=" + "\"" + SELLER + "\"";

		// �̻���վΨһ������
		orderInfo += "&out_trade_no=" + "\"" + getOutTradeNo() + "\"";

		// ��Ʒ����
		orderInfo += "&subject=" + "\"" + name + "\"";

		// ��Ʒ����
		orderInfo += "&body=" + "\"" + uid + "\"";

		// ��Ʒ���
		orderInfo += "&total_fee=" + "\"" + price + "\"";

		// �������첽֪ͨҳ��·��
		orderInfo += "&notify_url=" + "\"" + callbackurl + "\"";

		// ����ӿ����ƣ� �̶�ֵ
		orderInfo += "&service=\"mobile.securitypay.pay\"";

		// ֧�����ͣ� �̶�ֵ
		orderInfo += "&payment_type=\"1\"";

		// �������룬 �̶�ֵ
		orderInfo += "&_input_charset=\"utf-8\"";

		// ����δ����׵ĳ�ʱʱ��
		// Ĭ��30���ӣ�һ����ʱ���ñʽ��׾ͻ��Զ����رա�
		// ȡֵ��Χ��1m��15d��
		// m-���ӣ�h-Сʱ��d-�죬1c-���죨���۽��׺�ʱ����������0��رգ���
		// �ò�����ֵ������С���㣬��1.5h����ת��Ϊ90m��
		orderInfo += "&it_b_pay=\"30m\"";

		// �Զ�����RSA ǩ��
		String sign = sign(orderInfo);
		try {
			// �����sign ��URL����
			sign = URLEncoder.encode(sign, "UTF-8");
		} catch (UnsupportedEncodingException e) {
			e.printStackTrace();
		}

		// �����ķ���֧���������淶�Ķ�����Ϣ
		final String payInfo = orderInfo + "&sign=\"" + sign + "\"&"
				+ getSignType();

		Runnable payRunnable = new Runnable() {

			@Override
			public void run() {
				// ����PayTask ����
				PayTask alipay = new PayTask(MainActivity.this);
				// ����֧���ӿڣ���ȡ֧�����
				String result = alipay.pay(payInfo);

				Message msg = new Message();
				msg.what = SDK_PAY_FLAG;
				msg.obj = result;
				mHandler.sendMessage(msg);
			}
		};

		// �����첽����
		Thread payThread = new Thread(payRunnable);
		payThread.start();
	}
	
	/**
	 * get the sdk version. ��ȡSDK�汾��
	 * 
	 */
	public void getSDKVersion() {
		PayTask payTask = new PayTask(this);
		String version = payTask.getVersion();
		Toast.makeText(this, version, Toast.LENGTH_SHORT).show();
	}

	/**
	 * get the out_trade_no for an order. �����̻������ţ���ֵ���̻���Ӧ����Ψһ�����Զ����ʽ�淶��
	 * 
	 */
	public String getOutTradeNo() {
		SimpleDateFormat format = new SimpleDateFormat("MMddHHmmss",
				Locale.getDefault());
		Date date = new Date();
		String key = format.format(date);

		Random r = new Random();
		key = key + r.nextInt();
		key = key.substring(0, 15);
		return key;
	}

	/**
	 * sign the order info. �Զ�����Ϣ����ǩ��
	 * 
	 * @param content
	 *            ��ǩ��������Ϣ
	 */
	public String sign(String content) {
		return SignUtils.sign(content, RSA_PRIVATE);
	}

	/**
	 * get the sign type we use. ��ȡǩ����ʽ
	 * 
	 */
	public String getSignType() {
		return "sign_type=\"RSA\"";
	}
	
	/**
	 * ���ڻ�ȡ������ʶ����Ϸ�����߿������⴦���ø÷�����ȡ�����ֶΣ���������ա���������������е�������ChannelLabel���ձ�
	 * 
	 * @return
	 */
	@Override
	public String getChanelLabel() {
		return "yymoon";
	}
}
