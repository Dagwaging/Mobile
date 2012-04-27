//
//  RHMapViewController.h
//  Rose-Hulman Mobile
//
//  Copyright 2012 Rose-Hulman Institute of Technology
// 
//  Licensed under the Apache License, Version 2.0 (the "License");
//  you may not use this file except in compliance with the License.
//  You may obtain a copy of the License at
//
//      http://www.apache.org/licenses/LICENSE-2.0
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
#import "RHAnnotationViewDelegate.h"
#import "RHLoader.h"

#define kRHMapViewControllerNibName @"RHMapViewController"


@class RHRestHandler;
@class RHAnnotation;
@class RHLocation;
@class RHSimplePointAnnotation;
@class RHPath;
@class RHMapDirectionsManager;

/// \ingroup views
/// View controller for the map portion of the application.
@interface RHMapViewController : UIViewController
<MKMapViewDelegate, RHAnnotationViewDelegate, RHLoaderDelegate> {
    @private
    NSArray *currentDirections_;
    NSUInteger currentDirectionIndex_;
    RHSimplePointAnnotation *currentDirectionAnnotation_;
    UILabel *directionsStatus_;
    UIView *directionsStatusBar_;
    UIToolbar *directionsControls_;
    NSMutableArray *directionsPins_;
}

+ (id)instance;

/// Map view that is visible to the user.
@property (nonatomic, strong) IBOutlet MKMapView *mapView;

/// Core Data fetched results controller.
@property (nonatomic, strong) NSFetchedResultsController *fetchedResultsController;

/// Core Data managed object context. 
@property (nonatomic, strong) NSManagedObjectContext *managedObjectContext;

@property (nonatomic, strong) IBOutlet RHMapDirectionsManager *directionsManager;

@property (nonatomic, strong) NSMutableArray *quickListAnnotations;

@property (nonatomic, strong) NSArray *temporaryAnnotations;

@property (nonatomic, strong) IBOutlet UIView *backgroundView;

@property (nonatomic, strong) IBOutlet UILabel *directionsLabel;

@property (nonatomic, strong) IBOutlet UIPickerView *directionsPicker;

@property (nonatomic, assign) BOOL displayingDirections;

- (void)focusMapViewToTemporaryAnnotation:(RHAnnotation *)annotation;

- (void)focusMapViewToLocation:(RHLocation *)location;

- (IBAction)displaySearch:(id)sender;

- (IBAction)discloseLocationDetails:(id)sender;

- (IBAction)nextDirection:(id)sender;

- (IBAction)prevDirection:(id)sender;

- (IBAction)exitDirections:(id)sender;

- (void)displayPath:(MKPolyline *)path;

- (void)displayDirections:(RHPath *)directions;

@end