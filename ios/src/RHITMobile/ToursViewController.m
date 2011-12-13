//
//  ToursViewController.m
//  RHITMobile
//
//  Created by Jimmy Theis on 12/12/11.
//  Copyright (c) 2011 Rose-Hulman Institute of Technology. All rights reserved.
//

#import <CoreLocation/CoreLocation.h>
#import "MKMapView+ZoomLevel.h"
#import "ToursViewController.h"
#import "RHWrappedCoordinate.h"
#import "RHITMobileAppDelegate.h"
#import "MapViewController.h"
#import "RHTourRequester.h"

@implementation ToursViewController

- (id)initWithNibName:(NSString *)nibNameOrNil bundle:(NSBundle *)nibBundleOrNil
{
    self = [super initWithNibName:nibNameOrNil bundle:nibBundleOrNil];
    if (self) {
        // Custom initialization
    }
    return self;
}

- (void)didReceiveMemoryWarning
{
    // Releases the view if it doesn't have a superview.
    [super didReceiveMemoryWarning];
    
    // Release any cached data, images, etc that aren't in use.
}

#pragma mark - View lifecycle

- (void)viewDidLoad
{
    [super viewDidLoad];
    // Do any additional setup after loading the view from its nib.
}

- (void)viewDidUnload
{
    [super viewDidUnload];
    // Release any retained subviews of the main view.
    // e.g. self.myOutlet = nil;
}

- (BOOL)shouldAutorotateToInterfaceOrientation:(UIInterfaceOrientation)interfaceOrientation
{
    // Return YES for supported orientations
    return (interfaceOrientation == UIInterfaceOrientationPortrait);
}

- (void)selectTour:(id)sender {
    currentDirectionsRequest_  = [[RHTourRequester alloc] initWithDelegate:self];
}

- (IBAction)didFinishLoadingTour:(NSArray *)directions {
    [currentDirectionsRequest_ release];
    
    [RHITMobileAppDelegate.instance.mapViewController displayDirections:directions];
    
    // Transition to view
    UITabBarController *tabBarController = RHITMobileAppDelegate.instance.tabBarController;
    
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

@end
