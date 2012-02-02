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


@class RHMapViewController;
@class RHSearchViewController;
@class RHCampusServicesViewController;

// Conditionally care about beta classes.
#ifdef RHITMobile_RHBeta
@class RHBetaViewController;
#endif

/// Application delegate for the RHITMobile app. This class is responsible for
/// application-level initialization, as well as being the main owner of most of
/// the CoreData objects that propgate throughout the main thread.
@interface RHITMobileAppDelegate : NSObject <UIApplicationDelegate, UITabBarControllerDelegate>

/// The application's window.
@property (nonatomic, strong) IBOutlet UIWindow *window;

/// The application's tab bar controller.
@property (nonatomic, strong) IBOutlet UITabBarController *tabBarController;

/// The UINavigationController associated with the "Map" tab bar item. The first
/// view controller pushed to this navigation controller upon initialization
/// is a MapViewController.
@property (nonatomic, strong) IBOutlet UINavigationController *mapNavigationViewController;

/// The UINavigationController associated with the "Directory" tab bar item. The
/// first view controller pushed to this navigation controller upon
/// initialization is a DirectoryViewController.
@property (nonatomic, strong) IBOutlet UINavigationController *directoryNavigationViewController;

/// The UINavigationController associated with the "Info" tab bar item. The
/// first view controller pushed to this navigation controller upon
/// initialization is an InfoViewController.
@property (nonatomic, strong) IBOutlet UINavigationController *infoNavigationViewController;

@property (nonatomic, strong) RHCampusServicesViewController *infoViewController;

@property (nonatomic, strong) IBOutlet UINavigationController *toursNavigationViewController;

/// The single MapViewController which lives under than "Map" tab as the root
/// view controller of that tab's view controller. There is only ever one
/// MapViewController, and all map interactions and modifications are done
/// through interactions with it.
@property (nonatomic, strong) RHMapViewController *mapViewController;

/// The filepath to this application's documents directory. This value is used
/// to interact directly with the CoreData object store.
@property (unsafe_unretained, nonatomic, readonly) NSString *applicationDocumentsDirectory;

/// The CoreData NSManagedObjectModel for the application.
@property (nonatomic, strong, readonly) NSManagedObjectModel *managedObjectModel;

/// The CoreData NSManagedObjectContext for the main thread of the application.
/// All database persistent objects that are created or retrieved on the main
/// thread are products of this context.
@property (nonatomic, strong, readonly) NSManagedObjectContext *managedObjectContext;

/// The universal CoreData NSPersistentStoreCoordinator for the application.
/// This object is used by secondary threads to create their own
/// NSManagedObjectContext objects for database interactions.
@property (nonatomic, strong, readonly) NSPersistentStoreCoordinator *persistentStoreCoordinator;

@property (nonatomic, strong) NSDictionary *locationNames;

+ (RHITMobileAppDelegate *)instance;

- (void)prefetchLocationNames;

/// Clear the entire CoreData SQLite database for the application. This should
/// only be called immediately before a complete data reload, likely triggered
/// by a response from the server indicating that the current data set is
/// out of date.
- (void)clearDatabase;

/// Callback notifying the application delegate of managed object contexts
/// performing save operations on secondary threads. This method triggers a
/// merge and update operation on the main thread's NSManagedObjectContext so
/// that changes made elsewhere will be reflected on the main thread.
- (void)managedContextDidSave:(NSNotification *)notification;

@end