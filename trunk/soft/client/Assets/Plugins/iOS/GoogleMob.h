#import <Foundation/Foundation.h>

@import GoogleMobileAds;

@ interface GoogleMob : NSObject<GADBannerViewDelegate, GADInterstitialDelegate>

- (void)initad;
- (void)hfad;
- (void)close_hfad;
- (void)cyad;

@end
