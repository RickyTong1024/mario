#import "MarioTool.h"
#import "GoogleMob.h"

@implementation MarioTool

GoogleMob *googleMob = nil;

void init_tool()
{
	googleMob = [[GoogleMob alloc] init];
    [googleMob initad];
}

char *_getCountryCode() {
    NSLocale *locale = [NSLocale currentLocale];
    NSString *countrycode = [locale objectForKey:NSLocaleCountryCode];
    NSLog(@"gj code：%@",countrycode);
    
    const char *country = [countrycode UTF8String];
    char *back =malloc(countrycode.length + 1);
    char *back2 = back;
    for (int i = 0;i<countrycode.length; i++) {
        *back2 = country[i];
        back2++;
    }
    *back2 = '\0';
    return back;
}

void _hfad() {
	[googleMob hfad];
}

void _close_hfad() {
	[googleMob close_hfad];
}

void _cyad() {
	[googleMob cyad];
}

#pragma mark GADInterstitialDelegate implementation

- (void)interstitial:(GADInterstitial *)interstitial
    didFailToReceiveAdWithError:(GADRequestError *)error {
  NSLog(@"interstitialDidFailToReceiveAdWithError: %@", [error localizedDescription]);
}


@end
