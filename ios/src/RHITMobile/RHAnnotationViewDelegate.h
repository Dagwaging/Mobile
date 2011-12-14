//
//  RHAnnotationViewDelegate.h
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


@class RHAnnotation;

/// \ingroup map
/// A delegate protocol for properly responding to events triggered by
/// RHAnnotationViews. Any object conforming to this protocol should have access
/// to the RHAnnotationView's containing MKMapView, because most of the
/// methods defined here are triggers to apply certain actions to said map view.
/// The functionality defined here doesn't necessarily have to be used directly
/// by an RHAnnotationView, but it is one of the more likely uses for it.
@protocol RHAnnotationViewDelegate <NSObject>

/// Given an RHAnnotation that may or may not already be present on the
/// annotation's containing map view, focus to its location, adding it and
/// making it visible if necessary.
/// \param annotation The RHAnnotation to focus the map to.
- (void)focusMapViewToAnnotation:(RHAnnotation *)annotation;

/// Given an RHAnnotation containing an RHLocation representing more than a
/// single point in space, focus to that annotation, adding the appropriate
/// area overlay to the map to highlight it.
/// \param annotation An RHAnnotation containing an RHLocation representing an
/// area, not a point, to focus the map to.
/// \param selected Whether or not this method is being called as the result
/// of an RHAnnotation being selected. This parameter must be accurate to avoid
/// accidental looping or duplicate adding of overlays.
- (void)focusMapViewToAreaAnnotation:(RHAnnotation *)annotation
                            selected:(BOOL)selected;

/// Given an RHAnnotation containing an RHLocation representing a single point,
/// not an area, focus to that location, adding it and making it visible if
/// necessary.
/// \param annotation An RHAnnotation containing an RHLocation representing a
/// point, not an area, to focus the map to.
- (void)focusMapViewToPointAnnotation:(RHAnnotation *)annotation;

/// Clear both all unselected area overlays and unselected point annotationns
/// from the RHAnnotationView's containing MKMapView.
- (void)clearUnusedDynamicMapArtifacts;

/// Clear all unselected area overlays from the RHAnnotationView's
/// containing MKMapView.
- (void)clearUnusedOverlays;

/// Clear all unselected point annotations from the RHAnnotationView's
/// containing MKMapView.
- (void)clearUnusedAnnotations;

/// Remove both all overlays and all annotations from the RHAnnotationView's
/// containing MKMapView.
- (void)clearAllDynamicMapArtifacts;

/// Remove all overlays from the RHAnnotationView's containing MKMapView.
- (void)clearAllOverlays;

/// Remove all annotations from the RHAnnotationView's containing MKMapView.
- (void)clearAllAnnotations;

@end
