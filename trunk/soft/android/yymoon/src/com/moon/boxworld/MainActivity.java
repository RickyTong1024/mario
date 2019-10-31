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
	
	// 商户PID
		public static final String PARTNER = "2088811685407064";
		// 商户收款账号
		public static final String SELLER = "yymoonyymoon@163.com";
		// 商户私钥，pkcs8格式
		public static final String RSA_PRIVATE = "MIICeQIBADANBgkqhkiG9w0BAQEFAASCAmMwggJfAgEAAoGBANUgFdpaMTHD9VxyZWHtj0k//YYZKLA7UecLpmp0nUybxv0p8tYEnCg+N7OOUS1vBlTZ8/c9YEjOQNMcE7CCKoAEzYXsQ2TnthXY1llv405LMFD99C5JVbhfnOrBhjxUy4rzBo2j0ep52+nHihQYONvbkaLftfzYtsYrA8jnrAH7AgMBAAECgYEAj0ujZcx+hxdaQW4o/E7dqEJ+E0uXL+ayisYqfikqGfgjMtShkYRH+kba7L8jlYiwmRxyDCoYMtt6enbGRkc+itqL4ETz0r+zhBK2/oe1ghjpPG8fhP+EeRecLDUrro2DDIt1ckCTs6RVcIglo+syQINY5cVOlQKuBe8Nn2tgeAECQQDtPJQ8lyJUVlL0ExPIEk8PNKarAvq/q+ehUIikPjHrA76PZdD/jsz5WZuw4EA5mFcMGeaVFdiiljLfi895d81hAkEA5ftQxmxa0Q9s92Hws5kqp1+/YFnwPlBvWTmVgtYOK8ULVC+HtwEvZVAsc9yyfmHMcl1IeqiKXMd5SMCU5KFQ2wJBAJHuo9kptupeN6UAXzjBWnSmFKg5qvlgy5mFqqgtwhMpOH2JSikKNLhmEMmbCKblORaukBLBX4OUFTNCE5xKv8ECQQCMgSDVSQem59yzuoNaxj3X6dg63le3Sl7szRB50ZrwxL5qHWu1s6SoKfYuhuwJ6GgGitUoEuYNTtTbTMTqdV+3AkEA6dxfj2Ai7/fY2AUt5c/iuZtatAfUXoDnRZYXBl+B/fkHia4QU1LliZISybpUp2yNeerg8Yh+ebFsXvJyQY+hsQ==";
		// 支付宝公钥
		public static final String RSA_PUBLIC = "MIGfMA0GCSqGSIb3DQEBAQUAA4GNADCBiQKBgQCnxj/9qwVfgoUh/y2W89L6BkRAFljhNhgPdyPuBV64bfQNN1PjbCzkIM6qRdKBoLPXmKKMiFYnkd6rAoprih3/PrQEB/VsW8OoM8fxn67UDYuyBTqA23MML9q1+ilIZwBC2AQ2UBVOrFXfFl75p6/B5KsiNG9zpgmLCUYuLkxpLQIDAQAB";
		private static final int SDK_PAY_FLAG = 1;

		private Handler mHandler = new Handler() {
			public void handleMessage(Message msg) {
				switch (msg.what) {
				case SDK_PAY_FLAG: {
					PayResult payResult = new PayResult((String) msg.obj);
					// 支付宝返回此次支付结果及加签，建议对支付宝签名信息拿签约时支付宝提供的公钥做验签
					String resultStatus = payResult.getResultStatus();
					// 判断resultStatus 为“9000”则代表支付成功，具体状态码代表含义可参考接口文档
					if (TextUtils.equals(resultStatus, "9000")) {
						// 支付成功
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
	 * call alipay sdk pay. 调用SDK支付
	 * 
	 */
	@Override
	public void pay(String uid, int price, int pid, String name, String code, String callbackurl)
	{
		// 订单
		String orderInfo = "partner=" + "\"" + PARTNER + "\"";

		// 签约卖家支付宝账号
		orderInfo += "&seller_id=" + "\"" + SELLER + "\"";

		// 商户网站唯一订单号
		orderInfo += "&out_trade_no=" + "\"" + getOutTradeNo() + "\"";

		// 商品名称
		orderInfo += "&subject=" + "\"" + name + "\"";

		// 商品详情
		orderInfo += "&body=" + "\"" + uid + "\"";

		// 商品金额
		orderInfo += "&total_fee=" + "\"" + price + "\"";

		// 服务器异步通知页面路径
		orderInfo += "&notify_url=" + "\"" + callbackurl + "\"";

		// 服务接口名称， 固定值
		orderInfo += "&service=\"mobile.securitypay.pay\"";

		// 支付类型， 固定值
		orderInfo += "&payment_type=\"1\"";

		// 参数编码， 固定值
		orderInfo += "&_input_charset=\"utf-8\"";

		// 设置未付款交易的超时时间
		// 默认30分钟，一旦超时，该笔交易就会自动被关闭。
		// 取值范围：1m～15d。
		// m-分钟，h-小时，d-天，1c-当天（无论交易何时创建，都在0点关闭）。
		// 该参数数值不接受小数点，如1.5h，可转换为90m。
		orderInfo += "&it_b_pay=\"30m\"";

		// 对订单做RSA 签名
		String sign = sign(orderInfo);
		try {
			// 仅需对sign 做URL编码
			sign = URLEncoder.encode(sign, "UTF-8");
		} catch (UnsupportedEncodingException e) {
			e.printStackTrace();
		}

		// 完整的符合支付宝参数规范的订单信息
		final String payInfo = orderInfo + "&sign=\"" + sign + "\"&"
				+ getSignType();

		Runnable payRunnable = new Runnable() {

			@Override
			public void run() {
				// 构造PayTask 对象
				PayTask alipay = new PayTask(MainActivity.this);
				// 调用支付接口，获取支付结果
				String result = alipay.pay(payInfo);

				Message msg = new Message();
				msg.what = SDK_PAY_FLAG;
				msg.obj = result;
				mHandler.sendMessage(msg);
			}
		};

		// 必须异步调用
		Thread payThread = new Thread(payRunnable);
		payThread.start();
	}
	
	/**
	 * get the sdk version. 获取SDK版本号
	 * 
	 */
	public void getSDKVersion() {
		PayTask payTask = new PayTask(this);
		String version = payTask.getVersion();
		Toast.makeText(this, version, Toast.LENGTH_SHORT).show();
	}

	/**
	 * get the out_trade_no for an order. 生成商户订单号，该值在商户端应保持唯一（可自定义格式规范）
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
	 * sign the order info. 对订单信息进行签名
	 * 
	 * @param content
	 *            待签名订单信息
	 */
	public String sign(String content) {
		return SignUtils.sign(content, RSA_PRIVATE);
	}

	/**
	 * get the sign type we use. 获取签名方式
	 * 
	 */
	public String getSignType() {
		return "sign_type=\"RSA\"";
	}
	
	/**
	 * 用于获取渠道标识，游戏开发者可在任意处调用该方法获取到该字段，含义请参照《如何区分渠道》中的渠道与ChannelLabel对照表
	 * 
	 * @return
	 */
	@Override
	public String getChanelLabel() {
		return "yymoon";
	}
}
