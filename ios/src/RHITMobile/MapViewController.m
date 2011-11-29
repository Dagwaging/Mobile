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
#import "RHNode.h"
#import "RHLabelNode.h"
#import "RHRemoteHandler.h"
#import "RHRestHandler.h"
#import "RHLocationOverlay.h"
#import "RHITMobileAppDelegate.h"
#import "QuickListViewController.h"
#import "RHPinAnnotationView.h"
#import "LocationDetailViewController.h"
#import "SearchViewController.h"


#pragma mark Private Method Declarations

@interface MapViewController()

@property (nonatomic, retain) RHLocationOverlay *currentOverlay;

@property (nonatomic, retain) NSMutableDictionary *locationsDisplayed;

- (void)loadStoredLocations;

- (void)populateMapWithLocations:(NSSet *)locations;

@end


#pragma mark -
#pragma mark Implementation

@implementation MapViewController

#pragma mark -
#pragma mark Generic Properties

@synthesize mapView;
@synthesize fetchedResultsController;
@synthesize managedObjectContext;
@synthesize remoteHandler = remoteHandler_;
@synthesize quickListAnnotations;
@synthesize temporaryAnnotations;

// Private properties
@synthesize currentOverlay;
@synthesize locationsDisplayed;

#pragma mark -
#pragma mark General Methods

- (void)viewDidLoad {
    [super viewDidLoad];
    
    self.managedObjectContext = [(RHITMobileAppDelegate *)
                                 [[UIApplication sharedApplication]
                                  delegate] managedObjectContext];
    
    self.locationsDisplayed = [[[NSMutableDictionary alloc] initWithCapacity:20] autorelease];
    
    // Initialize what's visible on the map
    CLLocationCoordinate2D center = {kRHCampusCenterLatitude,
        kRHCampusCenterLongitude};
    
    self.mapView.mapType = MKMapTypeSatellite;
    self.mapView.showsUserLocation = YES;
    [self.mapView setCenterCoordinate:center
                            zoomLevel:kRHInitialZoomLevel
                             animated:NO];
    
    [self loadStoredLocations];
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

- (void)focusMapViewToTemporaryAnnotation:(RHAnnotation *)annotation {
    RHAnnotation *currentAnnotation = [self.mapView.selectedAnnotations objectAtIndex:0];
    if (currentAnnotation != nil) {
        [self.mapView deselectAnnotation:currentAnnotation animated:NO];
        [self.mapView removeOverlay:self.currentOverlay];
    }
    
    [self performSelector:@selector(clearOverlays) withObject:nil afterDelay:0.01];
    [self performSelector:@selector(clearOverlays) withObject:nil afterDelay:0.3];
    self.temporaryAnnotations = [NSArray arrayWithObject:annotation];
    [self.mapView addAnnotation:annotation];
    [self.mapView setCenterCoordinate:annotation.location.labelLocation.coordinate zoomLevel:kRHLocationFocusZoomLevel animated:NO];
    [self.mapView selectAnnotation:annotation animated:NO];
}

- (void)focusMapViewToLocation:(RHLocation *)location {
    RHAnnotation *annotation = [self.locationsDisplayed objectForKey:location.objectID];
    
    if (annotation == nil) {
        annotation = [[[RHAnnotation alloc] initWithLocation:location
                                  currentZoomLevel:self.mapView.zoomLevel]
                      autorelease];
    }
    
    [self focusMapViewToAnnotation:annotation];
}

#pragma mark -
#pragma mark Property Methods

- (RHRemoteHandler *)remoteHandler {
    if (remoteHandler_ == nil) {
        RHITMobileAppDelegate *appDelegate;
        appDelegate = (RHITMobileAppDelegate *)[[UIApplication
                                                 sharedApplication] delegate];
        remoteHandler_ = (RHRemoteHandler *) [RHRestHandler alloc];
        remoteHandler_ = [remoteHandler_
                          initWithPersistantStoreCoordinator:appDelegate.persistentStoreCoordinator
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

- (IBAction)displaySearch:(id)sender {
    SearchViewController *search = [SearchViewController alloc];
    search = [search initWithNibName:@"SearchView" bundle:nil];
    search.searchType = RHSearchViewControllerTypeLocation;
    [self.navigationController pushViewController:search animated:YES];
    [search release];
}

- (IBAction)displayQuickList:(id)sender {
    QuickListViewController *quickList = [QuickListViewController alloc];
    quickList = [quickList initWithNibName:@"QuickListView" bundle:nil];
    quickList.mapViewController = self;
    [self presentModalViewController:quickList animated:YES];
    [quickList release];
}

- (IBAction)discloseLocationDetails:(id)sender {
    MKAnnotationView *view = (MKAnnotationView *) ((UIView *) sender).superview.superview;
    RHAnnotation *annotation = (RHAnnotation *) view.annotation;
    LocationDetailViewController *detailViewController = [[[LocationDetailViewController alloc] initWithNibName:@"LocationDetailView" bundle:nil] autorelease];
    detailViewController.location = annotation.location;
    [self.navigationController pushViewController:detailViewController animated:YES];
}

# pragma mark -
# pragma mark MKMapViewDelegate Methods

- (MKAnnotationView *)mapView:(MKMapView *)mapView
            viewForAnnotation:(id <MKAnnotation>)inAnnotation {
    
    if ([inAnnotation isKindOfClass:[RHAnnotation class]]) {
        RHAnnotation *annotation = (RHAnnotation *)inAnnotation;
        
        NSString *identifier = annotation.location.serverIdentifier.description;
        
        if (annotation.location.visibleZoomLevel.intValue <= 0) {
            RHPinAnnotationView *pinView = (RHPinAnnotationView *)
                [self.mapView
                 dequeueReusableAnnotationViewWithIdentifier:identifier];
            
            if (pinView == nil) {
                pinView = [[[RHPinAnnotationView alloc]
                            initWithAnnotation:annotation
                            reuseIdentifier:identifier] autorelease];
            }
            
            pinView.canShowCallout = YES;
            pinView.draggable = NO;
            pinView.mapViewController = self;
            
            UIButton *pinDisclosureButton = [UIButton
                                             buttonWithType:UIButtonTypeDetailDisclosure];
            [pinDisclosureButton addTarget:self action:@selector(discloseLocationDetails:) forControlEvents:UIControlEventTouchUpInside];
            [pinView setRightCalloutAccessoryView:pinDisclosureButton];
            
            return pinView;
        }
        
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
        
        UIButton *newButton = [UIButton
                               buttonWithType:UIButtonTypeDetailDisclosure];
        [newButton addTarget:self action:@selector(discloseLocationDetails:) forControlEvents:UIControlEventTouchUpInside];
        [annotationView setRightCalloutAccessoryView:newButton];
        
        annotation.annotationView = annotationView;
        
        return annotationView;
    }
    
    return nil;
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
    CLLocationCoordinate2D newCenter = map.centerCoordinate;
    CLLocationCoordinate2D originalCenter = CLLocationCoordinate2DMake(kRHCampusCenterLatitude, kRHCampusCenterLongitude);
    BOOL needsToSnapBack = NO;
    
    if (newZoomLevel < kRHMinimumZoomLevel) {
        newZoomLevel = kRHMinimumZoomLevel;
        needsToSnapBack = YES;
    }
    
    if (newCenter.latitude < kRHMinimumLatitude) {
        needsToSnapBack = YES;
        newCenter = originalCenter;
    } else if (newCenter.latitude > kRHMaximumLatitude) {
        needsToSnapBack = YES;
        newCenter = originalCenter;
    }
    
    if (newCenter.longitude < kRHMinimumLongitude) {
        needsToSnapBack = YES;
        newCenter = originalCenter;
    } else if (newCenter.longitude > kRHMaximumLongitude) {
        needsToSnapBack = YES;
        newCenter = originalCenter;
    }
    
    if (needsToSnapBack) {
        [map setCenterCoordinate:newCenter zoomLevel:newZoomLevel animated:YES];
    }
    
    for (id annotation in self.mapView.annotations) {
        if ([annotation isKindOfClass:[RHAnnotation class]]) {
            [annotation mapView:self.mapView didChangeZoomLevel:newZoomLevel];
        }
    }
}

#pragma mark -
#pragma mark RHRemoteHandlerDelegate Methods

- (void)didFindMapLevelLocationUpdates {
    [self loadStoredLocations];
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

- (void)didFailPopulatingLocationsWithError:(NSError *)error {
    NSString *title = @"Error Populating Locations";
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

- (void)focusMapViewToAnnotation:(RHAnnotation *)annotation {
    if (annotation.area) {
        [self focusMapViewToAreaAnnotation:annotation selected:NO];
    } else {
        [self focusMapViewToPointAnnotation:annotation];
    }
}

- (void)focusMapViewToAreaAnnotation:(RHAnnotation *)annotation
                            selected:(BOOL)selected {
    
    [self.remoteHandler rushPopulateLocationsUnderLocationWithID:(NSManagedObjectID *)annotation.location.objectID];

    if (!selected) {
        [self clearAllDynamicMapArtifacts];
        [self.mapView selectAnnotation:annotation animated:YES];
        return;
    }
    
    [self clearAllOverlays];
    [self clearUnusedAnnotations];
    
    self.currentOverlay = [[[RHLocationOverlay alloc]
                            initWithLocation:annotation.location]
                           autorelease];
    [self.mapView addOverlay:self.currentOverlay];
    [self.mapView
     setCenterCoordinate:annotation.location.labelLocation.coordinate
     zoomLevel:kRHLocationFocusZoomLevel
     animated:YES];
}

- (void)focusMapViewToPointAnnotation:(RHAnnotation *)annotation {
    [self clearAllDynamicMapArtifacts];
    self.temporaryAnnotations = [NSArray arrayWithObject:annotation];
    [self.mapView addAnnotation:annotation];
    [self.mapView selectAnnotation:annotation animated:NO];
    [self.mapView
     setCenterCoordinate:annotation.location.labelLocation.coordinate
     zoomLevel:kRHLocationFocusZoomLevel
     animated:YES];
}

- (void)clearUnusedDynamicMapArtifacts {
    [self clearUnusedAnnotations];
    [self clearUnusedOverlays];
}

- (void)clearUnusedAnnotations {
    if (self.mapView.selectedAnnotations == nil ||
        self.mapView.selectedAnnotations.count == 0) {
        [self clearAllAnnotations];
        return;
    }
    
    for (id annotationId in self.mapView.selectedAnnotations) {
        if ([annotationId isKindOfClass:[RHAnnotation class]]) {
            RHAnnotation *annotation = (RHAnnotation *)annotationId;
            if (!annotation.area) {
                return;
            }
        }
    }
    

    [self clearAllAnnotations];
}

- (void)clearUnusedOverlays {
    if (self.mapView.selectedAnnotations == nil ||
        self.mapView.selectedAnnotations.count == 0) {
        [self clearAllOverlays];
        return;
    }
    
    for (id annotationId in self.mapView.selectedAnnotations) {
        if ([annotationId isKindOfClass:[RHAnnotation class]]) {
            RHAnnotation *annotation = (RHAnnotation *)annotationId;
            if (annotation.area) {
                return;
            }
        }
    }
    
    [self clearAllOverlays];
}

- (void)clearAllDynamicMapArtifacts {
    [self clearAllAnnotations];
    [self clearAllOverlays];
}

- (void)clearAllOverlays {
    [self.mapView removeOverlay:self.currentOverlay];
    self.currentOverlay = nil;
}

- (void)clearAllAnnotations {
    [self.mapView removeAnnotations:self.temporaryAnnotations];
    self.temporaryAnnotations = nil;
    self.locationsDisplayed = [[[NSMutableDictionary alloc] initWithCapacity:20] autorelease];
}

#pragma mark -
#pragma mark Private Methods

- (void)loadStoredLocations {
    // Describe the type of entity we'd like to retrieve
    NSEntityDescription *entityDescription;
    entityDescription = [NSEntityDescription
                         entityForName:@"Location"
                         inManagedObjectContext:managedObjectContext];
    
    NSFetchRequest *request = [[[NSFetchRequest alloc] init] autorelease];
    [request setEntity:entityDescription];
    
    // Put conditions on our fetch request
    NSPredicate *predicate = [NSPredicate predicateWithFormat:
                              @"visibleZoomLevel > 0 || "
                              "displayTypeNumber == %d || "
                              "displayTypeNumber == %d",
                              RHLocationDisplayTypePointOfInterest,
                              RHLocationDisplayTypeQuickList];
    NSSortDescriptor *sortDescriptor = [[NSSortDescriptor alloc]
                                        initWithKey:@"name"
                                        ascending:YES];
    request.sortDescriptors = [NSArray arrayWithObject:sortDescriptor];
    [request setPredicate:predicate];
    [sortDescriptor release];
    
    // Retrieve what we hope is our created object
    NSError *error = nil;
    NSArray *results = [managedObjectContext executeFetchRequest:request
                                                           error:&error];
    self.quickListAnnotations = [[[NSMutableArray alloc]
                                  initWithCapacity:results.count] autorelease];
    [self populateMapWithLocations:(NSSet *)results];
}

- (void)populateMapWithLocations:(NSSet *)locations {
    // Clear existing annotations first
    [self.mapView removeAnnotations:self.mapView.annotations];
    
    NSInteger currentZoomLevel = self.mapView.zoomLevel;
    
    for (RHLocation *location in locations) {
        RHAnnotation *annotation = [RHAnnotation alloc];
        annotation = [[annotation initWithLocation:location
                                  currentZoomLevel:currentZoomLevel]
                      autorelease];
        
        [self.locationsDisplayed setObject:annotation forKey:location.objectID];
        
        if (location.displayType == RHLocationDisplayTypeQuickList) {
            [self.quickListAnnotations addObject:annotation];
        }
        
        if (location.visibleZoomLevel.intValue > 0) {
            [self.mapView addAnnotation:annotation];
        }
    }
}

@end