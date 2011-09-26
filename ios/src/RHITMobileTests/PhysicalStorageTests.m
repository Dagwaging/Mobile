//
//  PhysicalStorageTests.m
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

#import "PhysicalStorageTests.h"
#import "RHITMobileAppDelegate.h"
#import "RHNode.h"


#define ARC4RANDOM_MAX 0x100000000

@implementation PhysicalStorageTests

- (void)testSimpleStorageAndRetrieval {
    double latValue = ((double)arc4random() / ARC4RANDOM_MAX) * 100.0;
    NSNumber *latitude = [NSNumber numberWithDouble:latValue];
    
    // Retrieve the App Delegate and Managed Object Context
    RHITMobileAppDelegate *appDelegate;
    appDelegate = (RHITMobileAppDelegate *)[[UIApplication
                                             sharedApplication] delegate];
    NSManagedObjectContext *context = appDelegate.managedObjectContext;
    
    RHNode *node = [RHNode fromContext:context];
    node.latitude = latitude;
    node.longitude = [NSNumber numberWithDouble:6.0];
    
    NSError *error = nil;
    
    // Actually save the object to disk, making sure it doesn't break
    STAssertTrue([context save:&error], @"Save operation failed");
    
    // Describe the type of entity we'd like to retrieve
    NSEntityDescription *entityDescription = [NSEntityDescription
                                              entityForName:@"Node"
                                              inManagedObjectContext:context];
    NSFetchRequest *request = [[[NSFetchRequest alloc] init] autorelease];
    [request setEntity:entityDescription];
    
    // Put conditions on our fetch request
    NSPredicate *predicate = [NSPredicate predicateWithFormat:
                              @"latitude == %@", latitude];
    [request setPredicate:predicate];
    
    // Retrieve what we hope is our created object
    error = nil;
    NSArray *results = [context executeFetchRequest:request error:&error];
    
    STAssertNotNil(results, @"Results array is nil");
    STAssertEquals(results.count, (NSUInteger) 1,
                   @"Results array has incorrect length");
    
    if (results.count > 0) {
        RHNode *storedNode = (RHNode *) [results objectAtIndex:0];
        
        // Verify the properties set on our retrieved object
        STAssertEquals(storedNode.latitude.doubleValue, latValue,
                       @"Latitude is incorrect");
        STAssertEquals(storedNode.longitude.doubleValue, 6.0,
                       @"Longitude is incorrect");
        
        // Clean up
        error = nil;
        [context deleteObject:storedNode];
        STAssertTrue([context save:&error], @"Cleanup failed");
    }
}

@end