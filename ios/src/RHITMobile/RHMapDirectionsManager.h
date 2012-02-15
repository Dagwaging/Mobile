//
//  RHMapDirectionsManager.h
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

#import <Foundation/Foundation.h>

#import "MKMapView+ZoomLevel.h"

@class RHMapViewController;
@class RHPath;


@interface RHMapDirectionsManager : NSObject

@property (nonatomic, strong) IBOutlet RHMapViewController *mapViewController;

@property (nonatomic, strong) IBOutlet MKMapView *mapView;

@property (nonatomic, strong) RHPath *currentPath;

@property (nonatomic, readonly) BOOL currentlyDisiplaying;

@property (nonatomic, readonly) NSManagedObjectContext *managedObjectContext;

- (void)displayPath:(RHPath *)path;

- (IBAction)exitDirections:(id)sender;

- (IBAction)nextStep:(id)sender;

- (IBAction)previousStep:(id)sender;

@end
