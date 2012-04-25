//
//  RHCampusServicesLoader.m
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

#import "RHCampusServicesLoader.h"
#import "RHAppDelegate.h"
#import "RHConstants.h"
#import "RHDataVersionsLoader.h"
#import "RHLoaderRequestsWrapper.h"
#import "RHServiceCategory.h"
#import "RHServiceLink.h"

#define kVersionKey @"Version"

#define kRootKey @"ServicesRoot"
#define kChildrenKey @"Children"
#define kLinksKey @"Links"
#define kNameKey @"Name"
#define kURLKey @"Url"


@interface RHCampusServicesLoader () {
@private
    BOOL _currentlyUpdating;
    void (^_callback)(void);
}

- (NSManagedObjectContext *)createThreadSafeManagedObjectContext;

- (void)createManagedObjectsFromLinks:(NSArray *)links
                             category:(RHServiceCategory *)category
                 managedObjectContext:(NSManagedObjectContext *)managedObjectContext;

- (void)createManagedObjectsFromCategories:(NSArray *)categories
                            parentCategory:(RHServiceCategory *)parentCategory 
                      managedObjectContext:(NSManagedObjectContext *)managedObjectContext;

- (void)deleteAllCampusServices:(NSManagedObjectContext *)managedObjectContext;

@end


@implementation RHCampusServicesLoader

static RHCampusServicesLoader *_instance;

+ (void)initialize
{
    static BOOL initialized = NO;
    if(!initialized)
    {
        initialized = YES;
        _instance = [[RHCampusServicesLoader alloc] init];
    }
}

- (id)init
{
    if (self = [super init]) {
        _currentlyUpdating = NO;
        _callback = NULL;
    }
    
    return self;
}

+ (id)instance
{
    return _instance;
}

- (BOOL)currentlyUpdating
{
    return _currentlyUpdating;
}

- (void)updateCampusServices:(NSNumber *)version
{
    if (_currentlyUpdating) {
        return;
    }
    
    // Only operate on background thread
    if ([NSThread isMainThread]) {
        [self performSelectorInBackground:@selector(updateCampusServices:) withObject:version];
        return;
    }
    
#ifdef RHITMobile_RHLoaderDebug
    NSLog(@"Starting campus services update");
#endif
    
    _currentlyUpdating = YES;
    
    NSError *currentError = nil;
    
    NSDictionary *jsonResponse = [RHLoaderRequestsWrapper makeSynchronousCampusServicesRequestWithWithVersion:version error:&currentError];
    
    if (currentError) {
        NSLog(@"Error updating campus services: %@", currentError);
        _currentlyUpdating = NO;
        return;
    }
    
    // Retrieve the new version
    NSNumber *newVersion = [jsonResponse objectForKey:kVersionKey];
    
    if (newVersion.doubleValue <= 0) {
        NSLog(@"New version not specified in campus services. Bailing");
        _currentlyUpdating = NO;
        return;
    }
    
    NSManagedObjectContext *localContext = [self createThreadSafeManagedObjectContext];
    
#ifdef RHITMobile_RHLoaderDebug
    NSLog(@"Deleting old campus services");
#endif
    
    // Delete old campus services
    [self deleteAllCampusServices:localContext];
    
#ifdef RHITMobile_RHLoaderDebug
    NSLog(@"Saving new campus services");
#endif
    
    // Retrieve new categories and links
    NSDictionary *root = [jsonResponse objectForKey:kRootKey];
    [self createManagedObjectsFromLinks:[root objectForKey:kLinksKey]
                               category:nil
                   managedObjectContext:localContext];
    [self createManagedObjectsFromCategories:[root objectForKey:kChildrenKey]
                              parentCategory:nil
                        managedObjectContext:localContext];
    
    NSError *createError;
    [localContext save:&createError];
    
    if (createError) {
        NSLog(@"Problem saving new campus services: %@", createError);
        return;
    }
    
#ifdef RHITMobile_RHLoaderDebug
    NSLog(@"Finished updating campus services");
#endif
    
    // Update version
    [RHDataVersionsLoader.instance setCampusServicesVersion:newVersion];
    
    _currentlyUpdating = NO;
    
#ifdef RHITMobile_RHLoaderDebug
    NSLog(@"Notifying delegates and performing callbacks");
#endif
    
    // Notify delegates
    for (NSObject<RHLoaderDelegate> *delegate in self.delegates) {
        [delegate performSelectorOnMainThread:@selector(loaderDidUpdateUnderlyingData)
                                   withObject:nil
                                waitUntilDone:NO];
    }

    if (_callback != NULL) {
#ifdef RHITMobile_RHLoaderDebug
    NSLog(@"Campus services callback found. Calling.");
#endif
        _callback();
    }
}

- (void)registerCallbackForCampusServices:(void (^)(void))callback
{
    _callback = callback;
}

- (NSManagedObjectContext *)createThreadSafeManagedObjectContext
{
    NSManagedObjectContext *localContext = [[NSManagedObjectContext alloc] init];
    localContext.persistentStoreCoordinator = [(RHAppDelegate *) [[UIApplication sharedApplication] delegate] persistentStoreCoordinator];
    return localContext;
}

- (void)createManagedObjectsFromCategories:(NSArray *)categories
                            parentCategory:(RHServiceCategory *)parentCategory 
                      managedObjectContext:(NSManagedObjectContext *)managedObjectContext
{
    for (NSDictionary *category in categories) {
        // Create managed object from this category
        RHServiceCategory *newCategory;
        newCategory = [NSEntityDescription insertNewObjectForEntityForName:kRHServiceCategoryEntityName
                                                    inManagedObjectContext:managedObjectContext];
        newCategory.name = [category objectForKey:kNameKey];
        newCategory.parent = parentCategory;
        
        // Create managed objects from any leaf hyperlinks
        NSArray *links = [category objectForKey:kLinksKey];
        [self createManagedObjectsFromLinks:links
                                   category:newCategory
                       managedObjectContext:managedObjectContext];
        
        // Recurse into child categories
        NSArray *children = [category objectForKey:kChildrenKey];
        [self createManagedObjectsFromCategories:children
                                  parentCategory:newCategory
                            managedObjectContext:managedObjectContext];
    }
}

- (void)createManagedObjectsFromLinks:(NSArray *)links
                             category:(RHServiceCategory *)category
                 managedObjectContext:(NSManagedObjectContext *)managedObjectContext
{
    for (NSDictionary *link in links) {
        RHServiceLink *newLink = [NSEntityDescription
                                  insertNewObjectForEntityForName:kRHServiceLinkEntityName 
                                  inManagedObjectContext:managedObjectContext];
        newLink.name = [link objectForKey:kNameKey];
        newLink.url = [link objectForKey:kURLKey];
        newLink.parent = category;
    }
}

- (void)deleteAllCampusServices:(NSManagedObjectContext *)managedObjectContext
{
    NSFetchRequest *fetchReqeust = [NSFetchRequest fetchRequestWithEntityName:kRHServiceItemEntityName];
    
    NSError *error = nil;
    NSArray *oldCampusServiceItems = [managedObjectContext executeFetchRequest:fetchReqeust
                                                                         error:&error];
    
    if (error) {
        NSLog(@"Problem finding old campus services to delete: %@", error);
        return;
    }
    
    for (RHServiceItem *serviceItem in oldCampusServiceItems) {
        [managedObjectContext deleteObject:serviceItem];
    }
}

@end
