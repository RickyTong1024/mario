#import "GoogleMob.h"

@interface GoogleMob ()
	@property(nonatomic, strong) IBOutlet GADBannerView *bannerView;
	@property(nonatomic, strong) GADInterstitial *interstitial;
@end

@implementation GoogleMob

- (void)initad {
	UIViewController *uvc = [[[UIApplication sharedApplication] keyWindow] rootViewController];    
    self.bannerView = [[GADBannerView alloc] initWithFrame:CGRectMake((uvc.view.frame.size.width - GAD_SIZE_320x50.width) * 0.5,uvc.view.frame.size.height -
                                                                      GAD_SIZE_320x50.height,
                                                                      GAD_SIZE_320x50.width,
                                                                      GAD_SIZE_320x50.height)];
    
	self.bannerView.adUnitID = @"ca-app-pub-5826033784775337/2656231408";
	self.bannerView.delegate = self;
	self.bannerView.rootViewController = uvc;
    [uvc.view addSubview:self.bannerView];
    
	GADRequest *request = [GADRequest request];					   
	[self.bannerView loadRequest:request];
	[self.bannerView setHidden: YES];
	
	[self initcyad];
}


- (void)hfad {
	[self.bannerView setHidden: NO];
}

- (void)close_hfad {
	[self.bannerView setHidden: YES];
}

#pragma mark GADBannerViewDelegate impl

// Since we've received an ad, let's go ahead and set the frame to display it.
- (void)adViewDidReceiveAd:(GADBannerView *)adView {
    NSLog(@"Received ad");
}

- (void)adView:(GADBannerView *)view
didFailToReceiveAdWithError:(GADRequestError *)error {
    NSLog(@"Failed to receive ad with error: %@", [error localizedFailureReason]);
}

- (void)initcyad {
	self.interstitial = [[GADInterstitial alloc] initWithAdUnitID:@"ca-app-pub-5826033784775337/1179498209"];
	self.interstitial.delegate = self;

	GADRequest *request = [GADRequest request];
	[self.interstitial loadRequest:request];
}

- (void)cyad {  
	if (self.interstitial.isReady) {
		UIViewController *uvc = [[[UIApplication sharedApplication] keyWindow] rootViewController]; 
		[self.interstitial presentFromRootViewController:uvc];
	}
}

#pragma mark GADInterstitialDelegate implementation

- (void)interstitial:(GADInterstitial *)interstitial
    didFailToReceiveAdWithError:(GADRequestError *)error {
	NSLog(@"interstitialDidFailToReceiveAdWithError: %@", [error localizedDescription]);
}

- (void)interstitialDidDismissScreen:(GADInterstitial *)interstitial {
	NSLog(@"interstitialDidDismissScreen");
	[self initcyad];
}

@end
