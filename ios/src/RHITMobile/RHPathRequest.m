//
//  RHPathRequest.m
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

#import "RHPathRequest.h"
#import "RHAppDelegate.h"
#import "RHJSONRequest.h"
#import "RHLocation.h"
#import "RHPath.h"
#import "RHPathStep.h"


#define kOffCampusTourPath @"/tours/offcampus%@"
#define kOnCampusTourByGPSPath @"/tours/oncampus/fromgps/%f/%f%@"
#define kOnCampusTourByLocationPath @"/tours/oncampus/fromloc/%d%@"
#define kDirectionsByGPSPath @"/directions/fromgps/%f/%f/toloc/%d"
#define kDirectionsByLocationPath @"/directions/fromloc/%d/toloc/%d"

#define kWaitKey @"wait"
#define kLengthKey @"length"

#define kURLTrue @"true"
#define kURLFalse @"false"

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


@interface RHPathRequest ()

+ (NSString *)pathComponentFromTags:(NSArray *)tagIds;

+ (void)processJSONPathResponse:(NSDictionary *)response
                   successBlock:(void (^)(RHPath *))successBlock
                   failureBlock:(void (^)(NSError *))failureBlock;

+ (NSManagedObjectContext *)createThreadSafeManagedObjectContext;

@end


@implementation RHPathRequest

+ (void)makeOffCampusTourRequestWithTagIds:(NSArray *)tags
                              successBlock:(void (^)(RHPath *))successBlock
                              failureBlock:(void (^)(NSError *))failureBlock
{
    NSDictionary *urlArgs = [NSDictionary dictionaryWithObject:kURLTrue forKey:kWaitKey];
    
    NSString *fullPath = [NSString stringWithFormat:kOffCampusTourPath, [self pathComponentFromTags:tags]];
    
    [RHJSONRequest makeRequestWithPath:fullPath
                               urlArgs:urlArgs
                          successBlock:^(NSDictionary *jsonDict) {
                              
                              NSOperationQueue *queue = [[NSOperationQueue alloc] init];
                              [queue addOperationWithBlock:^(void) {
                                  [self processJSONPathResponse:jsonDict
                                                   successBlock:successBlock
                                                   failureBlock:failureBlock];
                              }];
                              
                              [queue waitUntilAllOperationsAreFinished];
                          } 
                          failureBlock:^(NSError *error) {
                              failureBlock(error);
                          }];
}

+ (void)makeOnCampusTourRequestWithTagIds:(NSArray *)tags
                       fromGPSCoordinages:(CLLocation *)location
                              forDuration:(NSNumber *)duration
                             successBlock:(void (^)(RHPath *))successBlock
                             failureBlock:(void (^)(NSError *))failureBlock
{
    NSMutableDictionary *urlArgs = [NSMutableDictionary dictionaryWithCapacity:2];
    [urlArgs setObject:kURLTrue forKey:kWaitKey];
    [urlArgs setObject:duration.description forKey:kLengthKey];
    
    NSLog(@"%f", location.coordinate.latitude);
    
    NSString *fullPath = [NSString stringWithFormat:kOnCampusTourByGPSPath, location.coordinate.latitude, location.coordinate.longitude, [self pathComponentFromTags:tags]];
    
    [RHJSONRequest makeRequestWithPath:fullPath urlArgs:urlArgs successBlock:^(NSDictionary *jsonDict) {
        
        NSOperationQueue *queue = [[NSOperationQueue alloc] init];
        
        [queue addOperationWithBlock:^(void) {
            [self processJSONPathResponse:jsonDict successBlock:successBlock failureBlock:failureBlock];
        }];
        
    } failureBlock:^(NSError *error) {
        failureBlock(error);
    }];
}

+ (void)makeOnCampusTourRequestWithTagIds:(NSArray *)tags
                       fromLocationWithId:(NSNumber *)startLocationServerId
                              forDuration:(NSNumber *)duration
                             successBlock:(void (^)(RHPath *))successBlock
                             failureBlock:(void (^)(NSError *))failureBlock
{
    NSMutableDictionary *urlArgs = [NSMutableDictionary dictionaryWithCapacity:2];
    [urlArgs setObject:kURLTrue forKey:kWaitKey];
    [urlArgs setObject:duration.description forKey:kLengthKey];
    
    NSString *fullPath = [NSString stringWithFormat:kOnCampusTourByLocationPath, startLocationServerId.intValue, [self pathComponentFromTags:tags]];
    
    [RHJSONRequest makeRequestWithPath:fullPath
                               urlArgs:urlArgs
                          successBlock:^(NSDictionary *jsonDict) {
                              
                              NSOperationQueue *queue = [[NSOperationQueue alloc] init];
                              [queue addOperationWithBlock:^(void) {
                                  [self processJSONPathResponse:jsonDict
                                                   successBlock:successBlock
                                                   failureBlock:failureBlock];
                              }];
                              
                              [queue waitUntilAllOperationsAreFinished];
                          }
                          failureBlock:^(NSError *error) {
                              failureBlock(error);
                          }];
}

+ (void)makeDirectionsRequestFromGPSCoordinates:(CLLocation *)location
                                     toLocation:(NSNumber *)endLocationServerId
                                   successBlock:(void (^)(RHPath *))successBlock
                                   failureBlock:(void (^)(NSError *))failureBlock
{
    NSDictionary *urlArgs = [NSDictionary dictionaryWithObject:kURLTrue forKey:kWaitKey];
    
    NSString *fullpath = [NSString stringWithFormat:kDirectionsByGPSPath, location.coordinate.latitude, location.coordinate.longitude, endLocationServerId];
    
    [RHJSONRequest makeRequestWithPath:fullpath urlArgs:urlArgs successBlock:^(NSDictionary *jsonResponse) {
        
        NSOperationQueue *queue = [[NSOperationQueue alloc] init];
        [queue addOperationWithBlock:^(void) {
            [self processJSONPathResponse:jsonResponse successBlock:successBlock failureBlock:failureBlock];
        }];
        
        [queue waitUntilAllOperationsAreFinished];
        
    } failureBlock:^(NSError *error) {
        failureBlock(error);
    }];
}

+ (void)makeDirectionsRequestFromLocationWithId:(NSNumber *)startLocationServerId
                                     toLocation:(NSNumber *)endLocationServerId
                                   successBlock:(void (^)(RHPath *))successBlock
                                   failureBlock:(void (^)(NSError *))failureBlock
{
    NSDictionary *urlArgs = [NSDictionary dictionaryWithObject:kURLTrue forKey:kWaitKey];
    
    NSString *fullPath = [NSString stringWithFormat:kDirectionsByLocationPath, startLocationServerId.intValue, endLocationServerId.intValue];
    
    [RHJSONRequest makeRequestWithPath:fullPath
                               urlArgs:urlArgs
                          successBlock:^(NSDictionary *jsonDict) {
                              
                              NSOperationQueue *queue = [[NSOperationQueue alloc] init];
                              [queue addOperationWithBlock:^(void) {
                                  [self processJSONPathResponse:jsonDict
                                                   successBlock:successBlock
                                                   failureBlock:failureBlock];
                              }];
                              
                              [queue waitUntilAllOperationsAreFinished];
                              
                          } failureBlock:^(NSError *error) {
                              failureBlock(error);
                          }];
}

#pragma mark - Private Methods

+ (NSString *)pathComponentFromTags:(NSArray *)tagIds
{
    NSString *result = @"";
    
    for (NSNumber *tagId in tagIds) {
        result = [result stringByAppendingFormat:@"/%d", tagId.intValue];
    }
    
    return result;
}

+ (void)processJSONPathResponse:(NSDictionary *)response
                   successBlock:(void (^)(RHPath *))successBlock
                   failureBlock:(void (^)(NSError *))failureBlock
{
    // Check the "done" level to make sure the request completed
    NSNumber *doneLevel = [response objectForKey:kDoneKey];
    
    if (doneLevel.intValue < 100) {
        NSLog(@"Error in directions/tour request: Request not done");
        
        [[NSOperationQueue mainQueue] addOperationWithBlock:^(void) {
            failureBlock([NSError errorWithDomain:@"" code:0 userInfo:nil]);
        }];
        
        return;
    }
    
    // Pull off path response
    NSDictionary *result = [response objectForKey:kResultKey];
    
    // Create new model object to hold our data in
    RHPath *path = [[RHPath alloc] init];
    
    // Set properties on the new path itself
    path.distance = [result objectForKey:kDistanceKey];
    path.stairsUp = [result objectForKey:kStairsUpKey];
    path.stairsDown = [result objectForKey:kStairsDownKey];
    
    // Pull off steps array
    NSArray *steps = [result objectForKey:kPathsKey];
    
    // Create a steps array to hold our model result in
    NSMutableArray *newSteps = [NSMutableArray arrayWithCapacity:steps.count];
    
    for (NSDictionary *step in steps) {
        
        // Create a new step
        RHPathStep *newStep = [[RHPathStep alloc] init];
        
        // Set the action on the new step
        NSString *action = [step objectForKey:kActionKey];
        if ((id) action == [NSNull null]) {
            newStep.action = RHPathStepActionNoAction;
        } else if ([action isEqualToString:kDirectionGoStraightKey]) {
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
        }
        
        // Set the remaining properties on this step
        newStep.altitude = [step objectForKey:kAltitudeKey];
        newStep.detail = [step objectForKey:kDetailKey];
        newStep.flagged = [[step objectForKey:kFlaggedKey] boolValue];
        newStep.coordinate = CLLocationCoordinate2DMake([[step objectForKey:kLatKey] floatValue], [[step objectForKey:kLongKey] floatValue]);
        newStep.outside = [[step objectForKey:kOutsideKey] boolValue];
        
        // Pull off server id for a location (if there is one)
        NSNumber *locationID = [step objectForKey:kLocationIDKey];
        
        if ((id)locationID != [NSNull null]) {
            NSManagedObjectContext *localContext = [self createThreadSafeManagedObjectContext];
            
            NSFetchRequest *locationFetchRequest = [NSFetchRequest fetchRequestWithEntityName:kRHLocationEntityName];
            locationFetchRequest.predicate = [NSPredicate predicateWithFormat:@"serverIdentifier == %d", locationID.intValue];
            
            NSArray *results = [localContext executeFetchRequest:locationFetchRequest error:nil];
            
            if (results.count > 0) {
                newStep.locationID = [[results objectAtIndex:0] objectID];
            } else {
                NSLog(@"Error: Directions referenced invalid location ID: %d", locationID.intValue);
                
                [[NSOperationQueue mainQueue] addOperationWithBlock:^(void) {
                    failureBlock([NSError errorWithDomain:@"" code:0 userInfo:nil]);
                }];
                
                return;
            }
        }
        
        [newSteps addObject:newStep];
    }
    
    // Add the steps array to our path
    path.steps = newSteps;
    
    // Call the success callback on the main thread to finish up
    [[NSOperationQueue mainQueue] addOperationWithBlock:^(void) {
        successBlock(path);
    }];
}

+ (NSManagedObjectContext *)createThreadSafeManagedObjectContext
{
    NSManagedObjectContext *localContext = [[NSManagedObjectContext alloc] init];
    localContext.persistentStoreCoordinator = [(RHAppDelegate *) [[UIApplication sharedApplication] delegate] persistentStoreCoordinator];
    return localContext;
}

@end
