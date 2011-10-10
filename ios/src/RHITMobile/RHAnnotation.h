//
//  RHAnnotation.h
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
#import "MKMapView+ZoomLevel.h"


@class RHLocation;
@class RHAnnotationView;

/// \ingroup map
/// Representation of an annotatable map object. For use by RHAnnotationView.
@interface RHAnnotation : NSObject <MKAnnotation>

/// The "center point" of sorts for this location
@property (nonatomic, assign) CLLocationCoordinate2D coordinate;

/// Determines whether or not text or polygons will be rendered.
@property (nonatomic, assign) BOOL visible;

/// Location model to pull data from.
@property (nonatomic, retain) RHLocation *location;

/// The RHAnnotationView tied to this RHAnnotation once it is rendered to the
/// map. This reference is used to tell the view when it needs to hide or show
/// the annotation's text.
@property (nonatomic, retain) RHAnnotationView *annotationView;

/// Initialize with only an RHLocation and RHAnnotationType. This automatically
/// tries to guess the center point of the RHLocation.
- (RHAnnotation *)initWithLocation:(RHLocation *)location
                  currentZoomLevel:(NSUInteger)zoomLevel;

/// Nofify this RHAnnotation that the map view containing it has changed its
/// zoom level. This allows us to determine when to show and hide the
/// annotation's text.
- (void)mapView:(MKMapView *)mapView didChangeZoomLevel:(NSUInteger)zoomLevel;

@end