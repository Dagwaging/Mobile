//
//  RHDirectionsRequester.m
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

#import "RHDirectionsRequester.h"
#import "RHLocation.h"
#import "RHPathStep.h"
#import "RHWrappedCoordinate.h"
#import "RHWebRequestMaker.h"

#define kDirectionsPath @"/directions/fromloc/%d/toloc/%d"
#define kResultsKey @"Result"
#define kPathsKey @"Paths"
#define kDirectionsNameKey @"Dir"
#define kFlaggedKey @"Flag"
#define kCoordinateKey @"To"
#define kLatKey @"Lat"
#define kLngKey @"Lon"

@interface RHDirectionsRequester ()

@property (atomic, strong) NSManagedObjectID *startLocationID;

@property (atomic, strong) NSManagedObjectID *endLocationID;

- (void)requestDirectionsBetweenLocationsByID;

@end

@implementation RHDirectionsRequester

@synthesize startLocationID = startLocationID_;
@synthesize endLocationID = endLocationID_;

- (void)requestDirectionsFromLocation:(RHLocation *)startLocation
                           toLocation:(RHLocation *)endLocation {
    self.startLocationID = startLocation.objectID;
    self.endLocationID = endLocation.objectID;
    
    [self performSelectorInBackground:@selector(requestDirectionsByID) withObject:nil];
}

- (void)requestDirectionsBetweenLocationsByID {
    NSManagedObjectContext *localContext = [[NSManagedObjectContext alloc] init];
    localContext.persistentStoreCoordinator = self.persistantStoreCoordinator;
    
    RHLocation *startLocation = (RHLocation *)[localContext objectWithID:self.startLocationID];
    RHLocation *endLocation = (RHLocation *)[localContext objectWithID:self.endLocationID];
    
    NSString *requestPath = [NSString stringWithFormat:kDirectionsPath, startLocation.serverIdentifier.intValue, endLocation.serverIdentifier.intValue];
    NSDictionary *response = [RHWebRequestMaker JSONGetRequestWithPath:requestPath URLargs:@""];
    
    [self sendDelegatePathFromJSONResponse:response];
}

@end
