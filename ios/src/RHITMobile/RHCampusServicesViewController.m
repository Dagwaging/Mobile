//
//  RHCampusServicesViewController.m
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

#import "RHCampusServicesViewController.h"
#import "RHCampusServicesLoader.h"
#import "RHServiceCategory.h"
#import "RHServiceLink.h"
#import "RHAppDelegate.h"
#import "RHWebViewController.h"

#define kCategorySegueIdentifier @"CampusServicesToCampusServicesSegue"
#define kLinkSegueIdentifier @"CampusServicesToWebViewSegue"

#define kCategoryCellIdentifier @"CampusServiceCategoryCell"
#define kLinkCellIdentifier @"CampusServiceLinkCell"


@interface RHCampusServicesViewController ()

@property (nonatomic, readonly) NSManagedObjectContext *managedObjectContext;

- (void)loadRootServiceCategories;

@end


@implementation RHCampusServicesViewController

@synthesize serviceItems = serviceItems_;

#pragma mark - View Lifecycle

- (void)viewDidLoad {
    [super viewDidLoad];
    
    if ([self.navigationController.viewControllers objectAtIndex:0] == self) {
        
        // Initialization for root campus services controller
        [self loadRootServiceCategories];
        
        [RHCampusServicesLoader.instance addDelegate:self];
    }
}

- (BOOL)shouldAutorotateToInterfaceOrientation:(UIInterfaceOrientation)interfaceOrientation {
    return (interfaceOrientation == UIInterfaceOrientationPortrait);
}

- (void)prepareForSegue:(UIStoryboardSegue *)segue sender:(id)sender
{
    if ([segue.identifier isEqualToString:kCategorySegueIdentifier]) {
        
        RHServiceCategory *category = [self.serviceItems objectAtIndex:[[self.tableView indexPathForSelectedRow] row]];
        
        RHCampusServicesViewController *servicesViewController = segue.destinationViewController;
        servicesViewController.serviceItems = category.sortedContents;
        servicesViewController.title = category.name;
        
    } else if ([segue.identifier isEqualToString:kLinkSegueIdentifier]) {
        
        RHServiceLink *link = [self.serviceItems objectAtIndex:[[self.tableView indexPathForSelectedRow] row]];
        
        RHWebViewController *webViewController = segue.destinationViewController;
        webViewController.url = [NSURL URLWithString:link.url];
        webViewController.title = link.name;
        
    }
}

#pragma mark - UITableViewDataSource Methods

- (NSInteger)tableView:(UITableView *)tableView
 numberOfRowsInSection:(NSInteger)section {
    return self.serviceItems.count;
}

- (UITableViewCell *)tableView:(UITableView *)tableView
         cellForRowAtIndexPath:(NSIndexPath *)indexPath {
    
    RHServiceItem *serviceItem = [self.serviceItems objectAtIndex:indexPath.row];
    
    UITableViewCell *cell;
    
    if ([serviceItem isKindOfClass:[RHServiceCategory class]]) {
        cell = [self.tableView dequeueReusableCellWithIdentifier:kCategoryCellIdentifier];
    } else {
        cell = [self.tableView dequeueReusableCellWithIdentifier:kLinkCellIdentifier];
        cell.detailTextLabel.text = [(RHServiceLink *) serviceItem url];
    }
    
    cell.textLabel.text = serviceItem.name;
    
    return cell;
}

- (void)loadRootServiceCategories {
    NSFetchRequest *fetchRequest = [NSFetchRequest fetchRequestWithEntityName:kRHServiceItemEntityName];
    
    NSPredicate *predicate = [NSPredicate predicateWithFormat:@"parent == nil"];
    fetchRequest.predicate = predicate;
    
    fetchRequest.sortDescriptors = [NSArray arrayWithObject:[NSSortDescriptor sortDescriptorWithKey:@"name" ascending:YES]];
    
    self.serviceItems = [self.managedObjectContext
                         executeFetchRequest:fetchRequest
                         error:nil];
    
    [self.tableView reloadData];
}

#pragma mark - Loader Delegate Methods

- (void)loaderDidUpdateUnderlyingData
{
    [self.navigationController popToRootViewControllerAnimated:NO];
    [self loadRootServiceCategories];
}

#pragma mark - Private Methods

- (NSManagedObjectContext *)managedObjectContext{
    return [(RHAppDelegate *)[[UIApplication sharedApplication] delegate] managedObjectContext];
}

@end
