//
//  RHBoundaryNode.h
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

#define kRHBoundaryNodeEntityName @"RHBoundaryNode"


@class RHLocation;

/// \ingroup model
/// RHNode specifically used for defining the boundary of a location.
/// RHBoundaryNodes will be represented in an ordered manner, and thus contain
/// order information.
@interface RHBoundaryNode : RHNode

/// Position in the unordered set that Core Data provides. Used to enforce
/// order when connecting boundary nodes.
@property (nonatomic, strong) NSNumber *position;

/// The RHLocation that this RHBoundaryNode borders.
@property (nonatomic, strong) RHLocation *boundedLocation;

@end
