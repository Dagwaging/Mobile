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


@class RHLocation;

/// Delegate responsible for repsonding to events generated from an
/// RHAnnotationView, or for performing actions requested by the annotation
/// view. An RHAnnotationViewDelegate should have access to the MKMapView
/// responsible for the RHAnnotationView in question.
@protocol RHAnnotationViewDelegate <NSObject>

/// Focus the MKMapView containing this RHAnnotationView to a specific location.
/// The final zoom level of the map is a decision left to the implemntation of
/// this method.
-(void)focusMapViewToLocation:(RHLocation *)location;

@end
