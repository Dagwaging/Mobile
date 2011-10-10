//
//  MKMapViewTests.m
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

#import "MKMapViewTests.h"
#import "MKMapView+ZoomLevel.h"
#import "RHConstants.h"
#import "RHITMobileAppDelegate.h"
#import "MapViewController.h"

@implementation MKMapViewTests

- (void)testZoomLevelSetAndGetIntegrity {
    // Retrieve app delegate, view controller, and actual map view
    RHITMobileAppDelegate *appDelegate = (RHITMobileAppDelegate *)
        UIApplication.sharedApplication.delegate;
    
    MapViewController *mapViewController = appDelegate.mapViewController;
    
    MKMapView *mapView = mapViewController.mapView;
    
    CLLocationCoordinate2D center = CLLocationCoordinate2DMake(kRHCampusCenterLatitude, kRHCampusCenterLongitude);
    
    // Set the zoom level
    [mapView setCenterCoordinate:center zoomLevel:12 animated:NO];
    
    // Check the resulting zoom level
    STAssertEquals(mapView.zoomLevel, (NSUInteger) 12,
                   @"Zoom level is incorrect");
    
    // Set the zoom level again
    [mapView setCenterCoordinate:center zoomLevel:1 animated:NO];
    
    // Check the resulting zoom level
    STAssertEquals(mapView.zoomLevel, (NSUInteger) 1,
                   @"Zoom level is incorrect");
    
    // Set the zoom level again
    [mapView setCenterCoordinate:center zoomLevel:19 animated:NO];
    
    // Check the resulting zoom level
    STAssertEquals(mapView.zoomLevel, (NSUInteger) 19,
                   @"Zoom level is incorrect");
    
    // Set the zoom level again
    [mapView setCenterCoordinate:center zoomLevel:2 animated:NO];
    
    // Check the resulting zoom level
    STAssertEquals(mapView.zoomLevel, (NSUInteger) 2,
                   @"Zoom level is incorrect");
}

@end