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
/// Representation of an annotatable map object, used specifically for
/// displaying text labels associated with RHLocation objects. RHAnnotation
/// objects need to be notified of changes in the zoom level of their
/// containing MKMapView, because they are the objects in charge of deciding
/// whether or not to display the text associated with a location.
@interface RHAnnotation : NSObject <MKAnnotation> {
@private
    NSInteger visibleZoomLevel_;
}

/// The latitude/longitude point in space at which this annotation should
/// appear. Because RHAnnotations are used to display text labels for
/// RHLocations, this property is simply a proxy to the label coordinate
/// contained within this RHAnnotation's RHLocation object.
@property (nonatomic, assign) CLLocationCoordinate2D coordinate;

/// Whether or not this annotation will actually be visible on the map.
/// This property can be set directly, though this shouldn't necessarily be
/// needed as long as the annotation is notified of zoom level changes to
/// its parent MKMapView.
@property (nonatomic, assign) BOOL visible;

/// Whether or not this RHAnnotation represents an object that has more than
/// a single point to its location. This is mostly a sign that the enclosed
/// RHLocation is a building, though not necessarily. This property is readonly
/// as it is simply a proxy to data contained within the RHLocation object.
@property (nonatomic, readonly) BOOL area;

/// The RHLocation model object enclosed by this RHAnnotation. This property
/// can be set directly, but probably just be set in the initizlization method
/// call instead, as it is unlikely to change. Much of the data associated
/// with the RHAnnotation is pulled from this enclosed RHLocation, so it
/// is absolutely necessary that this property be set.
@property (nonatomic, retain) RHLocation *location;

/// The RHAnnotationView tied to this RHAnnotation once it is rendered to the
/// map. This reference is used to tell the view when it needs to hide or show
/// the annotation's text.
@property (nonatomic, retain) RHAnnotationView *annotationView;

/// General initializer method for the RHAnnotation class. When this method
/// is called, the object has all that it needs to be successfully added
/// to a MKMapView.
/// \param location The RHLocation object to associate with this RHAnnotation.
/// Any location-specific attributes of the RHAnnotation will be pulled from
/// this RHLocation.
/// \param zoomLevel The current zoom level value of the MKMapView this
/// RHAnnotation will be added to. This value is used to determine whether or
/// not the annotation should initially be visible when rendered.
- (id)initWithLocation:(RHLocation *)location
      currentZoomLevel:(NSUInteger)zoomLevel;

/// Notify this RHAnnotation that the map view containing it has changed its
/// zoom level. This allows us to determine when to show and hide the
/// annotation's text. The zoom level parameter here is explicit because the
/// calculation of the map's zoom level for every annotation could be
/// potentially time consuming.
/// \param mapView The MKMapView containing this RHAnnotation object.
/// \param zoomLevel The new zoom level of this MKMapView.
- (void)mapView:(MKMapView *)mapView didChangeZoomLevel:(NSUInteger)zoomLevel;

@end
