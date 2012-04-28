//
//  RHLocationSearchViewController.m
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

#import "RHLocationSearchViewController.h"
#import "RHAppDelegate.h"
#import "RHLocation.h"
#import "RHLocationDetailViewController.h"
#import "RHLocationSearchRequest.h"


#define kLocationCellIdentifier @"LocationSearchLocationCell"
#define kMoreCellIdentifier @"LocationSearchMoreCell"

#define kSegueIdentifier @"LocationSearchToDetailSegue"


@interface RHLocationSearchViewController () {
@private
    BOOL _searching;
    BOOL _searchTermChanged;
    BOOL _firstAppearance;
}

@property (nonatomic, strong) IBOutlet UITableView *tableView;

@property (nonatomic, strong) IBOutlet UISearchBar *searchBar;

@property (nonatomic, strong) NSArray *localResults;

@property (nonatomic, strong) NSArray *serverResults;

@property (nonatomic, readonly) NSManagedObjectContext *managedObjectContext;

- (void)performLocalSearch;

- (void)performServerSearch;

@end


@implementation RHLocationSearchViewController

@synthesize tableView = _tableView;
@synthesize searchBar = _searchBar;
@synthesize localResults = _localResults;
@synthesize serverResults = _serverResults;

#pragma mark - View Lifecycle

- (void)viewDidLoad
{
    _searching = NO;
    _searchTermChanged = NO;
    _firstAppearance = YES;
}

- (void)viewDidAppear:(BOOL)animated
{
    if (_firstAppearance) {
        [self.searchBar becomeFirstResponder];
        _firstAppearance = NO;
    }
}

- (BOOL)shouldAutorotateToInterfaceOrientation:(UIInterfaceOrientation)interfaceOrientation
{
    return (interfaceOrientation == UIInterfaceOrientationPortrait);
}

- (void)prepareForSegue:(UIStoryboardSegue *)segue sender:(id)sender
{
    if ([segue.identifier isEqualToString:kSegueIdentifier]) {
        RHLocationDetailViewController *locationDetail = segue.destinationViewController;
        NSIndexPath *indexPath = self.tableView.indexPathForSelectedRow;
        locationDetail.location = indexPath.row < self.localResults.count ? [self.localResults objectAtIndex:indexPath.row] : [self.serverResults objectAtIndex:(indexPath.row - self.localResults.count)];
    }
}

#pragma mark - Private Methods

- (NSManagedObjectContext *)managedObjectContext
{
    return [(RHAppDelegate *)[[UIApplication sharedApplication] delegate] managedObjectContext];
}

- (void)performLocalSearch
{
    NSString *searchTerm = self.searchBar.text;
    
    NSFetchRequest *fetchRequst = [NSFetchRequest fetchRequestWithEntityName:kRHLocationEntityName];
    fetchRequst.predicate = [NSPredicate predicateWithFormat:@"name BEGINSWITH[cd] %@", searchTerm];
    
    self.localResults = [self.managedObjectContext executeFetchRequest:fetchRequst error:nil];
    
    [self.tableView reloadData];
}

- (void)performServerSearch
{
    if (_searching) {
        return;
    }
    
    NSString *searchTerm = self.searchBar.text;
    
    _searching = YES;
    _searchTermChanged = NO;
    
    // Just in case the search bar is still up, resign the first responder
    [self.searchBar resignFirstResponder];
    
    // Hide the "More Results..." cell
    int moreCellIndex = self.localResults.count + self.serverResults.count;
    NSIndexPath *moreCellPath = [NSIndexPath indexPathForRow:moreCellIndex inSection:0];
    [self.tableView deleteRowsAtIndexPaths:[NSArray arrayWithObject:moreCellPath]withRowAnimation:UITableViewRowAnimationAutomatic];
    
    // Set the title to "Searching"
    self.navigationItem.title = @"Searching...";
    
    // Put an activity indicator in the navigation bar
    UIActivityIndicatorView *activityIndicator = [[UIActivityIndicatorView alloc] initWithActivityIndicatorStyle:UIActivityIndicatorViewStyleGray];
    [activityIndicator startAnimating];
    self.navigationItem.rightBarButtonItem = [[UIBarButtonItem alloc] initWithCustomView:activityIndicator];
    
    [RHLocationSearchRequest makeLocationSearchRequestWithSearchTerm:searchTerm limitToDepartable:NO successBlock:^(NSArray *results) {
        
        _searching = NO;
        
        NSMutableArray *potentialResults = [NSMutableArray arrayWithArray:results];
        
        // Filter the data
        for (RHLocation *location in results) {
            if ([self.localResults containsObject:location]) {
                [potentialResults removeObject:location];
            }
        }
        
        // Clear search display
        self.navigationItem.title = [NSString stringWithFormat:@"\"%@\"", searchTerm];
        self.navigationItem.rightBarButtonItem = nil;
        
        self.serverResults = potentialResults;
        
        // Display the results cooly
        if (self.serverResults.count > 0) {
            NSMutableArray *indexPaths = [NSMutableArray arrayWithCapacity:self.serverResults.count];
            
            for (int i = self.localResults.count; i < self.localResults.count + self.serverResults.count; i ++) {
                [indexPaths addObject:[NSIndexPath indexPathForRow:i inSection:0]];
            }
            
            [self.tableView insertRowsAtIndexPaths:indexPaths withRowAnimation:UITableViewRowAnimationAutomatic];
        }
        
    } failureBlock:^(NSError *error) {
        
        _searching = NO;
        
        // Show an alert
        [[[UIAlertView alloc] initWithTitle:@"Error" message:@"Something went wrong with your search. We're really sorry." delegate:nil cancelButtonTitle:@"OK" otherButtonTitles:nil] show];
        
        // Clear search display
        self.navigationItem.title = @"Search Locations";
        self.navigationItem.rightBarButtonItem = nil;
        
    }];
}

#pragma mark - Table View Delegate Methods

- (void)tableView:(UITableView *)tableView didSelectRowAtIndexPath:(NSIndexPath *)indexPath
{
    [tableView deselectRowAtIndexPath:indexPath animated:YES];
    
    if (indexPath.row == self.localResults.count + self.serverResults.count) {
        [self performServerSearch];
    }
}

#pragma mark - Table View Data Source Methods

- (NSInteger)tableView:(UITableView *)tableView numberOfRowsInSection:(NSInteger)section
{
    int numResults = self.localResults.count + self.serverResults.count;
    
    return !_searching && _searchTermChanged ? numResults + 1 : numResults;
}

- (UITableViewCell *)tableView:(UITableView *)tableView
         cellForRowAtIndexPath:(NSIndexPath *)indexPath
{
    UITableViewCell *cell;
    
    int totalResults = self.localResults.count + self.serverResults.count;
    
    if (indexPath.row == totalResults) {
        cell = [tableView dequeueReusableCellWithIdentifier:kMoreCellIdentifier];
    } else {
        cell = [tableView dequeueReusableCellWithIdentifier:kLocationCellIdentifier];
        
        RHLocation *location = indexPath.row < self.localResults.count ? [self.localResults objectAtIndex:indexPath.row] : [self.serverResults objectAtIndex:(indexPath.row - self.localResults.count)];
        
        cell.textLabel.text = location.name;
        cell.detailTextLabel.text = location.heirarchy;
    }
    
    return cell;
}

#pragma mark - Search Bar Delegate Methods

- (void)searchBarSearchButtonClicked:(UISearchBar *)searchBar
{
    [self performServerSearch];
}

- (void)searchBar:(UISearchBar *)searchBar textDidChange:(NSString *)searchText
{
    _searchTermChanged = YES;
    
    self.serverResults = nil;
    
    if (searchText.length > 0) {
        [self performLocalSearch];
    } else {
        self.localResults = nil;
        _searchTermChanged = NO;
        [self.tableView reloadData];
    }
}

- (void)searchBarCancelButtonClicked:(UISearchBar *)searchBar
{
    [searchBar resignFirstResponder];
}

@end
