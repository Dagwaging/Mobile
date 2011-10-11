//
//  RHITMobileAppDelegate.m
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

#import "RHITMobileAppDelegate.h"
#import "MapViewController.h"
#import "SearchViewController.h"

@interface RHITMobileAppDelegate ()

- (void)setupDefaults;

@end

@implementation RHITMobileAppDelegate

@synthesize window = _window;
@synthesize tabBarController = _tabBarController;
@synthesize mapViewController;
@synthesize searchViewController;
@synthesize managedObjectModel;
@synthesize managedObjectContext;
@synthesize persistentStoreCoordinator;

- (BOOL)application:(UIApplication *)application 
    didFinishLaunchingWithOptions:(NSDictionary *)launchOptions {
    // Override point for customization after application launch.
    // Add the tab bar controller's current view as a subview of the window
    self.window.rootViewController = self.tabBarController;
    [self.window makeKeyAndVisible];
    self.mapViewController.managedObjectContext = self.managedObjectContext;
    [self setupDefaults];
    return YES;
}

- (void)applicationWillResignActive:(UIApplication *)application {
    /*
     Sent when the application is about to move from active to inactive state. 
     This can occur for certain types of temporary interruptions (such as an 
     incoming phone call or SMS message) or when the user quits the application 
     and it begins the transition to the background state. Use this method to 
     pause ongoing tasks, disable timers, and throttle down OpenGL ES frame 
     rates. Games should use this method to pause the game.
     */
}

- (void)applicationDidEnterBackground:(UIApplication *)application {
    /*
     Use this method to release shared resources, save user data, invalidate 
     timers, and store enough application state information to restore your 
     application to its current state in case it is terminated later. 
     If your application supports background execution, this method is called 
     instead of applicationWillTerminate: when the user quits.
     */
}

- (void)applicationWillEnterForeground:(UIApplication *)application {
    /*
     Called as part of the transition from the background to the inactive 
     state; here you can undo many of the changes made on entering the 
     background.
     */
}

- (void)applicationDidBecomeActive:(UIApplication *)application {
    /*
     Restart any tasks that were paused (or not yet started) while the 
     application was inactive. If the application was previously in the 
     background, optionally refresh the user interface.
     */
    [self.mapViewController refreshPreferences];
}

- (void)applicationWillTerminate:(UIApplication *)application {
    /*
     Called when the application is about to terminate.
     Save data if appropriate.
     See also applicationDidEnterBackground:.
     */
}

- (void)dealloc {
    [_window release];
    [_tabBarController release];
    [managedObjectContext release];
    [managedObjectModel release];
    [persistentStoreCoordinator release];
    [super dealloc];
}

/*
// Optional UITabBarControllerDelegate method.
- (void)tabBarController:(UITabBarController *)tabBarController didSelectViewController:(UIViewController *)viewController
{
}
*/

/*
// Optional UITabBarControllerDelegate method.
- (void)tabBarController:(UITabBarController *)tabBarController didEndCustomizingViewControllers:(NSArray *)viewControllers changed:(BOOL)changed
{
}
*/

- (NSManagedObjectContext *)managedObjectContext {
    if (managedObjectContext != nil) {
        return managedObjectContext;
    }
    
    NSPersistentStoreCoordinator *coordinator;
    coordinator = [self persistentStoreCoordinator];
    
    if (coordinator != nil) {
        managedObjectContext = [[NSManagedObjectContext alloc] init];
        [managedObjectContext setPersistentStoreCoordinator: coordinator];
    }
    
    return managedObjectContext;
}

- (NSManagedObjectModel *)managedObjectModel {
    if (managedObjectModel != nil) {
        return managedObjectModel;
    }
    
    managedObjectModel = [[NSManagedObjectModel mergedModelFromBundles:nil]
                          retain];
    
    return managedObjectModel;
}

- (NSPersistentStoreCoordinator *)persistentStoreCoordinator {
    
    if (persistentStoreCoordinator != nil) {
        return persistentStoreCoordinator;
    }
    
    NSString *path = [self.applicationDocumentsDirectory
                      stringByAppendingPathComponent:@"RHITMobile.sqlite"];
    NSURL *storeUrl = [NSURL fileURLWithPath:path];
    NSError *error = nil;
    
    NSManagedObjectModel *model = [self managedObjectModel];
    
    persistentStoreCoordinator = [[NSPersistentStoreCoordinator alloc]
                                  initWithManagedObjectModel:model];
    
    if(![persistentStoreCoordinator addPersistentStoreWithType:NSSQLiteStoreType
                                                 configuration:nil
                                                           URL:storeUrl
                                                       options:nil
                                                         error:&error]) {
        /* Error for store creation should be handled in here */
    }
    
    return persistentStoreCoordinator;
}

- (NSString *)applicationDocumentsDirectory {
    return [NSSearchPathForDirectoriesInDomains(NSDocumentDirectory,
                                                NSUserDomainMask,
                                                YES) lastObject];
}

- (void)clearDatabase {
    NSArray *stores = [persistentStoreCoordinator persistentStores];
    NSPersistentStore *store = [stores objectAtIndex:0];
    
    // Delete existing database
    NSError *error = nil;
    NSURL *storeURL = store.URL;
    [persistentStoreCoordinator removePersistentStore:store
                                                error:&error];
    [[NSFileManager defaultManager] removeItemAtPath:storeURL.path
                                               error:&error];
    
    // Create new database
    NSString *path = [self.applicationDocumentsDirectory
                      stringByAppendingPathComponent:@"RHITMobile.sqlite"];
    NSURL *storeUrl = [NSURL fileURLWithPath:path];
    error = nil;
    
    if(![persistentStoreCoordinator addPersistentStoreWithType:NSSQLiteStoreType
                                                 configuration:nil
                                                           URL:storeUrl
                                                       options:nil
                                                         error:&error]) {
        /* Error for store creation should be handled in here */
    }
}

#pragma mark -
#pragma mark Private Methods

-(void)setupDefaults {
    //get the plist location from the settings bundle
    NSString *settingsPath = [[[NSBundle mainBundle] bundlePath]
                              stringByAppendingPathComponent:@"Settings.bundle"];
    NSString *plistPath = [settingsPath
                           stringByAppendingPathComponent:@"Root.plist"];
    
    //get the preference specifiers array which contains the settings
    NSDictionary *settingsDictionary = [NSDictionary
                                        dictionaryWithContentsOfFile:plistPath];
    NSArray *preferencesArray = [settingsDictionary
                                 objectForKey:@"PreferenceSpecifiers"];
    
    //use the shared defaults object
    NSUserDefaults *defaults = [NSUserDefaults standardUserDefaults];
    
    //for each preference item, set its default if there is no value set
    for(NSDictionary *item in preferencesArray) {
        
        //get the item key, if there is no key then we can skip it
        NSString *key = [item objectForKey:@"Key"];
        if (key) {
            
            //check to see if the value and default value are set
            //if a default value exists and the value is not set, use the default
            id value = [defaults objectForKey:key];
            id defaultValue = [item objectForKey:@"DefaultValue"];
            if(defaultValue && !value) {
                [defaults setObject:defaultValue forKey:key];
            }
        }
    }
    
    //write the changes to disk
    [defaults synchronize];
}

@end