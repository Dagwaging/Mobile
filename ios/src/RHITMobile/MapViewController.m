//
//  MapViewController.m
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

#import "MapViewController.h"
#import "MKMapView+ZoomLevel.h"
#import "RHConstants.h"
#import "RHAnnotation.h"
#import "RHAnnotationView.h"
#import "RHLocation.h"
#import "RHLabelNode.h"


@interface MapViewController ()

- (void) renderAdditionalLocations;
    
@end

@implementation MapViewController

@synthesize mapView;
@synthesize fetchedResultsController;
@synthesize managedObjectContext;

- (void)viewDidLoad {
    [super viewDidLoad];
    
    // Initialize what's visible on the map
    CLLocationCoordinate2D center = {RH_CAMPUS_CENTER_LATITUDE,
        RH_CAMPUS_CENTER_LONGITUDE};
    self.mapView.mapType = MKMapTypeSatellite;
    [self.mapView setCenterCoordinate:center
                            zoomLevel:RH_INITIAL_ZOOM_LEVEL
                             animated:NO];
    
    [self renderAdditionalLocations];
}


- (BOOL) shouldAutorotateToInterfaceOrientation:(UIInterfaceOrientation)interfaceOrientation {
    // Return YES for supported orientations
    return (interfaceOrientation == UIInterfaceOrientationPortrait);
}

- (void) didReceiveMemoryWarning {
    // Releases the view if it doesn't have a superview.
    [super didReceiveMemoryWarning];
    
    // Release any cached data, images, etc. that aren't in use.
}

- (void) viewDidUnload {
    [super viewDidUnload];
    self.mapView = nil;
}

- (void) renderAdditionalLocations {
    /// \todo This is just proof-of-concept code to render a single location.
    ///       A true implementation should obviously retrieve the location
    ///       data from somewhere dynamic.
    RHLabelNode *hatfieldCenter = (RHLabelNode *)[RHLabelNode 
                                                  fromContext:self.managedObjectContext];
    hatfieldCenter.latitude = [NSNumber numberWithDouble:39.481968];
    hatfieldCenter.longitude = [NSNumber numberWithDouble:-87.322276];
    RHLocation *hatfield = [RHLocation fromContext:self.managedObjectContext];
    hatfield.name = @"Hatfield Hall";
    hatfield.labelLocation = hatfieldCenter;
    RHAnnotation *annotation = [[[RHAnnotation alloc] initWithLocation:hatfield
                                                       annotationType:RHAnnotationTypeText] autorelease];
    [self.mapView addAnnotation:annotation];
}

# pragma mark -
# pragma mark MKMapViewDelegate Methods

- (MKAnnotationView *)mapView:(MKMapView *)mapView
            viewForAnnotation:(id <MKAnnotation>)inAnnotation {
    RHAnnotation *annotation = (RHAnnotation *)inAnnotation;
    NSString *identifier = annotation.location.name;
    
    RHAnnotationView *annotationView = (RHAnnotationView *)[self.mapView dequeueReusableAnnotationViewWithIdentifier:identifier];
    
    if (annotationView == nil) {
        annotationView = [[[RHAnnotationView alloc] initWithAnnotation:annotation
                                                       reuseIdentifier:identifier] autorelease];
    }
    
    annotationView.enabled = YES;
    
    return annotationView;
}

@end