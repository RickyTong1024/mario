package com.moon.boxworld;

import java.util.Locale;
import java.util.Properties;
import com.tencent.stat.StatConfig;
import com.tencent.stat.StatService;
import com.unity3d.player.UnityPlayer;
import com.unity3d.player.UnityPlayerNativeActivity;
import android.annotation.SuppressLint;
import android.app.AlertDialog;
import android.app.AlertDialog.Builder;
import android.content.DialogInterface;
import android.content.DialogInterface.OnKeyListener;
import android.content.pm.ActivityInfo;
import android.os.Build;
import android.os.Bundle;
import android.view.KeyEvent;
import android.view.OrientationEventListener;
import android.view.Surface;
import android.view.View;
import android.view.Window;
import android.view.WindowManager;
import android.widget.Toast;

public class BaseActivity extends UnityPlayerNativeActivity {

	public boolean m_flag = false;
	public String m_guid;
	public String m_sid;
	public String m_level;
	private OrientationEventListener mOrientationListener;
	private boolean m_zx = false;
	private boolean m_tc = false;
	private int m_lang = 0;

	@Override
	protected void onCreate(Bundle savedInstanceState) {
		super.onCreate(savedInstanceState);
		StatConfig.setDebugEnable(false);
		int rotation = this.getWindowManager().getDefaultDisplay()
				.getRotation();
		switch (rotation) {
		case Surface.ROTATION_0:
			m_zx = false;
			this.setRequestedOrientation(ActivityInfo.SCREEN_ORIENTATION_LANDSCAPE);
			break;
		case Surface.ROTATION_90:
			m_zx = false;
			this.setRequestedOrientation(ActivityInfo.SCREEN_ORIENTATION_LANDSCAPE);
			break;
		case Surface.ROTATION_180:
			m_zx = true;
			this.setRequestedOrientation(ActivityInfo.SCREEN_ORIENTATION_REVERSE_LANDSCAPE);
			break;
		case Surface.ROTATION_270:
			m_zx = true;
			this.setRequestedOrientation(ActivityInfo.SCREEN_ORIENTATION_REVERSE_LANDSCAPE);
		default:
			break;
		}
		startOrientationChangeListener();

		Window window = getWindow();
		window.setFlags(WindowManager.LayoutParams.FLAG_KEEP_SCREEN_ON,
				WindowManager.LayoutParams.FLAG_KEEP_SCREEN_ON);
		HideSystemUI();
		AlarmReceiver.m_act = this;
	}

	@SuppressLint("InlinedApi")
	public void HideSystemUI() {
		if (Build.VERSION.SDK_INT >= 16) {
			final View decorView = this.getWindow().getDecorView();

			int uiOptions = 0;

			if (Build.VERSION.SDK_INT >= 19) {
				uiOptions = View.SYSTEM_UI_FLAG_FULLSCREEN
						| View.SYSTEM_UI_FLAG_LAYOUT_STABLE
						| View.SYSTEM_UI_FLAG_LAYOUT_HIDE_NAVIGATION
						| View.SYSTEM_UI_FLAG_LAYOUT_FULLSCREEN
						| View.SYSTEM_UI_FLAG_HIDE_NAVIGATION // hide nav bar
						| View.SYSTEM_UI_FLAG_IMMERSIVE;
			} else {
				uiOptions = View.SYSTEM_UI_FLAG_FULLSCREEN;
			}
			decorView.setSystemUiVisibility(uiOptions);

			int updatedUIOptions = 0;
			// Fix input method showing causes ui show issue.
			if (Build.VERSION.SDK_INT >= 19) {
				updatedUIOptions = View.SYSTEM_UI_FLAG_LAYOUT_STABLE
						| View.SYSTEM_UI_FLAG_LAYOUT_HIDE_NAVIGATION
						| View.SYSTEM_UI_FLAG_LAYOUT_FULLSCREEN
						| View.SYSTEM_UI_FLAG_HIDE_NAVIGATION
						| View.SYSTEM_UI_FLAG_FULLSCREEN
						| View.SYSTEM_UI_FLAG_IMMERSIVE_STICKY;
			} else {
				updatedUIOptions = View.SYSTEM_UI_FLAG_FULLSCREEN;
			}

			final int finalUiOptions = updatedUIOptions;
			decorView
					.setOnSystemUiVisibilityChangeListener(new View.OnSystemUiVisibilityChangeListener() {
						@Override
						public void onSystemUiVisibilityChange(int i) {
							decorView.setSystemUiVisibility(finalUiOptions);
						}
					});
		}
	}

	private final void startOrientationChangeListener() {
		mOrientationListener = new OrientationEventListener(this) {
			@Override
			public void onOrientationChanged(int rotation) {
				if (rotation > 45 && rotation < 135) {
					if (!m_zx) {
						m_zx = true;
						BaseActivity.this
								.setRequestedOrientation(ActivityInfo.SCREEN_ORIENTATION_REVERSE_LANDSCAPE);
					}
				} else if (rotation > 225 && rotation < 315) {
					if (m_zx) {
						m_zx = false;
						BaseActivity.this
								.setRequestedOrientation(ActivityInfo.SCREEN_ORIENTATION_LANDSCAPE);
					}
				}
			}
		};
		mOrientationListener.enable();
	}

	// //////////////////////////////////////////////////////////////

	public void onJewelGet(String guid, String scene, String num) {
		Properties prop = new Properties();
		prop.setProperty("uid", guid);
		prop.setProperty("scene", scene);
		prop.setProperty("num", num);
		StatService.trackCustomKVEvent(this, "onJewelGet", prop);
	}

	public void onJewelConsume(String guid, String scene, String num) {
		Properties prop = new Properties();
		prop.setProperty("uid", guid);
		prop.setProperty("scene", scene);
		prop.setProperty("num", num);
		StatService.trackCustomKVEvent(this, "onJewelConsume", prop);
	}

	public void onUserDo(String guid, String scene, String num) {
		Properties prop = new Properties();
		prop.setProperty("uid", guid);
		prop.setProperty("scene", scene);
		prop.setProperty("num", num);
		StatService.trackCustomKVEvent(this, "onUserDo", prop);
	}

	public void onRaid(String guid, String scene, String num) {
		Properties prop = new Properties();
		prop.setProperty("uid", guid);
		prop.setProperty("scene", scene);
		prop.setProperty("num", num);
		StatService.trackCustomKVEvent(this, "onRaid", prop);
	}

	// //////////////////////////////////////////////////////////////

	public String getCountryCode() {
		return Locale.getDefault().getCountry();
	}

	public void showToast(final String content) {
		runOnUiThread(new Runnable() {

			@Override
			public void run() {
				Toast.makeText(BaseActivity.this, content, Toast.LENGTH_LONG)
						.show();
			}
		});
	}

	public void showExitGameDialog() {
		runOnUiThread(new Runnable() {

			@Override
			public void run() {
				AlertDialog.Builder builder = new Builder(BaseActivity.this);
				if (m_lang == 0) {
					builder.setMessage("确定要退出游戏吗？");
					builder.setTitle("提示");
					builder.setPositiveButton("确定",
							new DialogInterface.OnClickListener() {
								@Override
								public void onClick(DialogInterface dialog,
										int index) {
									// TODO Auto-generated method stub
									kill();
								}
							});
					builder.setNegativeButton("取消",
							new DialogInterface.OnClickListener() {
								@Override
								public void onClick(DialogInterface dialog,
										int index) {
									// TODO Auto-generated method stub
									m_tc = false;
								}
							});
				} else {
					builder.setMessage("Do you want to exit game?");
					builder.setTitle("Tip");
					builder.setPositiveButton("OK",
							new DialogInterface.OnClickListener() {
								@Override
								public void onClick(DialogInterface dialog,
										int index) {
									// TODO Auto-generated method stub
									kill();
								}
							});
					builder.setNegativeButton("Cancel",
							new DialogInterface.OnClickListener() {
								@Override
								public void onClick(DialogInterface dialog,
										int index) {
									// TODO Auto-generated method stub
									m_tc = false;
								}
							});
				}
				builder.setOnKeyListener(new OnKeyListener() {
					@Override
					public boolean onKey(DialogInterface dialog, int keyCode,
							KeyEvent event) {
						if (keyCode == KeyEvent.KEYCODE_BACK
								&& event.getRepeatCount() == 0) {
							m_tc = false;
						}
						return false;
					}
				});
				builder.create().show();
			}
		});
	}

	public void kill() {
		this.finish();
		onDestroy();
		android.os.Process.killProcess(android.os.Process.myPid());
	}

	public void init() {
		UnityPlayer.UnitySendMessage("LJSDK", "init_callback_success", "");
	}

	public void login() {

	}

	public void logout() {

	}

	public void exit(int lang) {
		if (m_tc) {
			return;
		}
		m_tc = true;
		m_lang = lang;
		showExitGameDialog();
	}

	public void req_info(String code) {
		UnityPlayer.UnitySendMessage("LJSDK", "recharge_android_product", "");
	}

	public void pay(String uid, int price, int pid, String name, String code,
			String callbackurl) {

	}

	public void doSetExtData(String uid, String name, String level,
			String balance) {

	}

	public String getChanelLabel() {
		return "";
	}
}
