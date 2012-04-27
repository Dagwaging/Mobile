//
//  RHITMobileAppDelegate.m
//  Rose-Hulman Mobile
//
//  Copyright 2012 Rose-Hulman Institute of Technology
// 
//  Licensed under the Apache License, Version 2.0 (the "License");
//  you may not use this file except in compliance with the License.
//  You may obtain a copy of the License at
//
//      http://www.apache.org/licenses/LICENSE-2.0
//
//  Unless required by applicable law or agreed to in writing, software
//  distributed under the License is distributed on an "AS IS" BASIS,
//  WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//  See the License for the specific language governing permissions and
//  limitations under the License.
//

#import "RHAppDelegate.h"
#import "RHMapViewController.h"
#import "RHSearchViewController.h"
#import "RHCampusServicesViewController.h"
#import "RHLocation.h"
#import "RHToursViewController.h"

#import "RHLoadersWrapper.h"


#pragma mark - Implementation

@implementation RHAppDelegate

#pragma mark - General Properties

@synthesize window = _window;
@synthesize managedObjectModel;
@synthesize managedObjectContext;
@synthesize persistentStoreCoordinator;
@synthesize locationNames;

#pragma mark - UIAppDelegate Methods

- (BOOL)application:(UIApplication *)application 
didFinishLaunchingWithOptions:(NSDictionary *)launchOptions {
    [RHLoadersWrapper updateAllStoredData];
    [self.window makeKeyAndVisible];
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
    //[self setupDefaults];
}

- (void)applicationWillTerminate:(UIApplication *)application {
    /*
     Called when the application is about to terminate.
     Save data if appropriate.
     See also applicationDidEnterBackground:.
     */
}

#pragma mark - General Methods


- (void)prefetchLocationNames {
    if ([NSThread isMainThread]) {
        [self performSelectorInBackground:@selector(prefetchLocationNames)
                               withObject:nil];
    } else {
        NSFetchRequest *request = [NSFetchRequest
                                   fetchRequestWithEntityName:kRHLocationEntityName];
        NSArray *fetchResults = [managedObjectContext
                                 executeFetchRequest:request
                                 error:nil];
        NSMutableDictionary *names = [NSMutableDictionary
                                      dictionaryWithCapacity:fetchResults.count];
        
        for (RHLocation *location in fetchResults) {
            if (location.name != nil) {
                [names setObject:location.objectID forKey:location.name];
            }
        }
        
        self.locationNames = names;
    }
}

#pragma mark - Property Methods

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
    
    managedObjectModel = [NSManagedObjectModel mergedModelFromBundles:nil];
    
    return managedObjectModel;
}

- (NSPersistentStoreCoordinator *)persistentStoreCoordinator {
    
    @synchronized(self) {
        
        if (persistentStoreCoordinator != nil) {
            return persistentStoreCoordinator;
        }
        
        NSString *path = [self.applicationDocumentsDirectory
                          stringByAppendingPathComponent:@"CachedData.sqlite"];
        NSURL *storeUrl = [NSURL fileURLWithPath:path];
        NSError *error = nil;
        
        NSManagedObjectModel *model = [self managedObjectModel];
        
        NSDictionary *options = [NSDictionary dictionaryWithObjectsAndKeys:
                                 [NSNumber numberWithBool:YES], NSMigratePersistentStoresAutomaticallyOption,
                                 [NSNumber numberWithBool:YES], NSInferMappingModelAutomaticallyOption, nil];
        persistentStoreCoordinator = [[NSPersistentStoreCoordinator alloc]
                                      initWithManagedObjectModel:model];
        
        if(![persistentStoreCoordinator addPersistentStoreWithType:NSSQLiteStoreType
                                                     configuration:nil
                                                               URL:storeUrl
                                                           options:options
                                                             error:&error]) {
            /* Error for store creation should be handled in here */
        }
        
        return persistentStoreCoordinator;
        
    }
}

- (NSString *)applicationDocumentsDirectory {
    return [NSSearchPathForDirectoriesInDomains(NSDocumentDirectory,
                                                NSUserDomainMask,
                                                YES) lastObject];
}

- (void)managedContextDidSave:(NSNotification *)notification {
    if ([NSThread isMainThread]) {
        [self.managedObjectContext
         mergeChangesFromContextDidSaveNotification:notification];
    } else {
        [self performSelectorOnMainThread:@selector(managedContextDidSave:) 
                               withObject:notification
                            waitUntilDone:YES];
    }
}

@end