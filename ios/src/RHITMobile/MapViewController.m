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
#import "RHDummyHandler.h"


@implementation MapViewController

@synthesize mapView;
@synthesize fetchedResultsController;
@synthesize managedObjectContext;
@synthesize remoteHandler = remoteHandler_;

- (void)viewDidLoad {
    [super viewDidLoad];
    
    // Initialize what's visible on the map
    CLLocationCoordinate2D center = {RH_CAMPUS_CENTER_LATITUDE,
        RH_CAMPUS_CENTER_LONGITUDE};
    self.mapView.mapType = MKMapTypeSatellite;
    [self.mapView setCenterCoordinate:center
                            zoomLevel:RH_INITIAL_ZOOM_LEVEL
                             animated:NO];
    
    [self.remoteHandler fetchAllLocations];
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

#pragma mark -
#pragma mark RHRemoteHandlerDelegate Methods

- (RHRemoteHandler *)remoteHandler {
    if (remoteHandler_ == nil) {
        // Use the dummy handler for now
        remoteHandler_ = [[RHDummyHandler alloc]
                          initWithContext:self.managedObjectContext
                          delegate:(RHRemoteHandlerDelegate *)self];
    }
    
    return remoteHandler_;
}

- (void)didFetchAllLocations:(NSSet *)locations {
    for (RHLocation *location in locations) {
        RHAnnotation *annotation = [RHAnnotation alloc];
        annotation = [[annotation initWithLocation:location
                                    annotationType:RHAnnotationTypeText]
                      autorelease];
        [self.mapView addAnnotation:annotation];
    }
}

@end