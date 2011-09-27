//
//  RHLocation.h
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


@class RHBoundaryNode, RHLabelNode;

/// \ingroup model
/// Representation of a canonical location. An RHLocation has areas that can
/// be travelled to, borders defining where it physically is, labelling
/// properties, and a human-readable name.
@interface RHLocation : NSManagedObject

/// Server-generated integer ID for this RHLocation
@property (nonatomic, retain) NSNumber *serverIdentifer;

/// Human-readable name for this RHLocation.
@property (nonatomic, retain) NSString *name;

/// The minimum canonical map zoom level this RHLocation shouldbe visible at.
@property (nonatomic, retain) NSNumber *visibleZoomLevel;

/// The list of RHBoundaryNode objects that define the boundary of this
/// RHLocation. The RHBoundaryNode objects contain ordering information.
@property (nonatomic, retain) NSSet *boundaryNodes;

/// The location at which the label for this RHLocation should appear.
@property (nonatomic, retain) RHLabelNode *labelLocation;

/// The navigable RHNavigationNode objects that are enclosed by this RHLocation.
@property (nonatomic, retain) NSSet *navigationNodes;

/// Init from Core Data managed object context.
+ (RHLocation *)fromContext:(NSManagedObjectContext *)context;

@end

@interface RHLocation (CoreDataGeneratedAccessors)

/// Add an RHBoundaryNode to the current boundary.
- (void)addBoundaryNodesObject:(RHBoundaryNode *)boundaryNode;

/// Remove an RHBoundaryNode from the current boundary.
- (void)removeBoundaryNodesObject:(RHBoundaryNode *)boundaryNode;

/// Add an NSSet of RHBoundaryNode objects to the current boundary.
- (void)addBoundaryNodes:(NSSet *)boundaryNodes;

/// Remove an NSSet of RHBoundaryNode objects from the current boundary.
- (void)removeBoundaryNodes:(NSSet *)boundaryNodes;


/// Add an RHNavigationNode to this RHLocation.
- (void)addNavigationNodesObject:(NSManagedObject *)navigationNode;

/// Remove an RHNavigationNode from this location.
- (void)removeNavigationNodesObject:(NSManagedObject *)navigationNode;

/// Add an NSSet of RHNavigationNode objects to this location.
- (void)addNavigationNodes:(NSSet *)navigationNodes;

/// Remove an NSSet of RHNavigationNode objects from this location.
- (void)removeNavigationNodes:(NSSet *)navigationNodes;

@end
