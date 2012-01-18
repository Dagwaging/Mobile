//
//  RHLocationOverlay.h
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
#import <MapKit/MapKit.h>


@class RHLocation;

/// \ingroup map
/// MKOverlay representing an RHLocation that occupies and area, not just a 
/// single point.
@interface RHLocationOverlay : NSObject <MKOverlay>

/// The RHLocation this RHOverlay will represent.
@property (nonatomic, strong) RHLocation *location;

/// The polygon to be provided by this overlay for rendering on the map.
@property (nonatomic, strong) MKPolygon *polygon;

/// Initialize this overlay with an RHLocation object.
/// \param location The RHLocation this RHOverlay will represent.
- (id)initWithLocation:(RHLocation *)location;

@end