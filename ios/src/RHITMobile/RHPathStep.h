//
//  RHDirectionLineItem.h
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

#import <Foundation/Foundation.h>
#import <CoreLocation/CoreLocation.h>
#import <CoreData/CoreData.h>

@class RHLocation;


typedef enum {
    RHPathStepActionStraight,
    RHPathStepActionCrossStreet,
    RHPathStepActionFollowPath,
    RHPathStepActionSlightLeft,
    RHPathStepActionSlightRight,
    RHPathStepActionLeft,
    RHPathStepActionRight,
    RHPathStepActionSharpLeft,
    RHPathStepActionSharpRight,
    RHPathStepActionEnterBuilding,
    RHPathStepActionExitBuilding,
    RHPathStepActionUpStairs,
    RHPathStepActionDownStairs,
    RHPathStepActionNoAction
} RHPathStepAction;


@interface RHPathStep : NSObject

@property (nonatomic, assign) RHPathStepAction action;

@property (nonatomic, strong) NSString *detail;

@property (nonatomic, strong) NSNumber *altitude;

@property (nonatomic, assign) BOOL flagged;

@property (nonatomic, assign) BOOL outside;

@property (nonatomic, strong) NSManagedObjectID *locationID;

@property (nonatomic, assign) CLLocationCoordinate2D coordinate;

@end
