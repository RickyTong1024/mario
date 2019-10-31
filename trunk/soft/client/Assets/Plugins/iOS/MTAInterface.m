#import "MTA.h"
#import "MTAConfig.h"
#import "MTAInterface.h"

@implementation MTAInterface

void startMTA(){
    [MTA startWithAppkey:@"IJ5QDB4CA61X"];
	[[MTAConfig getInstance] setDebugEnable:FALSE];
	
	if ([UIApplication instancesRespondToSelector:@selector(registerUserNotificationSettings:)])
    {
        [[UIApplication sharedApplication] registerUserNotificationSettings:[UIUserNotificationSettings settingsForTypes:UIUserNotificationTypeAlert|UIUserNotificationTypeBadge|UIUserNotificationTypeSound categories:nil]];
    }
	[[UIApplication sharedApplication] setApplicationIconBadgeNumber:0];	
}

void onJewelGet(void *guid, void *scene, void *num) {
	NSString *nguid = [NSString stringWithUTF8String:guid];
	NSString *nscene = [NSString stringWithUTF8String:scene];
	NSString *nnum = [NSString stringWithUTF8String:num];
	
	NSDictionary *kvs = [NSDictionary dictionaryWithObjectsAndKeys:
		nguid, @"uid",
		nscene, @"scene",
		nnum, @"num",
		nil];
	[MTA trackCustomKeyValueEvent:@"onJewelGet" props:kvs];
}

void onJewelConsume(void *guid, void *scene, void *num) {
	NSString *nguid = [NSString stringWithUTF8String:guid];
	NSString *nscene = [NSString stringWithUTF8String:scene];
	NSString *nnum = [NSString stringWithUTF8String:num];
	
	NSDictionary *kvs = [NSDictionary dictionaryWithObjectsAndKeys:
		nguid, @"uid",
		nscene, @"scene",
		nnum, @"num",
		nil];
	[MTA trackCustomKeyValueEvent:@"onJewelConsume" props:kvs];
}

void onUserDo(void *guid, void *scene, void *num) {
	NSString *nguid = [NSString stringWithUTF8String:guid];
	NSString *nscene = [NSString stringWithUTF8String:scene];
	NSString *nnum = [NSString stringWithUTF8String:num];
	
	NSDictionary *kvs = [NSDictionary dictionaryWithObjectsAndKeys:
		nguid, @"uid",
		nscene, @"scene",
		nnum, @"num",
		nil];
	[MTA trackCustomKeyValueEvent:@"onUserDo" props:kvs];
}

void onRaid(void *guid, void *scene, void *num) {
	NSString *nguid = [NSString stringWithUTF8String:guid];
	NSString *nscene = [NSString stringWithUTF8String:scene];
	NSString *nnum = [NSString stringWithUTF8String:num];
	
	NSDictionary *kvs = [NSDictionary dictionaryWithObjectsAndKeys:
		nguid, @"uid",
		nscene, @"scene",
		nnum, @"num",
		nil];
	[MTA trackCustomKeyValueEvent:@"onRaid" props:kvs];
}

void createNotify(void *text, int secondsFromNow) {
	UILocalNotification *newNotification = [[UILocalNotification alloc] init];
    if (newNotification) {
        newNotification.timeZone = [NSTimeZone defaultTimeZone];
		newNotification.repeatInterval = 0;
        newNotification.fireDate = [NSDate dateWithTimeIntervalSinceNow:secondsFromNow];
        newNotification.alertBody = [NSString stringWithUTF8String:text];
		newNotification.alertAction = @"打开";
        newNotification.applicationIconBadgeNumber = 1;
        newNotification.soundName = UILocalNotificationDefaultSoundName;
        [[UIApplication sharedApplication] scheduleLocalNotification:newNotification];
    }
    NSLog(@"Post new localNotification:%@", newNotification);
    [newNotification release];
}

void cancelNotify() {
	[[UIApplication sharedApplication] cancelAllLocalNotifications];
}

@end
