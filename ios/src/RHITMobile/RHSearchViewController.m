//
//  RHSearchViewController.m
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

#import "RHSearchViewController.h"
#import "RHRestHandler.h"
#import "RHLocation.h"
#import "RHLocationDetailViewController.h"
#import "RHAppDelegate.h"


@implementation RHSearchViewController

@synthesize searchBar;
@synthesize searchInitiated;
@synthesize currentAutocompleteTerm;
@synthesize searchType;
@synthesize tableView;
@synthesize remoteHandler;
@synthesize context;
@synthesize searchResults = searchResults_;

- (void)viewDidLoad {
    [super viewDidLoad];
    
    self.searchResults = [NSMutableArray array];
    
    if (self.searchType == RHSearchViewControllerTypeLocation) {
        self.navigationItem.title = @"Search Locations";
    } else if (self.searchType == RHSearchViewControllerTypePeople) {
        self.navigationItem.title = @"Search People";
    } else {
        self.navigationItem.title = @"Search";
    }
}

- (void)viewDidAppear:(BOOL)animated {
    if (!self.searchInitiated) {
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

- (NSDictionary *)autocompleteData {
    if (self.searchType == RHSearchViewControllerTypeLocation) {
        return RHAppDelegate.instance.locationNames;
    }
    
    return nil;
}

- (void)tryAutocomplete:(NSString *)searchTerm {
    if ([NSThread isMainThread]) {
        self.currentAutocompleteTerm = searchTerm;
        [self performSelectorInBackground:@selector(tryAutocomplete:)
                               withObject:searchTerm];
    } else {
        NSString *normalized = searchTerm.lowercaseString;
        NSMutableArray *results = [NSMutableArray arrayWithCapacity:self.autocompleteData.count];
        
        for (NSString *term in self.autocompleteData) {
            if ([term.lowercaseString hasPrefix:normalized]) {
                [results addObject:term];
            }
        }
        
        if ([self.currentAutocompleteTerm isEqual:searchTerm]) {
            self.searchResults = results;
            [self.tableView performSelectorOnMainThread:@selector(reloadData)
                                             withObject:nil
                                          waitUntilDone:NO];
        }
    }
}

#pragma mark - UISearchBarDelegate Methods

- (void)searchBarCancelButtonClicked:(UISearchBar *)inSearchBar {
    [inSearchBar resignFirstResponder];
}

- (void)searchBarSearchButtonClicked:(UISearchBar *)inSearchBar {
    [inSearchBar resignFirstResponder];
    self.searchInitiated = YES;
    
    self.navigationItem.title = @"Searching...";
    UIActivityIndicatorView* activityIndicatorView = [[UIActivityIndicatorView alloc] initWithFrame:CGRectMake(20, 0, 20, 20)];
    [activityIndicatorView startAnimating];
    
    UIBarButtonItem *activityButtonItem = [[UIBarButtonItem alloc] initWithCustomView:activityIndicatorView];
    self.navigationItem.rightBarButtonItem = activityButtonItem;
    
    [self.remoteHandler searchForLocations:self.searchBar.text searchViewController:self];
}

- (void)searchBar:(UISearchBar *)searchBar
    textDidChange:(NSString *)searchText {
    self.searchInitiated = NO;
    [self tryAutocomplete:searchText];
}

#pragma mark - UITableViewDataSource Methods

- (NSInteger)numberOfSectionsInTableView:(UITableView *)tableView {
    return 1;
}

- (NSInteger)tableView:(UITableView *)tableView numberOfRowsInSection:(NSInteger)section {
    if (self.searchBar.text.length == 0) {
        return 0;
    }
    
    return MAX(self.searchResults.count + (self.searchInitiated ? 0 : 1), 1);
}

- (UITableViewCell *)tableView:(UITableView *)inTableView
         cellForRowAtIndexPath:(NSIndexPath *)indexPath {
    
    UITableViewCell *cell = nil;
    
    if (self.searchResults.count == 0 && self.searchInitiated) {
        cell = [inTableView dequeueReusableCellWithIdentifier:@"NoResultsCell"];
        
        if (cell == nil) {
            cell = [[UITableViewCell alloc] initWithStyle:UITableViewCellStyleDefault reuseIdentifier:@"NoResultsCell"];
            cell.selectionStyle = UITableViewCellSelectionStyleNone;
            cell.textLabel.textAlignment = UITextAlignmentCenter;
            cell.textLabel.text = @"No results found";
        }
        
        return cell;
    } else if (!self.searchInitiated) {
        
        if (indexPath.row >= self.searchResults.count) {
            cell = [inTableView dequeueReusableCellWithIdentifier:@"AutocompleteMoreCell"];
            
            if (cell == nil) {
                cell = [[UITableViewCell alloc] initWithStyle:UITableViewCellStyleDefault reuseIdentifier:@"AutocompleteMoreCell"];
                cell.textLabel.textAlignment = UITextAlignmentCenter;
                cell.textLabel.textColor = [UIColor blueColor];
                cell.textLabel.text = @"More results...";
            }
            
            return cell;
        } 
        
        cell = [inTableView dequeueReusableCellWithIdentifier:@"AutocompleteResultCell"];
        
        if (cell == nil) {
            cell = [[UITableViewCell alloc] initWithStyle:UITableViewCellStyleDefault reuseIdentifier:@"AutocompleteResultCell"];
            cell.accessoryType = UITableViewCellAccessoryDisclosureIndicator;
        }
    } else {
        cell = [inTableView dequeueReusableCellWithIdentifier:@"SearchResultCell"];
        
        if (cell == nil) {
            cell = [[UITableViewCell alloc] initWithStyle:UITableViewCellStyleDefault reuseIdentifier:@"SearchResultCell"];
            cell.accessoryType = UITableViewCellAccessoryDisclosureIndicator;
        }
    }
    
    id result = [self.searchResults objectAtIndex:indexPath.row];
    id resultObject = [self objectFromResult:result];

    if (self.searchType == RHSearchViewControllerTypeLocation) {
        cell.textLabel.text = [resultObject name];
    } else {
        
    }
    
    return cell;
}

- (void)didFindSearchResults:(NSArray *)inSearchResults {
    self.navigationItem.rightBarButtonItem = nil;
    self.navigationItem.title = [NSString stringWithFormat:@"\"%@\"", self.searchBar.text];
    
    self.searchResults = [NSMutableArray arrayWithCapacity:inSearchResults.count];
    
    for (NSManagedObjectID *objectID in inSearchResults) {
        RHLocation *location = (RHLocation *)[context objectWithID:objectID];
        [self.searchResults addObject:location];
    }
    
    [self.tableView reloadData];
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
        RHLocationDetailViewController *details = [[RHLocationDetailViewController alloc] initWithNibName:kRHLocationDetailViewControllerNibName bundle:nil];
        details.location = (RHLocation *) resultObject;
        
        [self.navigationController pushViewController:details animated:YES];
    } else {
        
    }

}

- (id)objectFromResult:(id)result {
    if ([result isKindOfClass:[NSString class]]) {
        if ([result isKindOfClass:[NSString class]]) {
            NSManagedObjectID *objectID = [self.autocompleteData objectForKey:result];
            
            id resultObject = [self.context objectWithID:objectID];
            
            if (self.searchType == RHSearchViewControllerTypeLocation) {
                RHLocation *location = (RHLocation *) resultObject;
                return location;
            }
        }
    }
    return result;
}

@end