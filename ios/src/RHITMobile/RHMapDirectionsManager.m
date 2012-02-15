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


@interface RHMapDirectionsManager () {
    @private
    UIView *directionsStatusBar_;
    UILabel *directionsStatus_;
    UIToolbar *directionsControls_;
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
        NSLog(@"Path is empty. Skipping display.");
        return;
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
    
}

- (void)previousStep:(id)sender {
    
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
                                                                action:@selector(prevDirection:)];
    
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
    
}

- (void)hideDirections:(BOOL)animated {
    
}

@end
