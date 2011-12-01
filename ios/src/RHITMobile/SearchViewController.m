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
#import "RHRemoteHandler.h"
#import "RHLocation.h"
#import "LocationDetailViewController.h"


@implementation SearchViewController

@synthesize searchBar;
@synthesize searchType;
@synthesize tableView;
@synthesize remoteHandler;
@synthesize context;

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
    if (results_ == nil) {
        [self.searchBar becomeFirstResponder];
    }
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

#pragma mark - UISearchBarDelegate Methods

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
    
    [self.remoteHandler searchForLocations:self.searchBar.text searchViewController:self];
}

#pragma mark - UITableViewDataSource Methods

- (NSInteger)numberOfSectionsInTableView:(UITableView *)tableView {
    return 1;
}

- (NSInteger)tableView:(UITableView *)tableView numberOfRowsInSection:(NSInteger)section {
    if (results_ == nil) {
        return 0;
    }
    
    return MAX(results_.count, 1);
}

- (UITableViewCell *)tableView:(UITableView *)inTableView
         cellForRowAtIndexPath:(NSIndexPath *)indexPath {
    if (results_.count == 0) {
        UITableViewCell *cell = [inTableView dequeueReusableCellWithIdentifier:@"NoResultsCell"];
        
        if (cell == nil) {
            cell = [[[UITableViewCell alloc] initWithStyle:UITableViewCellStyleDefault reuseIdentifier:@"NoResultsCell"] autorelease];
            cell.selectionStyle = UITableViewCellSelectionStyleNone;
            cell.textLabel.textAlignment = UITextAlignmentCenter;
            cell.textLabel.text = @"No results found";
        } 

        
        return cell;
    }
    
    RHLocation *location = [results_ objectAtIndex:indexPath.row];
    
    UITableViewCell *cell = [inTableView dequeueReusableCellWithIdentifier:@"SearchResultCell"];
    
    if (cell == nil) {
        cell = [[[UITableViewCell alloc] initWithStyle:UITableViewCellStyleDefault reuseIdentifier:@"SearchResultCell"] autorelease];
        cell.accessoryType = UITableViewCellAccessoryDisclosureIndicator;
    }
    
    cell.textLabel.text = location.name;
    
    return cell;
}

- (void)didFindSearchResults:(NSArray *)searchResults {
    self.navigationItem.rightBarButtonItem = nil;
    self.navigationItem.title = [NSString stringWithFormat:@"\"%@\"", self.searchBar.text];
    
    results_ = [[NSMutableArray alloc] initWithCapacity:searchResults.count];
    
    for (NSManagedObjectID *objectID in searchResults) {
        RHLocation *location = (RHLocation *)[context objectWithID:objectID];
        [results_ addObject:location];
    }
    
    [self.tableView reloadData];
}

- (void)tableView:(UITableView *)inTableView
didSelectRowAtIndexPath:(NSIndexPath *)indexPath {
    if (results_.count == 0) {
        return;
    }
    
    RHLocation *location = [results_ objectAtIndex:indexPath.row];
    
    [inTableView deselectRowAtIndexPath:indexPath animated:YES];
    
    LocationDetailViewController *details = [[[LocationDetailViewController alloc] initWithNibName:@"LocationDetailView" bundle:nil] autorelease];
    details.location = location;
    
    [self.navigationController pushViewController:details animated:YES];
}

@end