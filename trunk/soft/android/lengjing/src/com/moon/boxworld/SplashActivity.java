package com.moon.boxworld;
import com.xinmei365.game.proxy.XMSplashActivity;

import android.content.Intent;
import android.graphics.Color;

/**
 * 闂睆Activity
 * 灏嗛棯灞廇ctivity缁ф壙com.xinmei365.game.proxy.XMSplashActivity,骞跺皢璇ctivity璁剧疆涓虹▼搴忓惎鍔ㄦ椂鐨凙ctivity锛堝彲鍙傝�傾ndroidManifest.xml鏂囦欢閰嶇疆锛夛紝
 * 骞跺疄鐜癵etBackgroundColor()鍙妎nSplashStop()鏂规硶銆�
 * 闂睆鍔熻兘涓哄繀椤绘帴鍏ュ姛鑳斤紝鎺ュ叆璇ユ帴鍙ｅ悗寮�鍙戣�呭彲浠ュ湪娓告垙鍚庡彴鐏垫椿閰嶇疆闂睆鍐呭銆佹暟閲忋�佹搴忕瓑锛屼笉閰嶇疆闂睆鍥剧墖鍒欎笉鍑虹幇闂睆,璇ユ帴鍙ｄ笉浼氬奖鍝嶇▼搴忓師鏈夐棯灞忋��
 */
public class SplashActivity extends XMSplashActivity {
	
	//閰嶇疆榛樿鐨勯棯灞忚儗鏅壊锛岄�傞厤鎬ч棶棰樺鑷撮棯灞忓浘鐗囦笉鑳藉湪鎵�鏈夋墜鏈轰笂鍋氬埌瀹屽叏瑕嗙洊锛屽綋鏃犳硶瀹屽叏瑕嗙洊浼氬皢姝ら鑹茶缃负灞忓箷鑳屾櫙鑹�
	public int getBackgroundColor() {
		return Color.WHITE;
	}

	@Override
	public void onSplashStop() {
		//闂睆缁撴潫锛屾墽琛岃繘鍏ユ父鎴忔搷浣�
		Intent intent = new Intent(this, MainActivity.class);
		startActivity(intent);
		this.finish();
	}
}
