//
//  RHTourTagsLoader.m
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

#import "RHTourTagsLoader.h"
#import "RHAppDelegate.h"
#import "RHDataVersionsLoader.h"
#import "RHLoaderRequestsWrapper.h"
#import "RHTourTag.h"
#import "RHTourTagCategory.h"

#define kVersionKey @"Version"

#define kRootKey @"TagsRoot"
#define kChildrenKey @"Children"
#define kTagsKey @"Tags"
#define kServerIDKey @"Id"
#define kNameKey @"Name"
#define kIsDefaultKey @"IsDefault"


@interface RHTourTagsLoader () {
@private
    BOOL _currentlyUpdating;
}

- (NSManagedObjectContext *)createThreadSafeManagedObjectContext;

- (void)deleteAllTags:(NSManagedObjectContext *)managedObjectContext;

- (void)createManagedObjectsFromCategories:(NSArray *)categories
                                inCategory:(RHTourTagCategory *)category
                    inManagedObjectContext:(NSManagedObjectContext *)managedObjectContext;

- (void)createManagedObjectsFromTags:(NSArray *)tags
                          inCategory:(RHTourTagCategory *)category
              inManagedObjectContext:(NSManagedObjectContext *)managedObjectContext;

@end


@implementation RHTourTagsLoader

static RHTourTagsLoader *_instance;

+ (void)initialize
{
    static BOOL initialized = NO;
    if(!initialized)
    {
        initialized = YES;
        _instance = [[RHTourTagsLoader alloc] init];
    }
}

- (id)init
{
    if (self = [super init]) {
        _currentlyUpdating = NO;
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

- (void)updateTourTags:(NSNumber *)version
{
    if (_currentlyUpdating) {
        return;
    }
    
    // Only operate on a background thread
    if ([NSThread isMainThread]) {
        [self performSelectorInBackground:@selector(updateTourTags:) withObject:version];
        return;
    }
    
#ifdef RHITMobile_RHLoaderDebug
    NSLog(@"Tour tags update started");
#endif
    
    _currentlyUpdating = YES;
    
    NSError *currentError = nil;
    
    NSDictionary *jsonResponse = [RHLoaderRequestsWrapper makeSynchronousTourTagsRequestWithWithVersion:version error:&currentError];
    
    if (currentError) {
        NSLog(@"Error updating tour tags: %@", currentError);
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
    NSLog(@"Deleting old tour tags");
#endif
    
    // Delete old tags
    [self deleteAllTags:localContext];
    
#ifdef RHITMobile_RHLoaderDebug
    NSLog(@"Saving new tour tags");
#endif
    
    // Save new tags
    NSDictionary *root = [jsonResponse objectForKey:kRootKey];
    
    // Recursively save new tags and categories
    [self createManagedObjectsFromCategories:[NSArray arrayWithObject:root]
                                  inCategory:nil
                      inManagedObjectContext:localContext];
    
    // Commit changes
    NSError *saveError;
    [localContext save:&saveError];
    
#ifdef RHITMobile_RHLoaderDebug
    NSLog(@"Done saving tour tags");
#endif
    
    // TODO: Notify
    
    [RHDataVersionsLoader.instance setTourTagsVersion:newVersion];
    
    _currentlyUpdating = NO;
}

- (void)registerCallbackForTourTags:(void (^)(void))callback
{
    // TODO
}

- (NSManagedObjectContext *)createThreadSafeManagedObjectContext
{
    NSManagedObjectContext *localContext = [[NSManagedObjectContext alloc] init];
    localContext.persistentStoreCoordinator = [(RHAppDelegate *) [[UIApplication sharedApplication] delegate] persistentStoreCoordinator];
    return localContext;
}

- (void)deleteAllTags:(NSManagedObjectContext *)managedObjectContext
{
    NSFetchRequest *fetchRequest = [NSFetchRequest fetchRequestWithEntityName:kRHTourItemEntityName];
    
    NSError *error = nil;
    NSArray *oldTags = [managedObjectContext executeFetchRequest:fetchRequest error:&error];
    
    if (error) {
        NSLog(@"Error deleting old tour tags: %@", error);
        return;
    }
    
    for (RHTourItem *tourItem in oldTags) {
        [managedObjectContext deleteObject:tourItem];
    }
}

- (void)createManagedObjectsFromCategories:(NSArray *)categories
                                inCategory:(RHTourTagCategory *)category
                    inManagedObjectContext:(NSManagedObjectContext *)managedObjectContext
{
    for (NSDictionary *categoryDict in categories) {
        RHTourTagCategory *newCategory = [NSEntityDescription insertNewObjectForEntityForName:kRHTourTagCategoryEntityName inManagedObjectContext:managedObjectContext];
        
        newCategory.name = [categoryDict objectForKey:kNameKey];
        newCategory.parent = category;
        
        [self createManagedObjectsFromTags:[categoryDict objectForKey:kTagsKey]
                                inCategory:newCategory
                    inManagedObjectContext:managedObjectContext];
        [self createManagedObjectsFromCategories:[categoryDict objectForKey:kChildrenKey]
                                      inCategory:newCategory
                          inManagedObjectContext:managedObjectContext];
    }
}

- (void)createManagedObjectsFromTags:(NSArray *)tags
                          inCategory:(RHTourTagCategory *)category
              inManagedObjectContext:(NSManagedObjectContext *)managedObjectContext
{
    for (NSDictionary *tagDict in tags) {
        RHTourTag *newTag = [NSEntityDescription insertNewObjectForEntityForName:kRHTourTagEntityName 
                                                          inManagedObjectContext:managedObjectContext];
        newTag.name = [tagDict objectForKey:kNameKey];
        newTag.isDefault = [tagDict objectForKey:kIsDefaultKey];
        newTag.serverIdentifier = [tagDict objectForKey:kServerIDKey];
        newTag.parent = category;
    }
}

@end
