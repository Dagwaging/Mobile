//
//  RHLocationOverlay.m
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

#import <CoreLocation/CoreLocation.h>

#import "RHLocationOverlay.h"
#import "RHLocation.h"
#import "RHLabelNode.h"
#import "RHBoundaryNode.h"


@interface RHLocationOverlay ()

- (void)updateBoundingRect;

- (void)updatePolygon;

@end

@implementation RHLocationOverlay

@synthesize location = location_;
@synthesize boundingMapRect = boundingMapRect_;
@synthesize polygon;

- (RHLocationOverlay *)initWithLocation:(RHLocation *)inLocation {
    self = [super init];
    if (self) {
        self.location = inLocation;
    }
    return self;
}

- (CLLocationCoordinate2D)coordinate {
    return self.location.labelLocation.coordinate;
}

- (void)setLocation:(RHLocation *)location {
    location_ = location;
    [self updateBoundingRect];
    [self updatePolygon];
}

#pragma mark -
#pragma mark Private Methods

- (void)updateBoundingRect {
    RHBoundaryNode *firstNode = [self.location.boundaryNodes anyObject];
    
    double littleLat = firstNode.latitude.doubleValue;
    double littleLong = firstNode.longitude.doubleValue;
    
    double bigLat = littleLat;
    double bigLong = littleLong;
    
    for (RHBoundaryNode *node in self.location.boundaryNodes) {
        if (node.latitude.doubleValue < littleLat) {
            littleLat = node.latitude.doubleValue;
        } else if (node.latitude.doubleValue > bigLat) {
            bigLat = node.latitude.doubleValue;
        }
        
        if (node.longitude.doubleValue < littleLong) {
            littleLong = node.longitude.doubleValue;
        } else if (node.longitude.doubleValue > bigLong) {
            bigLong = node.longitude.doubleValue;
        }
    }
    CLLocationCoordinate2D corner1 = CLLocationCoordinate2DMake(littleLat, littleLong);
    CLLocationCoordinate2D corner2 = CLLocationCoordinate2DMake(bigLat, bigLong);
    MKMapPoint mp1 = MKMapPointForCoordinate(corner1);
    MKMapPoint mp2 = MKMapPointForCoordinate(corner2);
    
    boundingMapRect_ = MKMapRectMake(mp1.x, mp1.y, mp2.x - mp1.x, mp2.y - mp1.y);
}

- (void)updatePolygon {
    NSArray *nodes = self.location.orderedBoundaryNodes;
    
    CLLocationCoordinate2D coords[nodes.count];
    
    for (RHBoundaryNode *node in nodes) {
        coords[[nodes indexOfObject:node]] = node.coordinate;
    }

    self.polygon = [MKPolygon polygonWithCoordinates:coords count:nodes.count];
}

@end
