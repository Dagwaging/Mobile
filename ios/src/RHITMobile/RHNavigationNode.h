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

/// Specialized RHNode that contains navigation-specific information, such
/// as floor level and whether or not a point is indoors.

#import "RHNode.h"

@interface RHNavigationNode : RHNode

/// Whether or not this node is indoors.
@property (nonatomic, assign) BOOL indoors;

/// Which floor this node is on, if applicable.
@property (nonatomic, assign) RHFloor floor;

/// Initialize with all properties.
- (RHNavigationNode *) initWithLatitude:(double)latitude 
                              longitude:(double)longitude 
                                indoors:(BOOL)indoors 
                                  floor:(RHFloor)floor;

@end
