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
#import "RHRestHandler.h"
#import "RHLocationOverlay.h"


#pragma mark Private Method Declarations

@interface MapViewController()

@property (nonatomic, retain) RHLocationOverlay *currentOverlay;
@property (nonatomic, assign) BOOL debugMapInfo;
@property (nonatomic, assign) BOOL debugMapZoomControls;

- (void)loadStoredLocations;

- (void)populateMapWithLocations:(NSSet *)locations;

@end


#pragma mark -
#pragma mark Implementation

@implementation MapViewController

#pragma mark -
#pragma mark Generic Properties

@synthesize mapView;
@synthesize toolbar;
@synthesize zoomInButton;
@synthesize zoomOutButton;
@synthesize placesButton;
@synthesize zoomLevelLabel;
@synthesize fetchedResultsController;
@synthesize managedObjectContext;
@synthesize remoteHandler = remoteHandler_;

// Private properties
@synthesize currentOverlay;
@synthesize debugMapInfo;
@synthesize debugMapZoomControls;


#pragma mark -
#pragma mark General Methods

- (void)viewDidLoad {
    [super viewDidLoad];
    
    // Initialize what's visible on the map
    CLLocationCoordinate2D center = {kRHCampusCenterLatitude,
        kRHCampusCenterLongitude};
    
    self.mapView.mapType = MKMapTypeSatellite;
    [self.mapView setCenterCoordinate:center
                            zoomLevel:kRHInitialZoomLevel
                             animated:NO];
    
    [self loadStoredLocations];
    [self.remoteHandler checkForLocationUpdates];
    [self refreshPreferences];
}

- (BOOL) shouldAutorotateToInterfaceOrientation:(UIInterfaceOrientation)io {
    // Return YES for supported orientations
    return (io == UIInterfaceOrientationPortrait);
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

- (void)dealloc {
    [mapView release];
    [fetchedResultsController release];
    [managedObjectContext release];
    [remoteHandler_ release];
    [currentOverlay release];
    [super dealloc];
}

- (void)refreshPreferences {
    NSUserDefaults *defaults = [NSUserDefaults standardUserDefaults];
    self.debugMapInfo = [defaults boolForKey:kRHPreferenceDebugMapInfo];
    self.debugMapZoomControls = [defaults
                                 boolForKey:kRHPreferenceDebugMapZoomControls];
    
    self.zoomLevelLabel.hidden = !self.debugMapInfo;
    
    NSArray *items = [NSArray alloc];
    
    if (self.debugMapZoomControls) {
        items = [items initWithObjects:self.placesButton, self.zoomInButton,
                 self.zoomOutButton, nil];
        self.toolbar.items = items;
    } else {
        items = [items initWithObjects:self.placesButton, nil];
        self.toolbar.items = items;
    }
    
    [items release];
}

#pragma mark -
#pragma mark Property Methods

- (RHRemoteHandler *)remoteHandler {
    if (remoteHandler_ == nil) {
        remoteHandler_ = (RHRemoteHandler *) [[RHRestHandler alloc]
                          initWithContext:self.managedObjectContext
                          delegate:(RHRemoteHandlerDelegate *)self];
    }
    
    return remoteHandler_;
}

#pragma mark -
#pragma mark IBActions

- (IBAction)debugZoomIn:(id)sender {
    [self.mapView setCenterCoordinate:self.mapView.region.center
                            zoomLevel:self.mapView.zoomLevel + 1
                             animated:YES];
}

- (IBAction)debugZoomOut:(id)sender {
    [self.mapView setCenterCoordinate:self.mapView.region.center
                            zoomLevel:self.mapView.zoomLevel - 1
                             animated:YES];
}

# pragma mark -
# pragma mark MKMapViewDelegate Methods

- (MKAnnotationView *)mapView:(MKMapView *)mapView
            viewForAnnotation:(id <MKAnnotation>)inAnnotation {
    RHAnnotation *annotation = (RHAnnotation *)inAnnotation;
    NSString *identifier = annotation.location.name;
    
    RHAnnotationView *annotationView = (RHAnnotationView *)[self.mapView dequeueReusableAnnotationViewWithIdentifier:identifier];
    
    if (annotationView == nil) {
        annotationView = [[[RHAnnotationView alloc]
                           initWithAnnotation:annotation
                           reuseIdentifier:identifier] autorelease];
    }
    
    [annotationView setEnabled:YES];
    [annotationView setCanShowCallout:YES];
    [annotationView setDraggable:NO];
    [annotationView setDelegate:(RHAnnotationViewDelegate *)self];
    
    UIButton *newButton = [UIButton buttonWithType:UIButtonTypeDetailDisclosure];
    [annotationView setRightCalloutAccessoryView:newButton];
    
    annotation.annotationView = annotationView;
    
    return annotationView;
}

- (MKOverlayView *)mapView:(MKMapView *)mapView
            viewForOverlay:(id<MKOverlay>)overlay {
    if ([overlay isKindOfClass:[RHLocationOverlay class]]) {
        MKPolygon *polygon = ((RHLocationOverlay *) overlay).polygon;
        MKPolygonView *view = [[[MKPolygonView alloc] initWithPolygon:polygon]
                               autorelease];
        
        view.fillColor = [[UIColor cyanColor] colorWithAlphaComponent:0.2];
        view.strokeColor = [[UIColor blueColor] colorWithAlphaComponent:0.7];
        view.lineWidth = 3;
        
        return view;
    }
    
    return nil;
}

- (void)mapView:(MKMapView *)map regionDidChangeAnimated:(BOOL)animated {
    NSInteger newZoomLevel = map.zoomLevel;
    
    for (RHAnnotation *annotation in self.mapView.annotations) {
        [annotation mapView:self.mapView didChangeZoomLevel:newZoomLevel];
    }
    
    NSString *zoomLevelText = [[NSString alloc]
                               initWithFormat:@"Zoom Level: %d", newZoomLevel];
    self.zoomLevelLabel.text = zoomLevelText;
    [zoomLevelText release];
}

#pragma mark -
#pragma mark RHRemoteHandlerDelegate Methods

- (void)didFindMapLevelLocationUpdates:(NSSet *)locations {
    [self populateMapWithLocations:locations];
}

- (void)didFailCheckingForLocationUpdatesWithError:(NSError *)error {
    NSString *title = @"Error Updating Map";
    NSString *message = error.localizedDescription;
    
    UIAlertView *alert = [[UIAlertView alloc] initWithTitle:title
                                                    message:message
                                                   delegate:self
                                          cancelButtonTitle:@"OK"
                                          otherButtonTitles:nil, nil];
    [alert show];
    [alert release];
}

#pragma mark -
#pragma mark RHAnnotationView Delegate Methods

- (void)focusMapViewToLocation:(RHLocation *)location {
    [self.mapView removeOverlay:self.currentOverlay];
    RHLocationOverlay *overlay = [[RHLocationOverlay alloc]
                                  initWithLocation:location];
    CLLocationCoordinate2D center = [[location labelLocation] coordinate];
    [self.mapView setCenterCoordinate:center
                            zoomLevel:kRHLocationFocusZoomLevel
                             animated:YES];
    [self.mapView addOverlay:overlay];
    self.currentOverlay = overlay;
    [overlay release];
}

- (void)clearOverlays {
    [mapView removeOverlay:self.currentOverlay];
    [currentOverlay release];
    self.currentOverlay = nil;
}

#pragma mark -
#pragma mark Private Methods

- (void)loadStoredLocations {
    // Describe the type of entity we'd like to retrieve
    NSEntityDescription *entityDescription = [NSEntityDescription
                                              entityForName:@"Location"
                                              inManagedObjectContext:managedObjectContext];
    NSFetchRequest *request = [[[NSFetchRequest alloc] init] autorelease];
    [request setEntity:entityDescription];
    
    // Put conditions on our fetch request
    NSPredicate *predicate = [NSPredicate predicateWithFormat:
                              @"visibleZoomLevel > 0"];
    [request setPredicate:predicate];
    
    // Retrieve what we hope is our created object
    NSError *error = nil;
    NSArray *results = [managedObjectContext executeFetchRequest:request error:&error];
    
    [self populateMapWithLocations:(NSSet *)results];
}

- (void)populateMapWithLocations:(NSSet *)locations {
    // Clear existing annotations first
    [self.mapView removeAnnotations:self.mapView.annotations];
    
    NSInteger currentZoomLevel = self.mapView.zoomLevel;
    
    for (RHLocation *location in locations) {
        RHAnnotation *annotation = [RHAnnotation alloc];
        NSLog(@"Lat: %f Long: %f", location.labelLocation.latitude.doubleValue, location.labelLocation.longitude.doubleValue);
        annotation = [[annotation initWithLocation:location
                                  currentZoomLevel:currentZoomLevel]
                      autorelease];
        
        [self.mapView addAnnotation:annotation];
    }
}

@end