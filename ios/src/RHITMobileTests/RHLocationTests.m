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
#import "RHBoundaryNode.h"
#import "RHITMobileAppDelegate.h"


@implementation RHLocationTests

- (void)testInitSmokeTest {
    // Retrieve the App Delegate and Managed Object Context
    RHITMobileAppDelegate *appDelegate;
    appDelegate = (RHITMobileAppDelegate *)[[UIApplication
                                             sharedApplication] delegate];
    NSManagedObjectContext *context = appDelegate.managedObjectContext;
    
    // Create a new location
    RHLocation *location = [RHLocation fromContext:context];
    
    // Just make sure it exists
    STAssertNotNil(location, @"New RHLocation is nil");
}

- (void)testBoundaryNodeOrdering {
    // Retrieve the App Delegate and Managed Object Context
    RHITMobileAppDelegate *appDelegate;
    appDelegate = (RHITMobileAppDelegate *)[[UIApplication
                                             sharedApplication] delegate];
    NSManagedObjectContext *context = appDelegate.managedObjectContext;
    
    // Create our candidate boundary nodes
    RHBoundaryNode *node1 = (RHBoundaryNode *) [RHBoundaryNode
                                                fromContext:context];
    node1.latitude = [NSNumber numberWithInt:1];
    
    RHBoundaryNode *node2 = (RHBoundaryNode *) [RHBoundaryNode
                                                fromContext:context];
    node2.latitude = [NSNumber numberWithInt:2];
    
    RHBoundaryNode *node3 = (RHBoundaryNode *) [RHBoundaryNode
                                                fromContext:context];
    node3.latitude = [NSNumber numberWithInt:3];
    
    // Aggregate them into an array
    NSArray *nodes = [[[NSArray alloc] initWithObjects:node2, node1, node3, nil]
                      autorelease];
    
    RHLocation *location = [RHLocation fromContext:context];
    location.orderedBoundaryNodes = nodes;
    
    NSArray *retrievedNodes = location.orderedBoundaryNodes;
    
    RHBoundaryNode *retrievedNode1 = [retrievedNodes objectAtIndex:1];
    RHBoundaryNode *retrievedNode2 = [retrievedNodes objectAtIndex:0];
    RHBoundaryNode *retrievedNode3 = [retrievedNodes objectAtIndex:2];
    
    STAssertEquals(retrievedNode1.latitude.intValue, 1, 
                   @"Incorrect first node.");
    STAssertEquals(retrievedNode2.latitude.intValue, 2, 
                   @"Incorrect second node.");
    STAssertEquals(retrievedNode3.latitude.intValue, 3,
                   @"Incorrect third node.");
}

- (void)testStorageAndRetrieval {
    // Use the current time as a name
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
    RHLocation *location = [RHLocation fromContext:context];
    location.name = name;
    location.serverIdentifier = [NSNumber numberWithInt:2];
    location.visibleZoomLevel = [NSNumber numberWithInt:15];
    location.quickDescription = @"Quick Description";
    
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
        STAssertEquals([location name], name, @"Name is incorrect.");
        STAssertEquals([[location serverIdentifier] intValue], 2,
                       @"Server ID is incorrect.");
        STAssertEquals([[location visibleZoomLevel] intValue], 15,
                       @"Visible zoom level is incorrect.");
        STAssertEquals(location.quickDescription, @"Quick Description", 
                    @"Description is incorrect.");
    }
}

- (void)testCompotedValues {
    // Retrieve the App Delegate and Managed Object Context
    RHITMobileAppDelegate *appDelegate;
    appDelegate = (RHITMobileAppDelegate *)[[UIApplication
                                             sharedApplication] delegate];
    NSManagedObjectContext *context = appDelegate.managedObjectContext;
    
    RHLocation *location = [RHLocation fromContext:context];
    location.displayType = RHLocationDisplayTypeNone;
    
    STAssertEquals(location.displayType, RHLocationDisplayTypeNone,
                   @"Display type set improperly");
    
    location.displayType = RHLocationDisplayTypeQuickList;
    
    STAssertEquals(location.displayType, RHLocationDisplayTypeQuickList,
                   @"Display type set improperly");
}

- (void)tearDown {
    // Retrieve the App Delegate and Managed Object Context
    RHITMobileAppDelegate *appDelegate;
    appDelegate = (RHITMobileAppDelegate *)[[UIApplication
                                             sharedApplication] delegate];
    NSManagedObjectContext *context = appDelegate.managedObjectContext;
    
    // Forget about all those changes
    [context rollback];
}

@end