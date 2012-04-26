//
//  RHITMobileAppDelegate.h
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
@interface RHAppDelegate : NSObject <UIApplicationDelegate, UITabBarControllerDelegate>

/// The application's window.
@property (nonatomic, strong) IBOutlet UIWindow *window;

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

- (void)prefetchLocationNames;

/// Callback notifying the application delegate of managed object contexts
/// performing save operations on secondary threads. This method triggers a
/// merge and update operation on the main thread's NSManagedObjectContext so
/// that changes made elsewhere will be reflected on the main thread.
- (void)managedContextDidSave:(NSNotification *)notification;

@end