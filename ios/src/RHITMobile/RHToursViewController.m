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
#import "RHTourRequester.h"
#import "RHTagsBasketViewController.h"

@implementation RHToursViewController

@synthesize durationLabel = durationLabel_;
@synthesize locationLabel = locationLabel_;
@synthesize locationControl = locationControl_;
@synthesize durationSlider = durationSlider_;

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
    
}

- (void)customTour:(id)sender {
    RHTagsBasketViewController *basketViewController = [[RHTagsBasketViewController alloc] initWithNibName:kRHTagsBasketViewControllerNibname bundle:nil];
    [self.navigationController pushViewController:basketViewController animated:YES];
}

- (void)tourTypeChanged:(id)sender {
    UISegmentedControl *tourControl = (UISegmentedControl *) sender;
    
    if (tourControl.selectedSegmentIndex == 0) {
        // On campus
        self.durationSlider.enabled = YES;
        self.locationControl.enabled = YES;
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
    self.durationLabel.text = [NSString stringWithFormat:@"%d", sliderValue.intValue];
}

- (IBAction)didFinishLoadingTour:(NSArray *)directions {
    
    //[RHAppDelegate.instance.mapViewController displayDirections:directions];
    
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
}

- (void)didLoadPath:(RHPath *)path {
    
}

@end
