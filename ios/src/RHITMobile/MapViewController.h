//
//  MapViewController.h
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

#import <UIKit/UIKit.h>
#import <CoreData/CoreData.h>

#import "MKMapView+ZoomLevel.h"
#import "RHRemoteHandlerDelegate.h"
#import "RHAnnotationViewDelegate.h"


@class RHRemoteHandler;
@class RHAnnotation;
@class RHLocation;
@class RHSimplePointAnnotation;

/// \ingroup views
/// View controller for the map portion of the application.
@interface MapViewController : UIViewController
<MKMapViewDelegate, RHRemoteHandlerDelegate, RHAnnotationViewDelegate> {
    @private
    NSArray *currentDirections_;
    NSUInteger currentDirectionIndex_;
    RHSimplePointAnnotation *currentDirectionAnnotation_;
}

/// Map view that is visible to the user.
@property (nonatomic, retain) IBOutlet MKMapView *mapView;

/// Core Data fetched results controller.
@property (nonatomic, retain) NSFetchedResultsController *fetchedResultsController;

/// Core Data managed object context. 
@property (nonatomic, retain) NSManagedObjectContext *managedObjectContext;

/// Remote data handler.
@property (nonatomic, retain) RHRemoteHandler *remoteHandler;

@property (nonatomic, retain) NSMutableArray *quickListAnnotations;

@property (nonatomic, retain) NSArray *temporaryAnnotations;

@property (nonatomic, retain) IBOutlet UIView *backgroundView;

@property (nonatomic, retain) IBOutlet UILabel *directionsLabel;

@property (nonatomic, retain) IBOutlet UIPickerView *directionsPicker;

- (void)focusMapViewToTemporaryAnnotation:(RHAnnotation *)annotation;

- (void)focusMapViewToLocation:(RHLocation *)location;

- (IBAction)debugZoomIn:(id)sender;

- (IBAction)debugZoomOut:(id)sender;

- (IBAction)displayQuickList:(id)sender;

- (IBAction)displaySearch:(id)sender;

- (IBAction)discloseLocationDetails:(id)sender;

- (void)displayPath:(MKPolyline *)path;

- (void)displayDirections:(NSArray *)directions;

- (void)slideInDirectionsTitle:(UIView *)titleView;

- (void)slideInDirectionsControls:(UIToolbar *)controls;

@end