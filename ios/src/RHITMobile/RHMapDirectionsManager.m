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
#import "RHConstants.h"
#import "RHPath.h"
#import "RHPathStep.h"


@interface RHMapDirectionsManager () 

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
    if (path.steps.count > 1) {
        NSLog(@"Path is empty. Skipping display.");
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

#pragma mark - Property Methods

- (BOOL)currentlyDisiplaying {
    return self.currentPath != nil;
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

#pragma mark - Private Methods

- (void)showControls {
    
}

- (void)hideControls:(BOOL)animated {
    
}

- (void)showDirections {
    
}

- (void)hideDirections:(BOOL)animated {
    
}

@end
