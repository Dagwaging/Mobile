//
//  RHTagsRequester.m
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

#import "RHTagsRequester.h"
#import "RHDataVersionManager.h"
#import "RHTourTag.h"
#import "RHTourTagCategory.h"
#import "RHWebRequestMaker.h"

#define kTagsPath @"/tours/tags"

#define kRootKey @"Root"
#define kChildrenKey @"Children"
#define kTagsKey @"Tags"
#define kNameKey @"Name"
#define kIsDefaultKey @"IsDefault"


@interface RHTagsRequester ()

- (void)createManagedObjectsFromCategories:(NSArray *)categories
                                inCategory:(RHTourTagCategory *)category
                    inManagedObjectContext:(NSManagedObjectContext *)managedObjectContext;

- (void)createManagedObjectsFromTags:(NSArray *)tags
                          inCategory:(RHTourTagCategory *)category
              inManagedObjectContext:(NSManagedObjectContext *)managedObjectContext;

@end


@implementation RHTagsRequester

@synthesize delegate = delegate_;
@synthesize persistantStoreCoordinator = persistantStoreCoordinator_;

- (id)initWithDelegate:(NSObject<RHTagsRequesterDelegate> *)delegate persistantStoreCoordinator:(NSPersistentStoreCoordinator *)persistantStoreCoordinator {
    self = [super init];
    
    if (self) {
        self.delegate = delegate;
        self.persistantStoreCoordinator = persistantStoreCoordinator;
    }
    
    return self;
}

- (void)updateTags {
    // Only run on a background thread
    if ([NSThread isMainThread]) {
        [self performSelectorInBackground:@selector(updateTags) withObject:nil];
        return;
    }
    
    // Check version before continuing
    RHDataVersionManager *dataVersionManager = [RHDataVersionManager instance];
    
    if (!dataVersionManager.needsTagsUpdate) {
        return;
    }
    
    NSManagedObjectContext *localContext = [[NSManagedObjectContext alloc] init];
    localContext.persistentStoreCoordinator = self.persistantStoreCoordinator;
    
    // Delete old tags and categories
    NSFetchRequest *oldRequest = [NSFetchRequest fetchRequestWithEntityName:kRHTourItemEntityName];
    NSArray *oldItems = [localContext executeFetchRequest:oldRequest error:nil];
    
    for (RHTourItem *tourTagItem in oldItems) {
        [localContext deleteObject:tourTagItem];
    }
    
    // Load new tags
    NSDictionary *response = [RHWebRequestMaker JSONGetRequestWithPath:kTagsPath URLargs:@""];
    NSDictionary *root = [response objectForKey:kRootKey];
    
    // Recursively save new tags and categories
    [self createManagedObjectsFromCategories:[NSArray arrayWithObject:root]
                                  inCategory:nil
                      inManagedObjectContext:localContext];
    
    // Commit changes
    NSError *saveError;
    [localContext save:&saveError];
    
    if (saveError) {
        NSLog(@"Problem saving new tags: %@", saveError);
        return;
    }
    
    // Notify delegate
    [self.delegate performSelectorOnMainThread:@selector(didFinishUpdatingTags)
                                    withObject:nil
                                 waitUntilDone:NO];
    
    // Update tags version
    [dataVersionManager upgradeTagsVersion];
}

#pragma mark - Private Methods

- (void)createManagedObjectsFromCategories:(NSArray *)categories
                                inCategory:(RHTourTagCategory *)category
                    inManagedObjectContext:(NSManagedObjectContext *)managedObjectContext {
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
              inManagedObjectContext:(NSManagedObjectContext *)managedObjectContext {
    for (NSDictionary *tagDict in tags) {
        RHTourTag *newTag = [NSEntityDescription insertNewObjectForEntityForName:kRHTourTagEntityName 
                                                          inManagedObjectContext:managedObjectContext];
        newTag.name = [tagDict objectForKey:kNameKey];
        newTag.isDefault = [tagDict objectForKey:kIsDefaultKey];
        newTag.parent = category;
    }
}

@end
