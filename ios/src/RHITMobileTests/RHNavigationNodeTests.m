//
//  RHNavigationNodeTests.m
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

#import "RHNavigationNodeTests.h"
#import "RHNavigationNode.h"
#import "RHITMobileAppDelegate.h"


#define ARC4RANDOM_MAX 0x100000000

@implementation RHNavigationNodeTests

- (void)testInitSmokeTest {
    // Retrieve the App Delegate and Managed Object Context
    RHITMobileAppDelegate *appDelegate;
    appDelegate = (RHITMobileAppDelegate *)[[UIApplication
                                             sharedApplication] delegate];
    NSManagedObjectContext *context = appDelegate.managedObjectContext;
    
    // Create a new node
    RHNavigationNode *node = (RHNavigationNode *) [RHNavigationNode fromContext:context];
    
    // Just make sure it exists
    STAssertNotNil(node, @"New RHNavigationNode is nil");
}

- (void)testStorageAndRetrieval {
    // Decide on a random latitude
    double latValue = ((double)arc4random() / ARC4RANDOM_MAX) * 100.0;
    NSNumber *latitude = [NSNumber numberWithDouble:latValue];
    
    // Retrieve the App Delegate and Managed Object Context
    RHITMobileAppDelegate *appDelegate;
    appDelegate = (RHITMobileAppDelegate *)[[UIApplication
                                             sharedApplication] delegate];
    NSManagedObjectContext *context = appDelegate.managedObjectContext;
    
    // Create and "store" a new node with our random latitude
    RHNavigationNode *node = (RHNavigationNode *) [RHNavigationNode
                                                   fromContext:context];
    node.latitude = latitude;
    node.longitude = [NSNumber numberWithDouble:0.0];
    
    // Describe the type of entity we'd like to retrieve
    NSEntityDescription *entityDescription = [NSEntityDescription
                                              entityForName:@"NavigationNode"
                                              inManagedObjectContext:context];
    NSFetchRequest *request = [[[NSFetchRequest alloc] init] autorelease];
    [request setEntity:entityDescription];
    
    // Put conditions on our fetch request
    NSPredicate *predicate = [NSPredicate predicateWithFormat:
                              @"latitude == %@", latitude];
    [request setPredicate:predicate];
    
    // Retrieve what we hope is our created object
    NSError *error = nil;
    NSArray *results = [context executeFetchRequest:request error:&error];
    
    STAssertNotNil(results, @"Results array is nil");
    STAssertEquals(results.count, (NSUInteger) 1,
                   @"Results array has wrong number length");
    
    if (results.count > 0) {
        RHNavigationNode *storedNode;
        storedNode = (RHNavigationNode *) [results objectAtIndex:0];
        
        // Verify the properties set on our retrieved object
        STAssertEquals(storedNode.latitude, latitude, @"Latitude is incorrect");
        STAssertEquals(storedNode.longitude.doubleValue, 0.0,
                       @"Longitude is incorrect");
    }
}

@end