//
//  RHITMobileAppDelegate.h
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

#import <UIKit/UIKit.h>
#import <CoreData/CoreData.h>


@class MapViewController;
@class SearchViewController;

/// AppDelgate for the RHITMobile app. Handles application-level delegate
/// functionality.
@interface RHITMobileAppDelegate : NSObject 
<UIApplicationDelegate, UITabBarControllerDelegate>

/// Current window.
@property (nonatomic, retain) IBOutlet UIWindow *window;

/// Main tab bar.
@property (nonatomic, retain) IBOutlet UITabBarController *tabBarController;

/// Map view controller
@property (nonatomic, retain) IBOutlet MapViewController *mapViewController;

/// Search view controller
@property (nonatomic, retain) IBOutlet SearchViewController *searchViewController;

/// Object model for the application
@property (nonatomic, retain, readonly) NSManagedObjectModel *managedObjectModel;

/// Context to execute data interactions against
@property (nonatomic, retain, readonly) NSManagedObjectContext *managedObjectContext;

/// Persistant store coordinator
@property (nonatomic, retain, readonly) NSPersistentStoreCoordinator *persistentStoreCoordinator;

- (NSString *)applicationDocumentsDirectory;

/// Clears all data from this application's storage.
- (void)clearDatabase;

@end