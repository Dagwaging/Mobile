//
//  RHCampusServicesRequester.m
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

#import "RHCampusServicesRequester.h"

#import <CoreData/CoreData.h>
#import "RHServiceCategory.h"
#import "RHServiceLink.h"

@implementation RHCampusServicesRequester

@synthesize persistantStoreCoordinator = persistantStoreCoordinator_;

- (id)initWithPersistantStoreCoordinator:(NSPersistentStoreCoordinator *)persistantStoreCoordinator {
    
    self = [super init];
    
    if (self) {
        self.persistantStoreCoordinator = persistantStoreCoordinator;
    }
    
    return self;
}

- (void)updateCampusServices {
    if ([NSThread isMainThread]) {
        [self performSelectorInBackground:@selector(updateCampusServices) withObject:nil];
        return;
    }

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
    }
    
    NSError *linksError;
    NSArray *oldLinks = [localContext executeFetchRequest:linkFetchRequest error:&linksError];
    
    if (linksError) {
        NSLog(@"Problem loading old links: %@", linksError);
    }
    
    RHServiceCategory *studentServices = [NSEntityDescription
                                          insertNewObjectForEntityForName:kRHServiceCategoryEntityName
                                          inManagedObjectContext:localContext];
    studentServices.name = @"Student Services";
    
    RHServiceCategory *diningServices = [NSEntityDescription
                                         insertNewObjectForEntityForName:kRHServiceCategoryEntityName 
                                         inManagedObjectContext:localContext];
    diningServices.name = @"Dining Services";
    
    RHServiceLink *courseCatalog = [NSEntityDescription
                                    insertNewObjectForEntityForName:kRHServiceLinkEntityName 
                                    inManagedObjectContext:localContext];
    courseCatalog.name = @"Course Catalog";
    courseCatalog.url = @"http://www.rose-hulman.edu/offices-services/registrar/course-catalog.aspx";
    courseCatalog.parent = studentServices;
    
    for (RHServiceCategory *category in oldCategories) {
        [localContext deleteObject:category];
    }
    
    for (RHServiceLink *link in oldLinks) {
        [localContext deleteObject:link];
    }
    
    NSError *error;
    [localContext save:&error];
    
    if (error) {
        NSLog(@"Problem saving campus services: %@", error);
    }
}

@end
