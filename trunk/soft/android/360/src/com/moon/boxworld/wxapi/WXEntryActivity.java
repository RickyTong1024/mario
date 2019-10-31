package com.moon.boxworld.wxapi;

import com.qihoo.gamecenter.sdk.matrix.Matrix;
import com.qihoo.gamecenter.sdk.protocols.ProtocolConfigs;
import com.qihoo.gamecenter.sdk.protocols.ProtocolKeys;

import android.app.Activity;
import android.content.Intent;
import android.os.Bundle;
import android.view.Window;

public class WXEntryActivity extends Activity {

    public void onCreate(Bundle savedInstanceState) {
        requestWindowFeature(Window.FEATURE_NO_TITLE);
        requestWindowFeature(Window.FEATURE_PROGRESS);
        super.onCreate(savedInstanceState);
        Intent intent = getIntent();
        // 妫�鏌ヤ笅intent鏈夋病鏈夊紓甯哥殑鍐呭锛屽彂鐢熷紓甯哥洿鎺ュ叧鎺夌粍浠讹紝涓嶇户缁墽琛�
        // 涓轰簡瑙ｅ喅android鐨勨�滃畨鍗撻�氱敤鍨嬫嫆缁濇湇鍔℃紡娲炩��
        try {
            intent.getStringExtra("try");
        } catch (Throwable tr) {
            finish();
            return;
        }
        intent.putExtra(ProtocolKeys.FUNCTION_CODE, ProtocolConfigs.FUNC_CODE_HANDEL_WEIXIN_CALLBACK);
        Matrix.execute(this, intent, null);
        finish();
    }

}
