//
//  SearchViewController.m
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

#import "SearchViewController.h"


@implementation SearchViewController

@synthesize searchBar;
@synthesize searchType;
@synthesize tableView;

- (void)viewDidLoad {
    [super viewDidLoad];
    if (self.searchType == RHSearchViewControllerTypeLocation) {
        self.navigationItem.title = @"Search Locations";
    } else if (self.searchType == RHSearchViewControllerTypePeople) {
        self.navigationItem.title = @"Search People";
    } else {
        self.navigationItem.title = @"Search";
    }
}

- (void)viewDidAppear:(BOOL)animated {
    [self.searchBar becomeFirstResponder];
    [super viewDidAppear:animated];
}


- (BOOL)shouldAutorotateToInterfaceOrientation:(UIInterfaceOrientation)interfaceOrientation {
    return (interfaceOrientation == UIInterfaceOrientationPortrait);
}

- (void)didReceiveMemoryWarning {
    [super didReceiveMemoryWarning];
    
    // Release any cached data, images, etc. that aren't in use.
}

- (void)viewDidUnload {
    [super viewDidUnload];
    self.searchBar = nil;
}

#pragma mark -
#pragma mark UISearchBarDelegate Methods

- (void)searchBarCancelButtonClicked:(UISearchBar *)inSearchBar {
    [inSearchBar resignFirstResponder];
}

- (void)searchBarSearchButtonClicked:(UISearchBar *)inSearchBar {
    [inSearchBar resignFirstResponder];
    
    self.navigationItem.title = @"Searching...";
    UIActivityIndicatorView* activityIndicatorView = [[[UIActivityIndicatorView alloc] initWithFrame:CGRectMake(20, 0, 20, 20)] autorelease];
    [activityIndicatorView startAnimating];
    
    UIBarButtonItem *activityButtonItem = [[[UIBarButtonItem alloc] initWithCustomView:activityIndicatorView] autorelease];
    self.navigationItem.rightBarButtonItem = activityButtonItem;
}

@end