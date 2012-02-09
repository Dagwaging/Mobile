//
//  RHPathRequester.m
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

#import "RHPathRequester.h"
#import "RHLocation.h"
#import "RHPath.h"
#import "RHPathStep.h"
#import "RHWebRequestMaker.h"

#define kDoneKey @"Done"
#define kRequestIDKey @"RequestId"
#define kDistanceKey @"Dist"
#define kResultKey @"Result"
#define kPathsKey @"Paths"
#define kStairsDownKey @"StairsDown"
#define kStairsUpKey @"StairsUp"
#define kActionKey @"Action"
#define kAltitudeKey @"Altitude"
#define kDetailKey @"Dir"
#define kFlaggedKey @"Flag"
#define kLatKey @"Lat"
#define kLocationIDKey @"Location"
#define kLongKey @"Lon"
#define kOutsideKey @"Outside"
#define kDirectionGoStraightKey @"GS"
#define kDirectionCrossStreetKey @"CS"
#define kDirectionFollowPathKey @"FP"
#define kDirectionSlightLeftKey @"L1"
#define kDirectionSlightRightKey @"R1"
#define kDirectionLeftKey @"L2"
#define kDirectionRightKey @"R2"
#define kDirectionSharpLeftKey @"L3"
#define kDirectionSharpRightKey @"R3"
#define kDirectionEnterBuildingKey @"EN"
#define kDirectionExitBuildingKey @"EX"
#define kDirectionUpStairsKey @"US"
#define kDirectionDownStairsKey @"DS"


@interface RHPathRequester()

- (void)checkRequestStatus;

@end


@implementation RHPathRequester

@synthesize delegate = _delegate;
@synthesize persistantStoreCoordinator = _persistantStoreCoordinator;
@synthesize requestID = requestID_;

- (id)initWithDelegate:(NSObject<RHPathRequesterDelegate> *)delegate
persistantStoreCoordinator:(NSPersistentStoreCoordinator *)persistantStoreCoordinator {
    self = [super init];
    
    if (self) {
        self.delegate = delegate;
        self.persistantStoreCoordinator = persistantStoreCoordinator;
    }
    
    return self;
}

- (void)sendDelegatePathFromJSONResponse:(NSDictionary *)response {
    NSNumber *done = [response objectForKey:kDoneKey];
    
    if (done.intValue < 100) {
        self.requestID = [response objectForKey:kRequestIDKey];
        [self performSelector:@selector(checkRequestStatus)
                   withObject:nil
                   afterDelay:5];
        return;
    }
    
    NSDictionary *result = [response objectForKey:kResultKey];
    
    RHPath *path = [[RHPath alloc] init];
    
    path.distance = [result objectForKey:kDistanceKey];
    path.stairsUp = [result objectForKey:kStairsUpKey];
    path.stairsDown = [result objectForKey:kStairsDownKey];
    
    NSArray *steps = [result objectForKey:kPathsKey];
    
    NSMutableArray *newSteps = [NSMutableArray arrayWithCapacity:steps.count];
    
    for (NSDictionary *step in steps) {
        RHPathStep *newStep = [[RHPathStep alloc] init];
        
        NSString *action = [step objectForKey:kActionKey];
        
        if ([action isEqualToString:kDirectionGoStraightKey]) {
            newStep.action = RHPathStepActionStraight;
        } else if ([action isEqualToString:kDirectionCrossStreetKey]) {
            newStep.action = RHPathStepActionCrossStreet;
        } else if ([action isEqualToString:kDirectionFollowPathKey]) {
            newStep.action = RHPathStepActionFollowPath;
        } else if ([action isEqualToString:kDirectionSlightLeftKey]) {
            newStep.action = RHPathStepActionSlightLeft;
        } else if ([action isEqualToString:kDirectionSlightRightKey]) {
            newStep.action = RHPathStepActionSlightRight;
        } else if ([action isEqualToString:kDirectionLeftKey]) {
            newStep.action = RHPathStepActionLeft;
        } else if ([action isEqualToString:kDirectionRightKey]) {
            newStep.action = RHPathStepActionRight;
        } else if ([action isEqualToString:kDirectionSharpLeftKey]) {
            newStep.action = RHPathStepActionSharpLeft;
        } else if ([action isEqualToString:kDirectionSharpRightKey]) {
            newStep.action = RHPathStepActionRight;
        } else if ([action isEqualToString:kDirectionEnterBuildingKey]) {
            newStep.action = RHPathStepActionEnterBuilding;
        } else if ([action isEqualToString:kDirectionExitBuildingKey]) {
            newStep.action = RHPathStepActionExitBuilding;
        } else if ([action isEqualToString:kDirectionUpStairsKey]) {
            newStep.action = RHPathStepActionUpStairs;
        } else if ([action isEqualToString:kDirectionDownStairsKey]) {
            newStep.action = RHPathStepActionDownStairs;
        } else {
            newStep.action = RHPathStepActionNoAction;
        }
        
        newStep.altitude = [step objectForKey:kAltitudeKey];
        newStep.detail = [step objectForKey:kDetailKey];
        newStep.flagged = [[step objectForKey:kFlaggedKey] boolValue];
        newStep.coordinate = CLLocationCoordinate2DMake([[step objectForKey:kLatKey] floatValue], [[step objectForKey:kLongKey] floatValue]);
        newStep.outside = [[step objectForKey:kOutsideKey] boolValue];
        
        NSNumber *locationID = [step objectForKey:kLocationIDKey];
        
        if ((id)locationID != [NSNull null]) {
            NSManagedObjectContext *localContext = [[NSManagedObjectContext alloc] init];
            localContext.persistentStoreCoordinator = self.persistantStoreCoordinator;
            
            NSFetchRequest *locationFetchRequest = [NSFetchRequest fetchRequestWithEntityName:kRHLocationEntityName];
            locationFetchRequest.predicate = [NSPredicate predicateWithFormat:@"serverIdentifier == %d", locationID.intValue];
            
            NSArray *results = [localContext executeFetchRequest:locationFetchRequest error:nil];
            
            if (results.count > 0) {
                newStep.locationID = [[results objectAtIndex:0] objectID];
            }
        }
        
        [newSteps addObject:newStep];
    }

    path.steps = newSteps;
    
    [self.delegate performSelectorOnMainThread:@selector(didLoadPath:)
                                    withObject:path
                                 waitUntilDone:NO];
}
 
- (void)checkRequestStatus {
    NSString *statusPath = [NSString stringWithFormat:kStatusPath, self.requestID.intValue];
    NSDictionary *response = [RHWebRequestMaker JSONGetRequestWithPath:statusPath URLargs:@""];
    
    [self sendDelegatePathFromJSONResponse:response];
}

@end
