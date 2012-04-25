//
//  RHLocationSelectorViewController.m
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

#import "RHLocationSelectorViewController.h"
#import "RHAppDelegate.h"
#import "RHLocation.h"
#import "RHMapDirectionsManager.h"
#import "RHMapViewController.h"
#import "RHPathRequest.h"

@implementation RHLocationSelectorViewController

@synthesize toLocation = toLocation_;

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

- (void)tableView:(UITableView *)inTableView
didSelectRowAtIndexPath:(NSIndexPath *)indexPath {
    [inTableView deselectRowAtIndexPath:indexPath animated:YES];
    
    if (self.searchResults.count == 0 && self.searchInitiated) {
        return;
    }
    
    if (indexPath.row >= self.searchResults.count && self.searchInitiated) {
        return;
    }
    
    if (indexPath.row >= self.searchResults.count) {
        return [self searchBarSearchButtonClicked:self.searchBar];
    }
    
    id result = [self.searchResults objectAtIndex:indexPath.row];
    id resultObject = [self objectFromResult:result];
    
    if (self.searchType == RHSearchViewControllerTypeLocation) {
        RHLocation *fromLocation = (RHLocation *) resultObject;
        
        [RHPathRequest makeDirectionsRequestFromLocationWithId:fromLocation.serverIdentifier
                                                    toLocation:self.toLocation.serverIdentifier
                                                  successBlock:^(RHPath *path) {
                                                      [self didLoadPath:path];
                                                  } failureBlock:^(NSError *error) {
                                                      [[[UIAlertView alloc] initWithTitle:@"Error"
                                                                                  message:@"Something went wrong when we tried to get you directions. We're really sorry."
                                                                                 delegate:nil
                                                                        cancelButtonTitle:@"OK"
                                                                        otherButtonTitles:nil] show];
                                                      [self.navigationController popViewControllerAnimated:YES];
                                                  }];
        
        self.navigationItem.title = @"Getting Directions";
        
        UIActivityIndicatorView *activityIndicator = [[UIActivityIndicatorView alloc] initWithActivityIndicatorStyle:UIActivityIndicatorViewStyleGray];
        [activityIndicator startAnimating];
        
        self.navigationItem.rightBarButtonItem = [[UIBarButtonItem alloc] initWithCustomView:activityIndicator];
    }
}

- (void)didLoadPath:(RHPath *)path {
    RHMapViewController *mapViewController = [(RHAppDelegate *)[[UIApplication sharedApplication] delegate] mapViewController];
    [self.navigationController popToRootViewControllerAnimated:YES];
    [mapViewController.directionsManager displayPath:path];
}

@end
