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

/// \ingroup views
/// View controller for the map portion of the application.
@interface MapViewController : UIViewController
<MKMapViewDelegate, RHRemoteHandlerDelegate, RHAnnotationViewDelegate>

/// Map view that is visible to the user.
@property (nonatomic, retain) IBOutlet MKMapView *mapView;

@property (nonatomic, retain) IBOutlet UIToolbar *toolbar;

@property (nonatomic, retain) IBOutlet UIBarButtonItem *zoomInButton;

@property (nonatomic, retain) IBOutlet UIBarButtonItem *zoomOutButton;

@property (nonatomic, retain) IBOutlet UIBarButtonItem *placesButton;

@property (nonatomic, retain) IBOutlet UILabel *zoomLevelLabel;

@property (nonatomic, retain) IBOutlet UILabel *overlaysLabel;

@property (nonatomic, retain) IBOutlet UILabel *annotationsLabel;

/// Core Data fetched results controller.
@property (nonatomic, retain) NSFetchedResultsController *fetchedResultsController;

/// Core Data managed object context. 
@property (nonatomic, retain) NSManagedObjectContext *managedObjectContext;

/// Remote data handler.
@property (nonatomic, retain) RHRemoteHandler *remoteHandler;

/// Reload preference data, in case something has changed while the application
/// was running.
- (void)refreshPreferences;

- (IBAction)debugZoomIn:(id)sender;

- (IBAction)debugZoomOut:(id)sender;

- (IBAction)displayQuickList:(id)sender;

@end