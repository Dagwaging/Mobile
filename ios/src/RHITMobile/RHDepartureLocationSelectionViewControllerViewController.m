//
//  RHDepartureLocationSelectionViewControllerViewController.m
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

#import "RHDepartureLocationSelectionViewControllerViewController.h"
#import "RHLocation.h"
#import "RHLocationSearchRequest.h"


#define kGPSCellIdentifier @"DepartureUseGPSCell"
#define kDepartureLocationCellIdentifier @"DepartureLocationCell"
#define kMoreResultsCellIdentifier @"DepartureMoreCell"


@implementation RHDepartureLocationCell

@synthesize locationNameLabel = _locationNameLabel;
@synthesize locationDetailLabel = _locationDetailLabel;

@end


@interface RHDepartureLocationSelectionViewControllerViewController ()

- (IBAction)departLocation:(id)sender;

- (IBAction)departGPS:(id)sender;

- (IBAction)cancelSelection:(id)sender;

@end


@implementation RHDepartureLocationSelectionViewControllerViewController

@synthesize locationChosenBlock = _locationChosenBlock;
@synthesize gpsChosenBlock = _gpsChosenBlock;

#pragma mark - Private Methods

- (void)performLocalSearch
{
    NSString *searchTerm = self.searchBar.text;
    
    NSFetchRequest *fetchRequst = [NSFetchRequest fetchRequestWithEntityName:kRHLocationEntityName];
    fetchRequst.predicate = [NSPredicate predicateWithFormat:@"departable == YES AND name BEGINSWITH[cd] %@", searchTerm];
    
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
    int moreCellIndex = self.localResults.count + self.serverResults.count + 1;
    NSIndexPath *moreCellPath = [NSIndexPath indexPathForRow:moreCellIndex inSection:0];
    [self.tableView deleteRowsAtIndexPaths:[NSArray arrayWithObject:moreCellPath]withRowAnimation:UITableViewRowAnimationAutomatic];
    
    // Set the title to "Searching"
    self.navigationItem.title = @"Searching...";
    
    // Put an activity indicator in the navigation bar
    UIActivityIndicatorView *activityIndicator = [[UIActivityIndicatorView alloc] initWithActivityIndicatorStyle:UIActivityIndicatorViewStyleGray];
    [activityIndicator startAnimating];
    self.navigationItem.rightBarButtonItem = [[UIBarButtonItem alloc] initWithCustomView:activityIndicator];
    
    [RHLocationSearchRequest makeLocationSearchRequestWithSearchTerm:searchTerm limitToDepartable:YES successBlock:^(NSArray *results) {
        
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
                [indexPaths addObject:[NSIndexPath indexPathForRow:i + 1 inSection:0]];
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

- (void)departLocation:(id)sender
{
    // TODO: Execute stored block
}

- (void)departGPS:(id)sender
{
    // TODO: Execute stored block
}

- (void)cancelSelection:(id)sender
{
    [self dismissModalViewControllerAnimated:YES];
}

#pragma mark - Table View Delegate Methods

- (void)tableView:(UITableView *)tableView didSelectRowAtIndexPath:(NSIndexPath *)indexPath
{
    [tableView deselectRowAtIndexPath:indexPath animated:YES];
    
    if (indexPath.row == self.localResults.count + self.serverResults.count + 1) {
        [self performServerSearch];
    }
}

#pragma mark - Table View Data Source Methods

- (UITableViewCell *)tableView:(UITableView *)tableView
         cellForRowAtIndexPath:(NSIndexPath *)indexPath
{
    UITableViewCell *cell;
    
    int totalResults = self.localResults.count + self.serverResults.count;
    
    if (indexPath.row == 0) {
        cell = [tableView dequeueReusableCellWithIdentifier:kGPSCellIdentifier];
    } else if (indexPath.row == totalResults + 1) {
        cell = [tableView dequeueReusableCellWithIdentifier:kMoreResultsCellIdentifier];
    } else {
        cell = [tableView dequeueReusableCellWithIdentifier:kDepartureLocationCellIdentifier];
        
        RHDepartureLocationCell *departureCell = (RHDepartureLocationCell *) cell;
        
        RHLocation *location = indexPath.row < self.localResults.count + 1 ? [self.localResults objectAtIndex:indexPath.row - 1] : [self.serverResults objectAtIndex:(indexPath.row - self.localResults.count - 1)];
        
        departureCell.locationNameLabel.text = location.name;
        departureCell.locationDetailLabel.text = location.heirarchy;
    }
    
    return cell;
}

- (NSInteger)tableView:(UITableView *)tableView
 numberOfRowsInSection:(NSInteger)section
{
    return [super tableView:tableView numberOfRowsInSection:section] + 1;
}

@end
