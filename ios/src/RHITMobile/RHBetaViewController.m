//
//  RHBetaViewController.m
//  RHIT Mobile Campus Directory
//
//  Copyright 2011 Rose-Hulman Institute of Technology
// 
//  Licensed under the Apache License, Version 2.0 (the "License");
//  you may not use this file except in compliance with the License.
//  You may obtain a copy of the License at
//
//  http://www.apache.org/licenses/LICENSE-2.0
//
//  Unless required by applicable law or agreed to in writing, software
//  distributed under the License is distributed on an "AS IS" BASIS,
//  WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//  See the License for the specific language governing permissions and
//  limitations under the License.
//

#include <sys/types.h>
#include <sys/sysctl.h>

#import "RHBetaViewController.h"
#import "RHBeta.h"
#import "CJSONDeserializer.h"
#import "NSDictionary_JSONExtensions.h"
#import "RHAppDelegate.h"
#import "RHBetaRegistrationViewController.h"
#import "RHMapViewController.h"
#import "RHRestHandler.h"
#import "RHPListStore.h"

#ifdef RHITMobile_RHBeta

#define kBetaServer @"http://rhitmobilebeta.heroku.com"
#define kBetaUpdatePath @"/platform/ios/builds/current"
#define kBetaRegisterPath @"/device/register"
#define kBetaNotifyPath @"/device/update"

#define kBetaUpdateTimeDefault @"LastUpdateTime"
#define kBetaAuthTokenDefault @"AuthToken"
#define kBetaCurrentBuildDefault @"CurrentBuild"

#define kBetaApplicationVersionLabel @"Application Version"
#define kBetaApplicationVersionCell @"ApplicationVersionCell"
#define kBetaBuildNumberLabel @"Build Number"
#define kBetaBuildNumberCell @"BuildNumberCell"
#define kBetaBuildTypeLabel @"Build Type"
#define kBetaBuildTypeCell @"BuildTypeCell"
#define kBetaUpdateTimeLabel @"Last Updated"
#define kBetaUpdateTimeCell @"UpdateTimeCell"
#define kBetaAuthTokenLabel @"Beta Authentication Token"
#define kBetaAuthTokenCell @"AuthTokenCell"
#define kBetaMapDebugLabel @"Map Debugging Tools"
#define kBetaMapDebugCell @"MapDebugCell"
#define kBetaDatabaseToolsLabel @"Database Tools"

@interface RHBetaViewController ()

@property (nonatomic, strong) NSArray *sections;
@property (nonatomic, assign) BOOL checkingForUpdates;
@property (nonatomic, assign) BOOL initialUpdateCheck;
@property (nonatomic, strong) NSString *authToken;
@property (nonatomic, strong) NSDate *updateDate;
@property (nonatomic, assign) NSInteger knownCurrentBuild;
@property (nonatomic, strong) NSOperationQueue *operations;
@property (nonatomic, strong) NSURL *updateURL;

- (IBAction)switchInstallationType:(id)sender;
- (IBAction)checkForUpdates:(id)sender;
- (IBAction)clearAndReloadData:(id)sender;
- (void)didFindNoUpdates;
- (void)didFindUpdateWithURL:(NSURL *)url;
- (void)performCheckForUpdates:(NSNumber *)official;
- (void)performNotificationOfUpdate;
- (void)setLoadingText:(NSString *)text;
- (void)clearLoadingText;

@end


@implementation RHBetaViewController

@synthesize registrationName;
@synthesize registrationEmail;

@synthesize sections;
@synthesize checkingForUpdates;
@synthesize initialUpdateCheck;
@synthesize updateDate;
@synthesize knownCurrentBuild;
@synthesize authToken;
@synthesize operations;
@synthesize updateURL;

- (id)initWithNibName:(NSString *)nibNameOrNil
               bundle:(NSBundle *)nibBundleOrNil {
    self = [super initWithNibName:nibNameOrNil bundle:nibBundleOrNil];
    if (self) {
        self.navigationItem.title = @"Beta Tools and Info";
        self.sections = [NSArray arrayWithObjects:kBetaApplicationVersionLabel,
                         kBetaBuildNumberLabel, kBetaBuildTypeLabel,
                         kBetaUpdateTimeLabel, kBetaAuthTokenLabel,
                         kBetaDatabaseToolsLabel, nil];
        self.operations = [[NSOperationQueue alloc] init];
    }
    return self;
}

- (void)didReceiveMemoryWarning {
    // Releases the view if it doesn't have a superview.
    [super didReceiveMemoryWarning];
    
    // Release any cached data, images, etc that aren't in use.
}

- (void)performInitialSetup {
    NSUserDefaults *defaults = [NSUserDefaults standardUserDefaults];
    
    self.authToken = [defaults stringForKey:kBetaAuthTokenDefault];
    
    if (self.authToken == nil) {
        RHBetaRegistrationViewController *registrationController = [[RHBetaRegistrationViewController alloc] initWithNibName:kRHBetaRegistrationViewControllerNibName bundle:nil];
        registrationController.betaViewController = self;
        [self presentModalViewController:registrationController animated:YES];
    }
    
    double updateNumber = [defaults doubleForKey:kBetaUpdateTimeDefault];
    
    if (updateNumber == 0) {
        updateNumber = (double) [[NSDate date] timeIntervalSince1970];
    }
    
    self.knownCurrentBuild = [defaults integerForKey:kBetaCurrentBuildDefault];
    
    if (self.knownCurrentBuild != kRHBetaBuildNumber) {
        self.knownCurrentBuild = kRHBetaBuildNumber;
        updateNumber = (double) [[NSDate date] timeIntervalSince1970];

        NSInvocationOperation* operation = [NSInvocationOperation alloc];
        operation = [operation
                      initWithTarget:self
                      selector:@selector(performNotificationOfUpdate)
                      object:nil];
        [self.operations addOperation:operation];
    }
    
    [defaults setDouble:updateNumber forKey:kBetaUpdateTimeDefault];
    self.updateDate = [NSDate dateWithTimeIntervalSince1970:updateNumber];
    self.initialUpdateCheck = YES;
    
    NSInvocationOperation* operation = [NSInvocationOperation alloc];
    operation = [operation
                  initWithTarget:self
                  selector:@selector(performCheckForUpdates:)
                  object:[NSNumber numberWithBool:YES]];
    [self.operations addOperation:operation];
}

#pragma mark - View lifecycle

- (void)viewDidLoad {
    [super viewDidLoad];
    // Do any additional setup after loading the view from its nib.
}

- (void)viewDidUnload {
    [super viewDidUnload];
    // Release any retained subviews of the main view.
    // e.g. self.myOutlet = nil;
}

- (BOOL)shouldAutorotateToInterfaceOrientation:(UIInterfaceOrientation)io {
    // Return YES for supported orientations
    return (io == UIInterfaceOrientationPortrait);
}


#pragma mark - UITableViewDelegate Method

- (UIView *)tableView:(UITableView *)tableView
viewForFooterInSection:(NSInteger)section {
    NSString *sectionLabel = [self.sections objectAtIndex:section];
    if ([sectionLabel isEqualToString:kBetaDatabaseToolsLabel]) {
        UIView *parentView = [[UIView alloc] initWithFrame:CGRectZero];
        parentView.backgroundColor = [UIColor clearColor];
        
        UIButton *updateButton = [UIButton buttonWithType:UIButtonTypeRoundedRect];
        updateButton.frame = CGRectMake(10.0, 10.0, 300.0, 44.0);
        [updateButton addTarget:self
                         action:@selector(clearAndReloadData:)
               forControlEvents:UIControlEventTouchUpInside];
        [updateButton setTitle:@"Clear and Reload Data"
                      forState:UIControlStateNormal];
        
        [parentView addSubview:updateButton];
        
        return parentView;
    } else if ([sectionLabel isEqualToString:kBetaBuildTypeLabel]) {
        UIView *parentView = [[UIView alloc] initWithFrame:CGRectZero];
        parentView.backgroundColor = [UIColor clearColor];
        
        UIButton *updateButton = [UIButton buttonWithType:UIButtonTypeRoundedRect];
        updateButton.frame = CGRectMake(10.0, 10.0, 300.0, 44.0);
        [updateButton addTarget:self
                         action:@selector(switchInstallationType:)
               forControlEvents:UIControlEventTouchUpInside];
        
        NSString *buttonTitle = nil;
        
        if (kRHBetaBuildType == kRHBetaBuildTypeOfficial) {
            buttonTitle = @"Switch to Bleeding Edge";
        } else {
            buttonTitle = @"Switch to Stable";
        }
        
        [updateButton setTitle:buttonTitle forState:UIControlStateNormal];
        [parentView addSubview:updateButton];
        return parentView;
    } else if ([sectionLabel isEqualToString:kBetaUpdateTimeLabel]) {
        UIView *parentView = [[UIView alloc] initWithFrame:CGRectZero];
        parentView.backgroundColor = [UIColor clearColor];
        
        UIButton *updateButton = [UIButton buttonWithType:UIButtonTypeRoundedRect];
        updateButton.frame = CGRectMake(10.0, 10.0, 300.0, 44.0);
        [updateButton addTarget:self
                         action:@selector(checkForUpdates:)
               forControlEvents:UIControlEventTouchUpInside];
        [updateButton setTitle:@"Check for Updates"
                      forState:UIControlStateNormal];
        
        [parentView addSubview:updateButton];
        
        return parentView; 
    }
    
    return nil;
}

-(CGFloat)tableView:(UITableView *)tableView
heightForFooterInSection:(NSInteger)section {
    NSString *sectionLabel = [self.sections objectAtIndex:section];
    
    if ([sectionLabel isEqualToString:kBetaDatabaseToolsLabel]) {
        return 64;
    }
    
    if ([sectionLabel isEqualToString:kBetaBuildTypeLabel]) {
        return 64;
    }
    
    if ([sectionLabel isEqualToString:kBetaUpdateTimeLabel]) {
        return 64;
    }
    
    return 0;
}

#pragma mark - UITableViewDataSource Method

- (UITableViewCell *)tableView:(UITableView *)tableView
         cellForRowAtIndexPath:(NSIndexPath *)indexPath {
    NSString *sectionLabel = [self.sections objectAtIndex:[indexPath indexAtPosition:0]];
    UITableViewCell *cell = nil;
    
    if (sectionLabel == kBetaApplicationVersionLabel) {
        cell = [tableView dequeueReusableCellWithIdentifier:kBetaApplicationVersionCell];
        
        if (cell == nil) {
            cell = [[UITableViewCell alloc] initWithStyle:UITableViewCellStyleDefault reuseIdentifier:kBetaApplicationVersionCell];
            cell.selectionStyle = UITableViewCellSelectionStyleNone;
        }
        
        cell.textLabel.text = [[[NSBundle mainBundle]
                                infoDictionary]
                               objectForKey:@"CFBundleVersion"];
    } else if (sectionLabel == kBetaBuildNumberLabel) {
        cell = [tableView dequeueReusableCellWithIdentifier:kBetaBuildNumberCell];
        
        if (cell == nil) {
            cell = [[UITableViewCell alloc] initWithStyle:UITableViewCellStyleDefault reuseIdentifier:kBetaBuildNumberCell];
            cell.selectionStyle = UITableViewCellSelectionStyleNone;
        }
        
        cell.textLabel.text = [[NSString alloc] initWithFormat:@"%d",
                                kRHBetaBuildNumber];
    } else if (sectionLabel == kBetaBuildTypeLabel) {
        cell = [tableView dequeueReusableCellWithIdentifier:kBetaBuildTypeCell];
        
        if (cell == nil) {
            cell = [[UITableViewCell alloc] initWithStyle:UITableViewCellStyleDefault reuseIdentifier:kBetaBuildTypeCell];
            cell.selectionStyle = UITableViewCellSelectionStyleNone;
        }
        
        if (kRHBetaBuildType == kRHBetaBuildTypeOfficial) {
            cell.textLabel.text = @"Stable";
        } else {
            cell.textLabel.text = @"Bleeding Edge";
        }
    } else if (sectionLabel == kBetaUpdateTimeLabel) {
        cell = [tableView dequeueReusableCellWithIdentifier:kBetaUpdateTimeCell];
        
        if (cell == nil) {
            cell = [[UITableViewCell alloc] initWithStyle:UITableViewCellStyleDefault reuseIdentifier:kBetaUpdateTimeCell];
            cell.selectionStyle = UITableViewCellSelectionStyleNone;
        }
        
        NSDateFormatter *formatter = [[NSDateFormatter alloc] init];
        formatter.dateStyle = NSDateFormatterMediumStyle;
        formatter.timeStyle = NSDateFormatterShortStyle;
        cell.textLabel.text = [formatter stringFromDate:self.updateDate];
    } else if (sectionLabel == kBetaAuthTokenLabel) {
        cell = [tableView dequeueReusableCellWithIdentifier:kBetaAuthTokenCell];
        
        if (cell == nil) {
            cell = [[UITableViewCell alloc] initWithStyle:UITableViewCellStyleDefault reuseIdentifier:kBetaAuthTokenCell];
            cell.selectionStyle = UITableViewCellSelectionStyleNone;
            cell.textLabel.font = [UIFont systemFontOfSize:14];
        }
        
        NSUserDefaults *defaults = [NSUserDefaults standardUserDefaults];
        cell.textLabel.text = [defaults stringForKey:kBetaAuthTokenDefault];
    }
    
    return cell;
}

- (NSInteger)numberOfSectionsInTableView:(UITableView *)tableView {
    return self.sections.count;
}

- (NSString *)tableView:(UITableView *)tableView
titleForHeaderInSection:(NSInteger)section {
    return [self.sections objectAtIndex:section];
}

- (NSInteger)tableView:(UITableView *)tableView
 numberOfRowsInSection:(NSInteger)section  {
    NSString *sectionLabel = [self.sections objectAtIndex:section];
    
    if (sectionLabel == kBetaDatabaseToolsLabel) {
        return 0;
    } else if (sectionLabel == kBetaApplicationVersionLabel) {
        return 1;
    } else if (sectionLabel == kBetaBuildNumberLabel) {
        return 1;
    } else if (sectionLabel == kBetaBuildTypeLabel) {
        return 1;
    } else if (sectionLabel == kBetaUpdateTimeLabel) {
        return 1;
    } else if (sectionLabel == kBetaAuthTokenLabel)  {
        return 1;
    }
    
    return 0;
}

#pragma mark - UIActionSheetDelegate Methods

- (void)actionSheet:(UIActionSheet *)actionSheet
didDismissWithButtonIndex:(NSInteger)buttonIndex {
    if (buttonIndex == 0) {
        [self setLoadingText:@"Fetching Update"];
        self.checkingForUpdates = YES;
        NSInvocationOperation* operation = [NSInvocationOperation alloc];
        operation = [operation
                      initWithTarget:self
                      selector:@selector(performCheckForUpdates:)
                      object:[NSNumber numberWithBool:(kRHBetaBuildType != kRHBetaBuildTypeOfficial)]];
        [self.operations addOperation:operation];
    }
}

#pragma mari - UIAlertViewDelegate Methods

- (void)alertView:(UIAlertView *)alertView
didDismissWithButtonIndex:(NSInteger)buttonIndex {
    if (buttonIndex == 1) {
        [[UIApplication sharedApplication] openURL:self.updateURL];
    }
}

#pragma mark - Private Methods

- (IBAction)switchInstallationType:(id)sender {
    UIActionSheet *actionSheet = [[UIActionSheet alloc] initWithTitle:@"Are You Sure?" delegate:self cancelButtonTitle:@"Cancel" destructiveButtonTitle:@"Switch Build Types" otherButtonTitles:nil];
    
    RHAppDelegate *appDelegate = (RHAppDelegate *) [UIApplication sharedApplication].delegate;
    [actionSheet showFromTabBar:appDelegate.tabBarController.tabBar];
}

- (IBAction)checkForUpdates:(id)sender {
    if (self.checkingForUpdates) {
        return;
    }
    
    self.checkingForUpdates = YES;
    [self setLoadingText:@"Checking for Updates"];
    NSInvocationOperation* operation = [NSInvocationOperation alloc];
    operation = [operation
                  initWithTarget:self
                  selector:@selector(performCheckForUpdates:)
                  object:[NSNumber
                          numberWithBool:kRHBetaBuildType == kRHBetaBuildTypeOfficial]];
    [self.operations addOperation:operation];
}

- (IBAction)clearAndReloadData:(id)sender {
    [RHITMobileAppDelegate.instance clearDatabase];
    RHPListStore *listStore = [[RHPListStore alloc] init];
    listStore.currentMapDataVersion = @"-1";
    listStore.currentServicesDataVersion = @"-1";
    [RHITMobileAppDelegate.instance.mapViewController.remoteHandler checkForLocationUpdates];
}

- (void)performCheckForUpdates:(NSNumber *)officialNumber {
    // Parse out BOOL for whether or not we're looking for an official build
    BOOL official = officialNumber.boolValue;
    
    // Retrieve data from web service
    NSURL *requestURL = [NSURL
                         URLWithString:[kBetaServer 
                                        stringByAppendingString:kBetaUpdatePath]];
    NSURLRequest *request = [NSURLRequest requestWithURL:requestURL];
    NSURLResponse *response = nil;
    NSData *data = [NSURLConnection sendSynchronousRequest:request
                                         returningResponse:&response
                                                     error:nil];
    NSDictionary *parsed = [NSDictionary dictionaryWithJSONData:data
                                                          error:nil];
    
    // Determine which type of build we're looking for
    NSDictionary *relevantBuild = nil;
    if (official) {
        relevantBuild = [parsed objectForKey:@"official"];
    } else {
        relevantBuild = [parsed objectForKey:@"rolling"];
    }
    
    if ([relevantBuild isKindOfClass:[NSNull class]]) {
        return [self performSelectorOnMainThread:@selector(didFindNoUpdates)
                                      withObject:nil
                                   waitUntilDone:NO];
    }
    
    // Set up build numbers for comparison
    NSNumberFormatter *formatter = [[NSNumberFormatter alloc] init];
    [formatter setNumberStyle:NSNumberFormatterDecimalStyle];
    NSNumber *newBuildNumber = [formatter
                                numberFromString:[relevantBuild
                                                  objectForKey:@"buildNumber"]];
    NSNumber *oldBuildNumber = [NSNumber numberWithInt:kRHBetaBuildNumber];
    
    // Determine if we need to actually update, then call the appropriate method
    if ([newBuildNumber compare:oldBuildNumber] == NSOrderedDescending ||
        (official && [newBuildNumber compare:oldBuildNumber] != NSOrderedSame &&
         !self.initialUpdateCheck)) {
        NSURL *url = [NSURL URLWithString:[relevantBuild
                                           objectForKey:@"downloadURL"]];
        [self performSelectorOnMainThread:@selector(didFindUpdateWithURL:)
                               withObject:url
                            waitUntilDone:NO];
    } else {
        [self performSelectorOnMainThread:@selector(didFindNoUpdates)
                               withObject:nil
                            waitUntilDone:NO];
    }
}

- (void)setLoadingText:(NSString *)text {
    self.navigationItem.title = text;
    UIActivityIndicatorView* activityIndicatorView = [[UIActivityIndicatorView alloc] initWithFrame:CGRectMake(20, 0, 20, 20)];
    [activityIndicatorView startAnimating];
    
    UIBarButtonItem *activityButtonItem = [[UIBarButtonItem alloc] initWithCustomView:activityIndicatorView];
    self.navigationItem.leftBarButtonItem = activityButtonItem;
}

- (void)clearLoadingText {
    self.navigationItem.title = @"Beta Tools and Info";
    self.navigationItem.leftBarButtonItem = nil;
}

- (void)didFindNoUpdates {
    [self clearLoadingText];
    self.checkingForUpdates = NO;
    if (self.initialUpdateCheck) {
        self.initialUpdateCheck = NO;
    } else {
        [[[UIAlertView alloc] initWithTitle:@"No Updates Found" message:@"You are already using the latest version of Rose-Hulman." delegate:nil cancelButtonTitle:@"OK" otherButtonTitles:nil] show];
    }
}

- (void)didFindUpdateWithURL:(NSURL *)url {
    [self clearLoadingText];
    self.checkingForUpdates = NO;
    
    if (self.initialUpdateCheck) {
        self.initialUpdateCheck = NO;
        self.updateURL = url;
        [[[UIAlertView alloc] initWithTitle:@"Update Available" message:@"An update is available. Would you like to install it?" delegate:self cancelButtonTitle:@"Not Yet" otherButtonTitles:@"Install", nil] show];
    } else {
        [[UIApplication sharedApplication] openURL:url];
    }
}

- (void)performRegistration {
    UIDevice *device = [UIDevice currentDevice];
    
    size_t size;
    sysctlbyname("hw.model", NULL, &size, NULL, 0);
    char *machine = malloc(size);
    sysctlbyname("hw.model", machine, &size, NULL, 0);
    NSString *platform = [NSString stringWithCString:machine encoding:NSUTF8StringEncoding];
    free(machine);
    
    NSString *name = self.registrationName;
    NSString *email = self.registrationEmail;
    NSString *deviceID = device.uniqueIdentifier;
    NSString *operatingSystem = [NSString stringWithFormat:@"%@ %@", device.systemName, device.systemVersion];
    NSString *model = platform;
    
    name = [name stringByAddingPercentEscapesUsingEncoding:NSUTF8StringEncoding];
    email = [email stringByAddingPercentEscapesUsingEncoding:NSUTF8StringEncoding];
    deviceID = [deviceID stringByAddingPercentEscapesUsingEncoding:NSUTF8StringEncoding];
    operatingSystem = [operatingSystem stringByAddingPercentEscapesUsingEncoding:NSUTF8StringEncoding];
    model = [model stringByAddingPercentEscapesUsingEncoding:NSUTF8StringEncoding];
    
    NSString *parameters = [NSString stringWithFormat:@"name=%@&email=%@&deviceID=%@&build=%d&operatingSystem=%@&model=%@&platform=ios", name, email, deviceID, kRHBetaBuildNumber, operatingSystem, model];
    
    NSURL *url = [NSURL URLWithString:[kBetaServer stringByAppendingString:kBetaRegisterPath]];
    
    NSMutableURLRequest *request = [NSMutableURLRequest requestWithURL:url];
    
    request.HTTPMethod = @"POST";
    request.HTTPBody = [parameters dataUsingEncoding:NSUTF8StringEncoding];
    
    NSData *data = [NSURLConnection sendSynchronousRequest:request returningResponse:nil error:nil];
    
    NSDictionary *response = [NSDictionary dictionaryWithJSONData:data error:nil];
    
    self.authToken = [response valueForKey:@"authToken"];
    
    if (self.authToken != nil) {
        NSUserDefaults *defaults = [NSUserDefaults standardUserDefaults];
        [defaults setValue:self.authToken forKey:kBetaAuthTokenDefault];
    }
}

- (void)performNotificationOfUpdate {
    NSUserDefaults *defaults = [NSUserDefaults standardUserDefaults];
    
    NSString *token = [defaults stringForKey:kBetaAuthTokenDefault];
    token = [token stringByAddingPercentEscapesUsingEncoding:NSUTF8StringEncoding];
    
    NSString *parameters = [NSString stringWithFormat:@"authToken=%@&build=%d",
                          token, kRHBetaBuildNumber];
    
    NSURL *url = [NSURL URLWithString:[kBetaServer stringByAppendingString:kBetaNotifyPath]];
    
    NSMutableURLRequest *request = [NSMutableURLRequest requestWithURL:url];
    
    request.HTTPMethod = @"POST";
    request.HTTPBody = [parameters dataUsingEncoding:NSUTF8StringEncoding];
    
    NSData *data = [NSURLConnection sendSynchronousRequest:request
                                         returningResponse:nil
                                                     error:nil];
    
    NSDictionary *response = [NSDictionary dictionaryWithJSONData:data error:nil];
    
    if ([[response objectForKey:@"success"] boolValue]) {
        [defaults setInteger:self.knownCurrentBuild forKey:kBetaCurrentBuildDefault];
    }
}

@end

#endif
