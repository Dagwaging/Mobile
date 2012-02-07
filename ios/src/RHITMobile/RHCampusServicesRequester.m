//
//  RHCampusServicesRequester.m
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

#import <CoreData/CoreData.h>

#import "RHCampusServicesRequester.h"
#import "RHDataVersionManager.h"
#import "RHServiceCategory.h"
#import "RHServiceLink.h"
#import "RHWebRequestMaker.h"

#define kCampusServicesPath @"/services"
#define kVersionKey @"Version"
#define kRootKey @"Root"
#define kChildrenKey @"Children"
#define kLinksKey @"Links"
#define kNameKey @"Name"
#define kURLKey @"Url"

@interface RHCampusServicesRequester ()

- (void)createManagedObjectsFromCategories:(NSArray *)categories
                            parentCategory:(RHServiceCategory *)parentCategory
                      managedObjectContext:(NSManagedObjectContext *)managedObjectContext;

- (void)createManagedObjectsFromLinks:(NSArray *)links
                             category:(RHServiceCategory *)category
                 managedObjectContext:(NSManagedObjectContext *)managedObjectContext;

@end

@implementation RHCampusServicesRequester

@synthesize persistantStoreCoordinator = persistantStoreCoordinator_;
@synthesize delegate = _delegate;

- (id)initWithPersistantStoreCoordinator:(NSPersistentStoreCoordinator *)persistantStoreCoordinator delegate:(NSObject<RHCampusServicesRequesterDelegate> *)delegate {
    
    self = [super init];
    
    if (self) {
        self.persistantStoreCoordinator = persistantStoreCoordinator;
        self.delegate = delegate;
    }
    
    return self;
}

- (void)updateCampusServices {
    // Only execute on a background thread
    if ([NSThread isMainThread]) {
        [self performSelectorInBackground:@selector(updateCampusServices) withObject:nil];
        return;
    }
    
    // Check version before continuing
    RHDataVersionManager *dataVersionManager = [RHDataVersionManager instance];
    
    if (!dataVersionManager.needsServicesUpdate) {
        return;
    }
    
    // Perform initial web request
    NSDictionary *response = [RHWebRequestMaker JSONGetRequestWithPath:kCampusServicesPath
                                                               URLargs:@""];
    
    // Load all old categories and links (to be deleted)
    NSManagedObjectContext *localContext = [[NSManagedObjectContext alloc] init];
    localContext.persistentStoreCoordinator = self.persistantStoreCoordinator;
    
    NSFetchRequest *categoryFetchRequest = [NSFetchRequest
                                            fetchRequestWithEntityName:kRHServiceCategoryEntityName];
    
    NSFetchRequest *linkFetchRequest = [NSFetchRequest
                                        fetchRequestWithEntityName:kRHServiceLinkEntityName];
    
    NSError *categoriesError;
    NSArray *oldCategories = [localContext executeFetchRequest:categoryFetchRequest
                                                         error:&categoriesError];
    
    if (categoriesError) {
        NSLog(@"Problem loading old categories: %@", categoriesError);
        return;
    }
    
    NSError *linksError;
    NSArray *oldLinks = [localContext executeFetchRequest:linkFetchRequest error:&linksError];
    
    if (linksError) {
        NSLog(@"Problem loading old links: %@", linksError);
        return;
    }
    
    // Delete all old categories and links
    for (RHServiceCategory *category in oldCategories) {
        [localContext deleteObject:category];
    }
    
    for (RHServiceLink *link in oldLinks) {
        [localContext deleteObject:link];
    }
    
    NSError *error;
    [localContext save:&error];
    
    if (error) {
        NSLog(@"Problem saving deletion of old campus services: %@", error);
        return;
    }
    
    // Retrieve new categories and links
    NSDictionary *root = [response objectForKey:kRootKey];
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

    
    // Notify the delegate that we've finished updating campus service entries
    [self.delegate performSelectorOnMainThread:@selector(didFinishUpdatingCampusServices)
                                    withObject:nil
                                 waitUntilDone:NO];
    
    // Update local version
    [dataVersionManager upgradeServicesVersion];
}

- (void)createManagedObjectsFromCategories:(NSArray *)categories
                            parentCategory:(RHServiceCategory *)parentCategory 
                      managedObjectContext:(NSManagedObjectContext *)managedObjectContext{
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
                 managedObjectContext:(NSManagedObjectContext *)managedObjectContext {
    for (NSDictionary *link in links) {
        RHServiceLink *newLink = [NSEntityDescription
                                  insertNewObjectForEntityForName:kRHServiceLinkEntityName 
                                  inManagedObjectContext:managedObjectContext];
        newLink.name = [link objectForKey:kNameKey];
        newLink.url = [link objectForKey:kURLKey];
        newLink.parent = category;
    }
}

@end
