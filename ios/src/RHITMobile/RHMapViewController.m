//
//  RHMapViewController.m
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

#import "RHMapViewController.h"
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
#import "RHAppDelegate.h"
#import "RHQuickListViewController.h"
#import "RHPinAnnotationView.h"
#import "RHLocationDetailViewController.h"
#import "RHSearchViewController.h"
#import "RHDirectionLineItem.h"
#import "RHSimplePointAnnotation.h"


#pragma mark Private Method Declarations

@interface RHMapViewController()

@property (nonatomic, strong) RHLocationOverlay *currentOverlay;

@property (nonatomic, strong) NSMutableDictionary *locationsDisplayed;

- (void)loadStoredLocations;

- (void)populateMapWithLocations:(NSSet *)locations;

@end


#pragma mark -
#pragma mark Implementation

@implementation RHMapViewController

#pragma mark -
#pragma mark Generic Properties

@synthesize mapView;
@synthesize fetchedResultsController;
@synthesize managedObjectContext;
@synthesize remoteHandler = remoteHandler_;
@synthesize quickListAnnotations;
@synthesize temporaryAnnotations;
@synthesize backgroundView;
@synthesize directionsLabel;
@synthesize directionsPicker;

// Private properties
@synthesize currentOverlay;
@synthesize locationsDisplayed;

#pragma mark -
#pragma mark General Methods

- (void)viewDidLoad {
    [super viewDidLoad];
    
    self.managedObjectContext = [(RHAppDelegate *)
                                 [[UIApplication sharedApplication]
                                  delegate] managedObjectContext];
    
    self.locationsDisplayed = [[NSMutableDictionary alloc] initWithCapacity:20];
    
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
        annotation = [[RHAnnotation alloc] initWithLocation:location
                                  currentZoomLevel:self.mapView.zoomLevel];
    }
    
    [self focusMapViewToAnnotation:annotation];
}

#pragma mark -
#pragma mark Property Methods

- (RHRestHandler *)remoteHandler {
    if (remoteHandler_ == nil) {
        RHAppDelegate *appDelegate;
        appDelegate = (RHAppDelegate *)[[UIApplication
                                                 sharedApplication] delegate];
        remoteHandler_ = [RHRestHandler alloc];
        remoteHandler_ = [remoteHandler_
                          initWithPersistantStoreCoordinator:appDelegate.persistentStoreCoordinator
                          delegate:self];
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
    RHSearchViewController *search = [RHSearchViewController alloc];
    search = [search initWithNibName:kRHSearchViewControllerNibName bundle:nil];
    search.searchType = RHSearchViewControllerTypeLocation;
    search.remoteHandler = self.remoteHandler;
    search.context = self.managedObjectContext;
    [self.navigationController pushViewController:search animated:YES];
}

- (IBAction)displayQuickList:(id)sender {
    RHQuickListViewController *quickList = [RHQuickListViewController alloc];
    quickList = [quickList initWithNibName:kRHQuickListViewControllerNibName bundle:nil];
    quickList.mapViewController = self;
    [self presentModalViewController:quickList animated:YES];
}

- (IBAction)discloseLocationDetails:(id)sender {
    MKAnnotationView *view = (MKAnnotationView *) ((UIView *) sender).superview.superview;
    RHAnnotation *annotation = (RHAnnotation *) view.annotation;
    RHLocationDetailViewController *detailViewController = [[RHLocationDetailViewController alloc] initWithNibName:kRHLocationDetailViewControllerNibName bundle:nil];
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
                pinView = [[RHPinAnnotationView alloc]
                            initWithAnnotation:annotation
                            reuseIdentifier:identifier];
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
            annotationView = [[RHAnnotationView alloc]
                               initWithAnnotation:annotation
                               reuseIdentifier:identifier];
        }
        
        [annotationView setEnabled:YES];
        [annotationView setCanShowCallout:YES];
        [annotationView setDraggable:NO];
        [annotationView setDelegate:self];
        
        UIButton *newButton = [UIButton
                               buttonWithType:UIButtonTypeDetailDisclosure];
        [newButton addTarget:self action:@selector(discloseLocationDetails:) forControlEvents:UIControlEventTouchUpInside];
        [annotationView setRightCalloutAccessoryView:newButton];
        
        annotation.annotationView = annotationView;
        
        return annotationView;
    }
    
    if ([inAnnotation isKindOfClass:[RHSimplePointAnnotation class]]) {
        MKPinAnnotationView *annotationView = (MKPinAnnotationView *)[self.mapView dequeueReusableAnnotationViewWithIdentifier:@"SimplePinView"];
        
        if (annotationView == nil) {
            annotationView = [[MKPinAnnotationView alloc] initWithAnnotation:inAnnotation reuseIdentifier:@"SimplePinView"];
            annotationView.animatesDrop = YES;
        }
        
        RHSimplePointAnnotation *annotation = inAnnotation;
        
        if ([annotation color] == RHSimplePointAnnotationColorRed) {
            annotationView.pinColor = MKPinAnnotationColorRed;
        } else if ([annotation color] == RHSimplePointAnnotationColorGreen) {
            annotationView.pinColor = MKPinAnnotationColorGreen;
        } else if ([annotation color] == RHSimplePointAnnotationColorBlue) {
            annotationView.pinColor = MKPinAnnotationColorPurple;
        }
        
        return annotationView;
    }
    
    return nil;
}

- (MKOverlayView *)mapView:(MKMapView *)mapView
            viewForOverlay:(id<MKOverlay>)overlay {
    if ([overlay isKindOfClass:[RHLocationOverlay class]]) {
        MKPolygon *polygon = ((RHLocationOverlay *) overlay).polygon;
        MKPolygonView *view = [[MKPolygonView alloc] initWithPolygon:polygon];
        
        view.fillColor = [[UIColor cyanColor] colorWithAlphaComponent:0.2];
        view.strokeColor = [[UIColor blueColor] colorWithAlphaComponent:0.7];
        view.lineWidth = 3;
        
        return view;
    } else if ([overlay isKindOfClass:[MKPolyline class]]) {
        NSLog(@"Polyline view");
        MKPolylineView *view = [[MKPolylineView alloc] initWithPolyline:overlay];
        view.strokeColor = [UIColor blueColor];
        view.fillColor = [UIColor blueColor];
        view.lineWidth = 10;
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
    
    self.currentOverlay = [[RHLocationOverlay alloc]
                            initWithLocation:annotation.location];
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
    self.locationsDisplayed = [[NSMutableDictionary alloc] initWithCapacity:20];
}

#pragma mark -
#pragma mark Private Methods

- (void)loadStoredLocations {
    // Describe the type of entity we'd like to retrieve
    NSEntityDescription *entityDescription;
    entityDescription = [NSEntityDescription
                         entityForName:@"Location"
                         inManagedObjectContext:managedObjectContext];
    
    NSFetchRequest *request = [[NSFetchRequest alloc] init];
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
    
    // Retrieve what we hope is our created object
    NSError *error = nil;
    NSArray *results = [managedObjectContext executeFetchRequest:request
                                                           error:&error];
    self.quickListAnnotations = [[NSMutableArray alloc]
                                  initWithCapacity:results.count];
    [self populateMapWithLocations:(NSSet *)results];
    
    [[RHAppDelegate instance] prefetchLocationNames];
}

- (void)populateMapWithLocations:(NSSet *)locations {
    // Clear existing annotations first
    [self.mapView removeAnnotations:self.mapView.annotations];
    
    NSInteger currentZoomLevel = self.mapView.zoomLevel;
    
    for (RHLocation *location in locations) {
        RHAnnotation *annotation = [RHAnnotation alloc];
        annotation = [annotation initWithLocation:location
                                  currentZoomLevel:currentZoomLevel];
        
        [self.locationsDisplayed setObject:annotation forKey:location.objectID];
        
        if (location.displayType == RHLocationDisplayTypeQuickList) {
            [self.quickListAnnotations addObject:annotation];
        }
        
        if (location.visibleZoomLevel.intValue > 0) {
            [self.mapView addAnnotation:annotation];
        }
    }
}

- (void)displayPath:(MKPolyline *)path {
    if (self.mapView.selectedAnnotations.count > 0) {
        [self.mapView deselectAnnotation:[self.mapView.selectedAnnotations objectAtIndex:0] animated:NO];
    }
    self.backgroundView.hidden = NO;
    self.directionsLabel.hidden = NO;
    self.directionsPicker.hidden = NO;
    self.mapView.frame = CGRectMake(0, 0, 320, 155);
    
    [self clearAllDynamicMapArtifacts];
    [self.mapView addOverlay:path];
    [self.mapView setCenterCoordinate:self.mapView.centerCoordinate zoomLevel:15 animated:YES];
}

- (void)displayDirections:(NSArray *)directions {
    if (self.mapView.selectedAnnotations.count > 0) {
        [self.mapView deselectAnnotation:[self.mapView.selectedAnnotations objectAtIndex:0] animated:NO];
        [self clearAllDynamicMapArtifacts];
    }
    RHDirectionLineItem *start = [directions objectAtIndex:0];
    [self.mapView setCenterCoordinate:start.coordinate zoomLevel:17 animated:YES];
    
    directionsStatusBar_ = [[UIView alloc] initWithFrame:CGRectMake(0, -50, 320, 50)];
    directionsStatusBar_.backgroundColor = [[UIColor blackColor] colorWithAlphaComponent:0.7];
    
    directionsStatus_ = [[UILabel alloc] initWithFrame:CGRectMake(5, 5, 310, 40)];
    directionsStatus_.backgroundColor = [UIColor clearColor];
    directionsStatus_.textColor = [UIColor whiteColor];
    directionsStatus_.text = [[directions objectAtIndex:0] name];
    directionsStatus_.numberOfLines = 2;
    directionsStatus_.textAlignment = UITextAlignmentCenter;
    
    [directionsStatusBar_ addSubview:directionsStatus_];
    
    directionsControls_ = [[UIToolbar alloc] initWithFrame:CGRectMake(0, self.mapView.frame.size.height, 320, 44)];
    
    directionsControls_.tintColor = [UIColor blackColor];
    
    UIBarButtonItem *prevItem = [[UIBarButtonItem alloc] initWithTitle:@"Prev" style:UIBarButtonItemStyleBordered target:self action:@selector(prevDirection:)];
    
    UIBarButtonItem *nextItem = [[UIBarButtonItem alloc] initWithTitle:@"Next" style:UIBarButtonItemStyleBordered target:self action:@selector(nextDirection:)];
    
    UIBarButtonItem *spaceItem = [[UIBarButtonItem alloc] initWithBarButtonSystemItem:UIBarButtonSystemItemFlexibleSpace target:nil action:nil];
    
    UIBarButtonItem *cancelItem = [[UIBarButtonItem alloc] initWithTitle:@"Exit Directions" style:UIBarButtonItemStyleBordered target:self action:@selector(exitDirections:)];
    
    directionsControls_.items = [NSArray arrayWithObjects:prevItem, nextItem, spaceItem, cancelItem, nil];
    
    [self.view addSubview:directionsStatusBar_];
    [self.view addSubview:directionsControls_];
    
    [self slideInDirectionsTitle:directionsStatusBar_];
    [self slideInDirectionsControls:directionsControls_];
    
    CLLocationCoordinate2D coords[directions.count];
    
    directionsPins_ = [NSMutableArray arrayWithCapacity:directions.count];
    
//    RHDirectionLineItem *last = [directions objectAtIndex:(directions.count - 1)];
//    
//    RHSimplePointAnnotation *lastAnnotation = [[[RHSimplePointAnnotation alloc] init] autorelease];
//    lastAnnotation.coordinate = last.coordinate;
//    lastAnnotation.color = RHSimplePointAnnotationColorRed;
//    [self.mapView addAnnotation:lastAnnotation];
    
//    [directionsPins_ addObject:lastAnnotation];
    
    currentDirectionAnnotation_ = [[RHSimplePointAnnotation alloc] init];
    currentDirectionAnnotation_.coordinate = start.coordinate;
    currentDirectionAnnotation_.color = RHSimplePointAnnotationColorGreen;
    [self.mapView addAnnotation:currentDirectionAnnotation_];

    
    for (RHDirectionLineItem *lineItem in directions) {
        coords[[directions indexOfObject:lineItem]] = lineItem.coordinate;
        
        if (lineItem.flagged) {
            RHSimplePointAnnotation *annotation = [[RHSimplePointAnnotation alloc] init];
            annotation.coordinate = lineItem.coordinate;
            [self.mapView addAnnotation:annotation];
            
            [directionsPins_ addObject:annotation];
        }
    }
    
    MKPolyline *line = [MKPolyline polylineWithCoordinates:coords
                                                     count:directions.count];
    
    [self.mapView addOverlay:line];
    
    currentDirections_ = directions;
    
    currentDirectionIndex_ = 0;
}

- (void)slideInDirectionsTitle:(UIView *)titleView {
    [UIView beginAnimations:nil context:NULL];
    [UIView setAnimationDelay:0.75];
    [UIView setAnimationDuration:.25];
    
    CGRect frame = titleView.frame;
    frame.origin.y = 0;
    titleView.frame = frame;
    
    [UIView commitAnimations];
}

- (void)slideInDirectionsControls:(UIToolbar *)controls {
    [UIView beginAnimations:nil context:NULL];
    [UIView setAnimationDelay:0.75];
    [UIView setAnimationDuration:0.25];
    
    CGRect mapFrame = self.mapView.frame;
    mapFrame.size.height = mapFrame.size.height - 40;
    self.mapView.frame = mapFrame;
    
    CGRect toolbarFrame = controls.frame;
    toolbarFrame.origin.y = self.mapView.frame.size.height;
    controls.frame = toolbarFrame;
    
    [UIView commitAnimations];
}

- (void)nextDirection:(id)sender {
    if (currentDirectionIndex_ < currentDirections_.count - 1) {
        currentDirectionIndex_ ++;
        RHDirectionLineItem *direction = [currentDirections_ objectAtIndex:currentDirectionIndex_];
        if (![direction.name isKindOfClass:[NSNull class]]) {
            directionsStatus_.text = direction.name;
        }
        currentDirectionAnnotation_.coordinate = direction.coordinate;
        [self.mapView setCenterCoordinate:direction.coordinate animated:YES];
    }
}

- (void)prevDirection:(id)sender {
    if (currentDirectionIndex_ > 0) {
        currentDirectionIndex_ --;
        RHDirectionLineItem *direction = [currentDirections_ objectAtIndex:currentDirectionIndex_];
        if (![direction.name isKindOfClass:[NSNull class]]) {
            directionsStatus_.text = direction.name;
        }
        currentDirectionAnnotation_.coordinate = direction.coordinate;
        [self.mapView setCenterCoordinate:direction.coordinate animated:YES];
    }
}

- (void)exitDirections:(id)sender {
    [self.mapView removeOverlays:self.mapView.overlays];
    [self.mapView removeAnnotation:currentDirectionAnnotation_];
    [self.mapView removeAnnotations:directionsPins_];
    
    [UIView beginAnimations:nil context:NULL];
    [UIView setAnimationDuration:0.25];
    
    CGRect mapFrame = self.mapView.frame;
    mapFrame.size.height = mapFrame.size.height + 40;
    self.mapView.frame = mapFrame;
    
    CGRect frame = directionsStatusBar_.frame;
    frame.origin.y = -50;
    directionsStatusBar_.frame = frame;
    
    CGRect toolbarFrame = directionsControls_.frame;
    toolbarFrame.origin.y = self.mapView.frame.size.height;
    directionsControls_.frame = toolbarFrame;
    
    [UIView commitAnimations];
}

@end