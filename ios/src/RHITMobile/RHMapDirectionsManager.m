//
//  RHMapDirectionsManager.m
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

#import "RHMapDirectionsManager.h"
#import "RHAppDelegate.h"
#import "RHConstants.h"
#import "RHLocation.h"
#import "RHMapViewController.h"
#import "RHPath.h"
#import "RHPathStep.h"
#import "RHSimplePointAnnotation.h"


@interface RHMapDirectionsManager () {
    @private
    UIView *directionsStatusBar_;
    UILabel *directionsStatus_;
    UIToolbar *directionsControls_;
    NSInteger currentStepIndex_;
    RHSimplePointAnnotation *currentStepAnnotation_;
    NSMutableArray *stepPins_;
}

- (void)showControls;

- (void)hideControls:(BOOL)animated;

- (void)showDirections;

- (void)hideDirections:(BOOL)animated;

@end


@implementation RHMapDirectionsManager

@synthesize mapViewController = mapViewController_;
@synthesize mapView = mapView_;
@synthesize currentPath = currentPath_;

- (void)displayPath:(RHPath *)path {
    if (self.currentlyDisiplaying) {
        [self hideDirections:NO];
        [self hideControls:NO];
    }
    
    // Quick check for a bad path
    if (path.steps.count < 1) {
        [[[UIAlertView alloc] initWithTitle:@"No path" message:@"Oops, we can't find a path to your desination. We're really sorry, but you're on your own." delegate:nil cancelButtonTitle:@"OK" otherButtonTitles:nil] show];
        return;
    }
    
    if (self.mapView.selectedAnnotations.count > 0) {
        [self.mapView deselectAnnotation:[self.mapView.selectedAnnotations objectAtIndex:0] animated:NO];
        [self.mapViewController clearAllDynamicMapArtifacts];
    }
    
    self.currentPath = path;
    
    RHPathStep *firstStep = [path.steps objectAtIndex:0];
    
    // Jump straight to the location without animating first
    [self.mapView setCenterCoordinate:firstStep.coordinate
                            zoomLevel:kRHDirectionsFocusZoomLevel
                             animated:NO];
    
    [self showControls];
    [self showDirections];
}


- (void)exitDirections:(id)sender {
    self.currentPath = nil;
    [self hideControls:YES];
    [self hideDirections:YES];
}

- (void)nextStep:(id)sender {
    if (currentStepIndex_ < self.currentPath.steps.count - 1) {
        currentStepIndex_ ++;
        RHPathStep *step = [self.currentPath.steps objectAtIndex:currentStepIndex_];
        
        while ((id)step.detail == [NSNull null] && currentStepIndex_ < self.currentPath.steps.count - 1 && !step.flagged) {
            currentStepIndex_ ++;
            step = [self.currentPath.steps objectAtIndex:currentStepIndex_];
        }
        
        if (currentStepIndex_ == self.currentPath.steps.count - 1 && self.currentPath.turnByTurn) {
            RHLocation *firstLocation = (RHLocation *) [self.managedObjectContext
                                                        objectWithID:step.locationID];
            directionsStatus_.text = [NSString stringWithFormat:@"Arrive at %@", firstLocation.name];
        } else if ((id) step.detail == [NSNull null] && step.flagged) {
            RHLocation *thisLocation = (RHLocation *) [self.managedObjectContext
                                                        objectWithID:step.locationID];
            directionsStatus_.text = [NSString stringWithFormat:@"Visit %@", thisLocation.name];
        } else {
            directionsStatus_.text = step.detail;
        }

        currentStepAnnotation_.coordinate = step.coordinate;
        
        if (step.flagged && (id) step.locationID != [NSNull null]) {
            RHLocation *thisLocation = (RHLocation *) [self.managedObjectContext objectWithID:step.locationID];
            [self.mapViewController focusMapViewToLocation:thisLocation];
        } else {
            [self.mapView setCenterCoordinate:step.coordinate animated:YES];
        }
    }
}

- (void)previousStep:(id)sender {
    if (currentStepIndex_ > 0) {
        currentStepIndex_ --;
        RHPathStep *step = [self.currentPath.steps objectAtIndex:currentStepIndex_];
        
        while ((id)step.detail == [NSNull null] && currentStepIndex_ > 0 && !step.flagged) {
            currentStepIndex_ --;
            step = [self.currentPath.steps objectAtIndex:currentStepIndex_];
        }
        
        if (currentStepIndex_ == 0 && self.currentPath.turnByTurn) {
            RHLocation *firstLocation = (RHLocation *) [self.managedObjectContext
                                                        objectWithID:step.locationID];
            directionsStatus_.text = [NSString stringWithFormat:@"Depart %@", firstLocation.name];
        } else if ((id) step.detail == [NSNull null] && step.flagged) {
            RHLocation *thisLocation = (RHLocation *) [self.managedObjectContext
                                                       objectWithID:step.locationID];
            directionsStatus_.text = [NSString stringWithFormat:@"Visit %@", thisLocation.name];
        } else {
            directionsStatus_.text = step.detail;
        }

        currentStepAnnotation_.coordinate = step.coordinate;
        
        if (step.flagged && (id) step.locationID != [NSNull null]) {
            RHLocation *thisLocation = (RHLocation *) [self.managedObjectContext objectWithID:step.locationID];
            [self.mapViewController focusMapViewToLocation:thisLocation];
        } else {
            [self.mapView setCenterCoordinate:step.coordinate animated:YES];
        }
    }
}

#pragma mark - Property Methods

- (BOOL)currentlyDisiplaying {
    return self.currentPath != nil;
}

- (NSManagedObjectContext *)managedObjectContext {
    return [(RHAppDelegate *) [[UIApplication sharedApplication] delegate] managedObjectContext];
}

#pragma mark - Private Methods

- (void)showControls {
    // Create status bar view
    directionsStatusBar_ = [[UIView alloc] initWithFrame:CGRectMake(0, -50, 320, 50)];
    directionsStatusBar_.backgroundColor = [[UIColor blackColor] colorWithAlphaComponent:0.7];
    
    directionsStatus_ = [[UILabel alloc] initWithFrame:CGRectMake(5, 5, 310, 40)];
    directionsStatus_.backgroundColor = [UIColor clearColor];
    directionsStatus_.textColor = [UIColor whiteColor];
    
    directionsStatus_.numberOfLines = 2;
    directionsStatus_.textAlignment = UITextAlignmentCenter;
    
    RHPathStep *firstStep = [self.currentPath.steps objectAtIndex:0];
    if ((id)firstStep.detail == [NSNull null] || firstStep.detail.length < 1) {
        RHLocation *firstLocation = (RHLocation *) [self.managedObjectContext
                                                    objectWithID:firstStep.locationID];
        directionsStatus_.text = [NSString stringWithFormat:@"Depart %@", firstLocation.name];
    } else {
        directionsStatus_.text = firstStep.detail;
    }
    
    [directionsStatusBar_ addSubview:directionsStatus_];
    
    // Create control views
    directionsControls_ = [[UIToolbar alloc]
                           initWithFrame:CGRectMake(0, self.mapView.frame.size.height, 320, 44)];
    
    directionsControls_.tintColor = [UIColor blackColor];
    
    UIBarButtonItem *prevItem = [[UIBarButtonItem alloc] initWithTitle:@"Prev"
                                                                 style:UIBarButtonItemStyleBordered 
                                                                target:self
                                                                action:@selector(previousStep:)];
    
    UIBarButtonItem *nextItem = [[UIBarButtonItem alloc] initWithTitle:@"Next"
                                                                 style:UIBarButtonItemStyleBordered 
                                                                target:self
                                                                action:@selector(nextStep:)];
    
    UIBarButtonItem *spaceItem = [[UIBarButtonItem alloc] initWithBarButtonSystemItem:UIBarButtonSystemItemFlexibleSpace target:nil action:nil];
    
    UIBarButtonItem *cancelItem = [[UIBarButtonItem alloc] initWithTitle:@"Exit Directions" style:UIBarButtonItemStyleBordered target:self action:@selector(exitDirections:)];
    
    directionsControls_.items = [NSArray arrayWithObjects:prevItem, nextItem,
                                 spaceItem, cancelItem, nil];
    
    // Add new views as subviews of map view controller
    [self.mapViewController.view addSubview:directionsStatusBar_];
    [self.mapViewController.view addSubview:directionsControls_];
    
    // Animate the entrance of the new views onto the screen
    [UIView beginAnimations:nil context:NULL];
    [UIView setAnimationDelay:0.75];
    [UIView setAnimationDuration:.25];
    
    CGRect frame = directionsStatusBar_.frame;
    frame.origin.y = 0;
    directionsStatusBar_.frame = frame;
    
    CGRect mapFrame = self.mapView.frame;
    mapFrame.size.height = mapFrame.size.height - 40;
    self.mapView.frame = mapFrame;
    
    CGRect toolbarFrame = directionsControls_.frame;
    toolbarFrame.origin.y = self.mapView.frame.size.height;
    directionsControls_.frame = toolbarFrame;
    
    [UIView commitAnimations];
}

- (void)hideControls:(BOOL)animated {
    if (animated) {
        [UIView beginAnimations:nil context:NULL];
        [UIView setAnimationDuration:0.25];
    }

    CGRect mapFrame = self.mapView.frame;
    mapFrame.size.height = mapFrame.size.height + 40;
    self.mapView.frame = mapFrame;
    
    CGRect frame = directionsStatusBar_.frame;
    frame.origin.y = -50;
    directionsStatusBar_.frame = frame;
    
    CGRect toolbarFrame = directionsControls_.frame;
    toolbarFrame.origin.y = self.mapView.frame.size.height;
    directionsControls_.frame = toolbarFrame;
    
    if (animated) {
        [UIView commitAnimations];
    }
    
    self.currentPath = nil;
}

- (void)showDirections {
    RHPathStep *firstStep = [self.currentPath.steps objectAtIndex:0];
    CLLocationCoordinate2D coords[self.currentPath.steps.count];
    
    stepPins_ = [NSMutableArray arrayWithCapacity:self.currentPath.steps.count];
    
    RHPathStep *last = [self.currentPath.steps objectAtIndex:(self.currentPath.steps.count - 1)];
    
    RHSimplePointAnnotation *lastAnnotation = [[RHSimplePointAnnotation alloc] init];
    lastAnnotation.coordinate = last.coordinate;
    lastAnnotation.color = RHSimplePointAnnotationColorRed;
    [self.mapView addAnnotation:lastAnnotation];
    
    [stepPins_ addObject:lastAnnotation];
    
    currentStepAnnotation_ = [[RHSimplePointAnnotation alloc] init];
    currentStepAnnotation_.coordinate = firstStep.coordinate;
    currentStepAnnotation_.color = RHSimplePointAnnotationColorGreen;
    [self.mapView addAnnotation:currentStepAnnotation_];
    
    
    for (RHPathStep *lineItem in self.currentPath.steps) {
        coords[[self.currentPath.steps indexOfObject:lineItem]] = lineItem.coordinate;
        
        if (lineItem.flagged) {
            RHSimplePointAnnotation *annotation = [[RHSimplePointAnnotation alloc] init];
            annotation.coordinate = lineItem.coordinate;
            annotation.color = RHSimplePointAnnotationColorBlue;
            [self.mapView addAnnotation:annotation];
            
            [stepPins_ addObject:annotation];
        }
    }
    
    if (self.currentPath.turnByTurn) {
        MKPolyline *line = [MKPolyline polylineWithCoordinates:coords
                                                         count:self.currentPath.steps.count];
        
        [self.mapView addOverlay:line];
    }

    currentStepIndex_ = 0;
    
    if (!self.currentPath.turnByTurn) {
        RHPathStep *step = [self.currentPath.steps objectAtIndex:0];
        RHLocation *thisLocation = (RHLocation *) [self.managedObjectContext objectWithID:step.locationID];
        [self.mapViewController focusMapViewToLocation:thisLocation];
    }
}

- (void)hideDirections:(BOOL)animated {
    [self.mapView removeOverlays:self.mapView.overlays];
    [self.mapView removeAnnotation:currentStepAnnotation_];
    [self.mapView removeAnnotations:stepPins_];
}

@end
