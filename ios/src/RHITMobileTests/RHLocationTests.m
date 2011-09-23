//
//  RHLocationTests.m
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

#import <CoreLocation/CoreLocation.h>

#import "RHLocationTests.h"
#import "RHLocation.h"
#import "RHITMobileAppDelegate.h"


@implementation RHLocationTests

- (void)testInitSmokeTest {
    // Retrieve the App Delegate and Managed Object Context
    RHITMobileAppDelegate *appDelegate;
    appDelegate = (RHITMobileAppDelegate *)[[UIApplication
                                             sharedApplication] delegate];
    NSManagedObjectContext *context = appDelegate.managedObjectContext;
    
    // Create a new location
    RHLocation *node = [[RHLocation alloc] initWithContext:context];
    
    // Just make sure it exists
    STAssertNotNil(node, @"New RHLocation is nil");
}

- (void)testStorageAndRetrieval {
    // User the current time as a name
    NSDate *now = [NSDate date];
	NSDateFormatter *formatter = nil;
	formatter = [[NSDateFormatter alloc] init];
	[formatter setTimeStyle:NSDateFormatterShortStyle];
    NSString *name = [formatter stringFromDate:now];
	[formatter release];
    
    // Retrieve the App Delegate and Managed Object Context
    RHITMobileAppDelegate *appDelegate;
    appDelegate = (RHITMobileAppDelegate *)[[UIApplication
                                             sharedApplication] delegate];
    NSManagedObjectContext *context = appDelegate.managedObjectContext;
    
    // Create and "store" a new loaction with our specific name
    RHLocation *location = [[RHLocation alloc] initWithContext:context];
    location.name = name;
    
    // Describe the type of entity we'd like to retrieve
    NSEntityDescription *entityDescription = [NSEntityDescription
                                              entityForName:@"Location"
                                              inManagedObjectContext:context];
    NSFetchRequest *request = [[[NSFetchRequest alloc] init] autorelease];
    [request setEntity:entityDescription];
    
    // Put conditions on our fetch request
    NSPredicate *predicate = [NSPredicate predicateWithFormat:
                              @"name == %@", name];
    [request setPredicate:predicate];
    
    // Retrieve what we hope is our created object
    NSError *error = nil;
    NSArray *results = [context executeFetchRequest:request error:&error];
    
    STAssertNotNil(results, @"Results array is nil");
    STAssertEquals(results.count, (NSUInteger) 1,
                   @"Results array has wrong number length");
    
    if (results.count > 0) {
        RHLocation *storedLocation;
        storedLocation = (RHLocation *) [results objectAtIndex:0];
        
        // Verify the properties set on our retrieved object
        STAssertEquals(location.name, name, @"Name is incorrect.");
    }
}

@end