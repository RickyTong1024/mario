package com.moon.boxworld;

import java.util.ArrayList;
import java.util.List;

import android.content.Intent;
import android.os.Bundle;
import android.view.Gravity;
import android.view.View;
import android.view.ViewGroup.LayoutParams;
import android.widget.LinearLayout;

import com.example.android.trivialdrivesample.util.IabHelper;
import com.example.android.trivialdrivesample.util.IabResult;
import com.example.android.trivialdrivesample.util.Inventory;
import com.example.android.trivialdrivesample.util.Purchase;
import com.example.android.trivialdrivesample.util.SkuDetails;
import com.google.android.gms.ads.AdListener;
import com.google.android.gms.ads.AdRequest;
import com.google.android.gms.ads.AdSize;
import com.google.android.gms.ads.AdView;
import com.google.android.gms.ads.InterstitialAd;
import com.moon.boxworld.BaseActivity;
import com.unity3d.player.UnityPlayer;

public class MainActivity extends BaseActivity {
	
	InterstitialAd m_interstitial;
	AdView m_adView;
	IabHelper m_Helper;
	Inventory m_inventory;
	static final String base64EncodedPublicKey = "MIIBIjANBgkqhkiG9w0BAQEFAAOCAQ8AMIIBCgKCAQEAmtbKb0OHWaPhTeD0GgxMF5swDV8F0AoesRz4QUVn7MKRIjWn30o8fbmGAXFA1HZk6Jwgw0dx5T5JKCWBzGaoqwPxy45Ai/vicfQeF8D5bqxtpFU2HBw1Lo2wCfGb0OKGg6t6sOIfVJ5Y7wiihGXBZnxo+T8QgycD2vk0QHZkW15QeVZt3vmzaBwqf7wu4YsZR9E53uzu5bJ9Ovyj7z+3bRWarzU8vgUkDPd5hjAoSbt22xAPYDF7/dBhSkbJNd3PtHYE5AEJ70kDeil63h0Sl170NQqF7kMgAxeYDEMrmSt7ixiBjtN/BGJGJz0rDYmeRHYVEmTsNDbaTBZMeEdtOwIDAQAB";
    static final int RC_REQUEST = 10001;
    private boolean m_suc = false;
	
	@Override
	protected void onCreate(Bundle savedInstanceState) {
		super.onCreate(savedInstanceState);
		m_interstitial = new InterstitialAd(MainActivity.this);
		m_interstitial.setAdUnitId("ca-app-pub-5826033784775337/7226031801");
		
		LinearLayout layout = new LinearLayout(this);
		layout.setOrientation(LinearLayout.HORIZONTAL);
		layout.setGravity(Gravity.BOTTOM);
		addContentView(layout, new LayoutParams(LayoutParams.MATCH_PARENT, LayoutParams.MATCH_PARENT));
		m_adView = new AdView(this);
		m_adView.setAdUnitId("ca-app-pub-5826033784775337/5749298601");
		m_adView.setAdSize(AdSize.BANNER);
		layout.addView(m_adView, new LayoutParams(LayoutParams.MATCH_PARENT, LayoutParams.WRAP_CONTENT));
		AdRequest adRequest = new AdRequest.Builder().build();
		m_adView.loadAd(adRequest);
		m_adView.setVisibility(View.INVISIBLE);
		init_cyad();
	}
	
	@Override
	public void init()
	{
		m_Helper = new IabHelper(this, base64EncodedPublicKey);
		m_Helper.startSetup(new IabHelper.OnIabSetupFinishedListener() {
			public void onIabSetupFinished(IabResult result) {
				if (!result.isSuccess()) {
					UnityPlayer.UnitySendMessage("LJSDK", "init_callback_failed", result.getMessage());
					return;
				}
				List<String> ss = new ArrayList<String>();
				ss.add("p_60");
				ss.add("p_300");
				ss.add("p_980");
				ss.add("p_1980");
				ss.add("p_3280");
				ss.add("p_6480");
				m_Helper.queryInventoryAsync(true, ss, mGotInventoryListener);
			}
		});
	}
	
	IabHelper.QueryInventoryFinishedListener mGotInventoryListener = new IabHelper.QueryInventoryFinishedListener() {
        public void onQueryInventoryFinished(IabResult result, Inventory inventory) {
            // Is it a failure?
            if (result.isFailure()) {
            	UnityPlayer.UnitySendMessage("LJSDK", "init_callback_failed", result.getMessage());
                return;
            }

            m_inventory = inventory;
            m_suc = true;
            UnityPlayer.UnitySendMessage("LJSDK", "init_callback_success", "");
            List<String> ss = new ArrayList<String>();
			ss.add("p_60");
			ss.add("p_300");
			ss.add("p_980");
			ss.add("p_1980");
			ss.add("p_3280");
			ss.add("p_6480");
			for (int i = 0; i < ss.size(); ++i)
			{
	            Purchase p = m_inventory.getPurchase(ss.get(i));
	            if (p != null)
	            {
	            	 m_Helper.consumeAsync(p, mConsumeFinishedListenerex);
	            }
			}
        }
    };
    
    IabHelper.OnConsumeFinishedListener mConsumeFinishedListenerex = new IabHelper.OnConsumeFinishedListener() {
        public void onConsumeFinished(Purchase purchase, IabResult result) {
            
        }
    };
	
	@Override
	public void onDestroy() {
		super.onDestroy();
		if (m_Helper != null) m_Helper.dispose();
		m_Helper = null;
	}
	
	@Override
	public void req_info(String code) {
		if (!m_suc)
		{
			UnityPlayer.UnitySendMessage("LJSDK", "recharge_android_product", "");
		}
		else
		{
			SkuDetails sd = m_inventory.getSkuDetails(code);
			UnityPlayer.UnitySendMessage("LJSDK", "recharge_android_product", sd.getPrice() + " " + sd.getSku());
		}
	}
	
	public void pay(String uid, int price, int pid, String name, String code, String callbackurl)
	{
		if (!m_suc)
		{
			UnityPlayer.UnitySendMessage("LJSDK", "pay_callback_failed", "");
			return;
		}
		String payload = "";
        m_Helper.launchPurchaseFlow(this, code, RC_REQUEST, mPurchaseFinishedListener, payload);
	}
	
	IabHelper.OnIabPurchaseFinishedListener mPurchaseFinishedListener = new IabHelper.OnIabPurchaseFinishedListener() {
        public void onIabPurchaseFinished(IabResult result, Purchase purchase) {
        	
            if (result.isFailure()) {
            	UnityPlayer.UnitySendMessage("LJSDK", "pay_callback_failed", "");
                return;
            }

            m_Helper.consumeAsync(purchase, mConsumeFinishedListener);
        }
    };
    
    @Override
    protected void onActivityResult(int requestCode, int resultCode, Intent data) {
        if (m_Helper == null)
		{
        	return;
		}

        if (!m_Helper.handleActivityResult(requestCode, resultCode, data)) {
            super.onActivityResult(requestCode, resultCode, data);
        }
    }
    
    IabHelper.OnConsumeFinishedListener mConsumeFinishedListener = new IabHelper.OnConsumeFinishedListener() {
        public void onConsumeFinished(Purchase purchase, IabResult result) {
            
            if (result.isSuccess()) {
                showToast("Purchase successful.");
                UnityPlayer.UnitySendMessage("LJSDK", "pay_callback_success", purchase.getToken());
            }
            else {
            	showToast("Error while consuming: " + result);
            	UnityPlayer.UnitySendMessage("LJSDK", "pay_callback_failed", "");
            }
        }
    };
	
	public void hfad() {
        // And this is the same, but done programmatically  
		runOnUiThread(new Runnable() {
			@Override
			public void run() {
				m_adView.setVisibility(View.VISIBLE);			
			}
		});
    }
	
	public void close_hfad() {
        // And this is the same, but done programmatically  
		runOnUiThread(new Runnable() {
			@Override
			public void run() {
				m_adView.setVisibility(View.INVISIBLE);			
			}
		});
    }
	
	public void init_cyad() {
		AdRequest adRequest = new AdRequest.Builder().build();
		m_interstitial.loadAd(adRequest);
		AdListener adListener = new AdListener()
		{	 
	        @Override
	        public void onAdClosed() {
	            super.onAdClosed();
	            runOnUiThread(new Runnable() {
	    	        @Override public void run() {
	    	        	init_cyad();
	    	        }
	    		});
	        }
	 
	        @Override
	        public void onAdFailedToLoad(int errorCode) {
	        	super.onAdFailedToLoad(errorCode);
	        	init_cyad();
	        }
		};
		m_interstitial.setAdListener(adListener);
	}
	
	public void cyad() {
		runOnUiThread(new Runnable() {
	        @Override public void run() {
	        	if (m_interstitial.isLoaded())
	    		{
	    			m_interstitial.show();
	    		}	
	        }
		});
	}
	
	/**
	 * 用于获取渠道标识，游戏开发者可在任意处调用该方法获取到该字段，含义请参照《如何区分渠道》中的渠道与ChannelLabel对照表
	 * 
	 * @return
	 */
	@Override
	public String getChanelLabel() {
		return "google";
	}
}
