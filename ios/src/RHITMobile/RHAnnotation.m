//
//  RHAnnotation.m
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

#import "RHAnnotation.h"
#import "RHAnnotationView.h"
#import "RHLabelNode.h"
#import "RHLocation.h"


@implementation RHAnnotation

#pragma mark - General Properties

@synthesize coordinate;
@synthesize visible;
@synthesize location;
@synthesize annotationView;

#pragma mark - General Methods

- (id)initWithLocation:(RHLocation *)inLocation
      currentZoomLevel:(NSUInteger)zoomLevel {
    self = [super init];
    
    if (self) {
        // Set our location property and determine whether or not we should
        // start out visible.
        self.location = inLocation;
        visibleZoomLevel_ = inLocation.visibleZoomLevel.intValue;
        self.visible = zoomLevel >= self.location.visibleZoomLevel.intValue;
    }
    
    return self;
}

- (void)mapView:(MKMapView *)mapView didChangeZoomLevel:(NSUInteger)zoomLevel {
    if (self.visible && zoomLevel < visibleZoomLevel_) {
        
        // We're below the minimum zoom level for displaying this annotation,
        // so mark it as invisible.
        self.visible = NO;
        [self.annotationView updateAnnotationVisibility];
        
    } else if (!self.visible && zoomLevel >= visibleZoomLevel_) {
        
        // We're within the acceptable zoom level threshold for displaying
        // this annotation, so mark it as visible.
        self.visible = YES;
        [self.annotationView updateAnnotationVisibility];
        
    }
}

#pragma mark - Property Methods

- (BOOL)area {
    return self.location.boundaryNodes != nil &&
    self.location.boundaryNodes.count > 0;
}

- (CLLocationCoordinate2D)coordinate {
    return self.location.labelLocation.coordinate;
}

- (NSString *)title {
    return self.location.name;
}

- (NSString *)subtitle {
    return self.location.quickDescription;
}


#pragma mark - Private Methods

- (NSInteger)visibleZoomLevel {
    return self.location.visibleZoomLevel.integerValue;
}

@end
