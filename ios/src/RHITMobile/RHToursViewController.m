//
//  RHToursViewController.m
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

#import <CoreLocation/CoreLocation.h>

#import "MKMapView+ZoomLevel.h"
#import "RHToursViewController.h"
#import "RHWrappedCoordinate.h"
#import "RHAppDelegate.h"
#import "RHMapViewController.h"
#import "RHMapDirectionsManager.h"
#import "RHTagsBasketViewController.h"
#import "RHPathRequest.h"

@implementation RHToursViewController

@synthesize durationLabel = durationLabel_;
@synthesize locationLabel = locationLabel_;
@synthesize locationControl = locationControl_;
@synthesize durationSlider = durationSlider_;
@synthesize duration = _duration;
@synthesize isBuilding = _isBuilding;

- (id)initWithNibName:(NSString *)nibNameOrNil bundle:(NSBundle *)nibBundleOrNil {
    self = [super initWithNibName:nibNameOrNil bundle:nibBundleOrNil];
    if (self) {
        // Custom initialization
    }
    return self;
}

- (void)didReceiveMemoryWarning {
    // Releases the view if it doesn't have a superview.
    [super didReceiveMemoryWarning];
    
    // Release any cached data, images, etc that aren't in use.
}

#pragma mark - View lifecycle

- (void)viewDidLoad {
    [super viewDidLoad];
    self.duration = [NSNumber numberWithInt:30];
    self.isBuilding = NO;
    // Do any additional setup after loading the view from its nib.
}

- (void)viewDidUnload {
    [super viewDidUnload];
    // Release any retained subviews of the main view.
    // e.g. self.myOutlet = nil;
}

- (BOOL)shouldAutorotateToInterfaceOrientation:(UIInterfaceOrientation)interfaceOrientation {
    // Return YES for supported orientations
    return (interfaceOrientation == UIInterfaceOrientationPortrait);
}

- (void)defaultTour:(id)sender {
    if (self.isBuilding) {
        return;
    }
    
    self.isBuilding = YES;
    self.navigationItem.title = @"Building Tour";
    
    UIActivityIndicatorView *activityIndicator = [[UIActivityIndicatorView alloc] initWithActivityIndicatorStyle:UIActivityIndicatorViewStyleGray];
    self.navigationItem.rightBarButtonItem = [[UIBarButtonItem alloc] initWithCustomView:activityIndicator];
    [activityIndicator startAnimating];
    
    NSArray *tagIds = [NSArray arrayWithObject:[NSNumber numberWithInt:1]];
    
    [RHPathRequest makeOnCampusTourRequestWithTagIds:tagIds
                                  fromLocationWithId:[NSNumber numberWithInt:111] // TODO
                                         forDuration:self.duration
                                        successBlock:^(RHPath *path) {
                                            self.isBuilding = NO;
                                            [self didLoadPath:path];
                                            self.navigationItem.title = @"Tours";
                                            self.navigationItem.rightBarButtonItem = nil;
                                        } failureBlock:^(NSError *error) {
                                            [[[UIAlertView alloc] initWithTitle:@"Error"
                                                                        message:@"Something went wrong building the tour. We're really sorry."
                                                                       delegate:nil
                                                              cancelButtonTitle:@"OK"
                                                              otherButtonTitles: nil] show];
                                            self.isBuilding = NO;
                                            self.navigationItem.title = @"Tours";
                                            self.navigationItem.rightBarButtonItem = nil;
                                        }];
}

- (void)customTour:(id)sender {
    if (self.isBuilding) {
        return;
    }
    
    RHTagsBasketViewController *basketViewController = [[RHTagsBasketViewController alloc] initWithNibName:kRHTagsBasketViewControllerNibname bundle:nil];
    basketViewController.duration = self.duration;
    [self.navigationController pushViewController:basketViewController animated:YES];
}

- (void)tourTypeChanged:(id)sender {
    UISegmentedControl *tourControl = (UISegmentedControl *) sender;
    
    if (tourControl.selectedSegmentIndex == 0) {
        // On campus
        // TODO: Fix
        self.durationSlider.enabled = NO;
        self.locationControl.enabled = NO;
    } else {
        // Virtual
        self.durationSlider.enabled = NO;
        self.locationControl.enabled = NO;
    }
}

- (void)locationTypeChanged:(id)sender {
    UISegmentedControl *locationControl = (UISegmentedControl *) sender;
    
    if (locationControl.selectedSegmentIndex == 0) {
        // Use GPS
        self.locationLabel.text = @"(Determined Automatically)";
    } else {
        // Choose location
        self.locationLabel.text = @"TODO";
    }
}

- (void)durationSliderMoved:(id)sender {
    UISlider *slider = (UISlider *) sender;
    
    NSNumber *sliderValue = [NSNumber numberWithFloat:slider.value];
    self.duration = [NSNumber numberWithInt:sliderValue.intValue];
    self.durationLabel.text = [NSString stringWithFormat:@"%d", self.duration.intValue];
}

- (void)didLoadPath:(RHPath *)path {
    [RHAppDelegate.instance.mapViewController.directionsManager displayPath:path];
    
    // Transition to view
    UITabBarController *tabBarController = RHAppDelegate.instance.tabBarController;
    
    UIView *fromView = tabBarController.selectedViewController.view;
    UIView *toView = [[tabBarController.viewControllers objectAtIndex:0] view];
    
    // Transition using a page curl.
    [UIView transitionFromView:fromView 
                        toView:toView 
                      duration:0.5 
                       options:UIViewAnimationOptionTransitionCurlDown
                    completion:^(BOOL finished) {
                        if (finished) {
                            tabBarController.selectedIndex = 0;
                        }
                    }];
    
    [self.navigationController popToRootViewControllerAnimated:NO];
}

@end
