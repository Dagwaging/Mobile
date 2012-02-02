//
//  RHNavigationNode.h
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

#import <Foundation/Foundation.h>
#import <CoreData/CoreData.h>

#import "RHNode.h"

#define kRHNavigatioNodeEntityName @"RHNavigationNode"


@class RHLocation;

/// \ingroup model
/// An RHNode subclass that is able to be navigated to. These are used as
/// points for users to travel to.
@interface RHNavigationNode : RHNode

/// Integer representing which floor this node is on.
@property (nonatomic, strong) NSNumber *floor;

/// Integer representing whether or not this floor is indoors.
@property (nonatomic, strong) NSNumber *indoors;

/// RHLocation that contains this RHNavigationNode.
@property (nonatomic, strong) RHLocation *enclosingLocation;

@end
