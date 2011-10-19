//
//  RHLocation.m
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

#import "RHLocation.h"
#import "RHBoundaryNode.h"
#import "RHLabelNode.h"


@implementation RHLocation

@dynamic serverIdentifier;
@dynamic name;
@dynamic quickDescription;
@dynamic inQuickList;
@dynamic visibleZoomLevel;
@dynamic boundaryNodes;
@dynamic labelLocation;
@dynamic navigationNodes;

+ (RHLocation *)fromContext:(NSManagedObjectContext *)context {
    RHLocation *location = [NSEntityDescription
                            insertNewObjectForEntityForName:@"Location"
                            inManagedObjectContext:context];
    return location;
}

- (void)setOrderedBoundaryNodes:(NSArray *)inBoundaryNodes {
    for (RHBoundaryNode *node in self.boundaryNodes) {
        [self removeBoundaryNodesObject:node];
    }
    
    for (RHBoundaryNode *node in inBoundaryNodes) {
        node.position = [NSNumber numberWithInt:[inBoundaryNodes
                                                 indexOfObject:node]];
        [self addBoundaryNodesObject:node];
    }
}

- (NSArray *)orderedBoundaryNodes {
    return [[self.boundaryNodes allObjects]
            sortedArrayUsingComparator: ^(id n1, id n2) {
                
                RHBoundaryNode *node1 = (RHBoundaryNode *) n1;
                RHBoundaryNode *node2 = (RHBoundaryNode *) n2;
                
                
                if (node1.position.integerValue > node2.position.integerValue) {
                    return (NSComparisonResult)NSOrderedDescending;
                }
                
                if (node1.position.integerValue < node2.position.integerValue) {
                    return (NSComparisonResult)NSOrderedAscending;
                }
                
                return (NSComparisonResult)NSOrderedSame;
            }];
}

@end
