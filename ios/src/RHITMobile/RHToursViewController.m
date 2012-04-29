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


#define kSegueIdentifier @"ToursToTagsBasketSegue"


@interface RHToursViewController ()

@property (nonatomic, strong) NSNumber *duration;

@property (nonatomic, assign) BOOL onCampus;

@end


@implementation RHToursViewController

@synthesize durationLabel = durationLabel_;
@synthesize locationLabel = locationLabel_;
@synthesize locationControl = locationControl_;
@synthesize durationSlider = durationSlider_;
@synthesize duration = _duration;
@synthesize onCampus = _onCampus;

#pragma mark - View lifecycle

- (void)viewDidLoad
{
    [super viewDidLoad];
    self.duration = [NSNumber numberWithInt:30];
    self.onCampus = YES;
}

- (BOOL)shouldAutorotateToInterfaceOrientation:(UIInterfaceOrientation)interfaceOrientation
{
    // Return YES for supported orientations
    return (interfaceOrientation == UIInterfaceOrientationPortrait);
}

- (void)prepareForSegue:(UIStoryboardSegue *)segue sender:(id)sender
{
    if ([segue.identifier isEqualToString:kSegueIdentifier]) {
        RHTagsBasketViewController *tagsBasketController = segue.destinationViewController;
        tagsBasketController.duration = self.duration;
        tagsBasketController.onCampus = self.onCampus;
    }
}

- (void)tourTypeChanged:(id)sender
{
    UISwitch *tourTypeSwitch = sender;
    self.onCampus = tourTypeSwitch.isOn;
}

- (void)durationSliderMoved:(id)sender
{
    UISlider *slider = (UISlider *) sender;
    
    NSNumber *sliderValue = [NSNumber numberWithFloat:slider.value];
    self.duration = [NSNumber numberWithInt:sliderValue.intValue];
    self.durationLabel.text = [NSString stringWithFormat:@"%d", self.duration.intValue];
}

@end
