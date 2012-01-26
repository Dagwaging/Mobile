//
//  InfoViewController.m
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

#import "InfoViewController.h"
#import "RHServiceCategory.h"
#import "RHServiceLink.h"
#import "RHITMobileAppDelegate.h"
#import "WebViewController.h"

#define kLoadingReuseIdentifier @"LoadingCell"
#define kReuseIdentifier @"ServiceItemCell"

@implementation InfoViewController

@synthesize serviceItems = serviceItems_;
@synthesize fetchingData = fetchingData_;
@synthesize tableView = tableView_;

- (id)initWithNibName:(NSString *)nibNameOrNil
               bundle:(NSBundle *)nibBundleOrNil {
    self = [super initWithNibName:nibNameOrNil bundle:nibBundleOrNil];
    if (self) {
        self.fetchingData = YES;
    }
    return self;
}

- (void)didReceiveMemoryWarning {
    // Releases the view if it doesn't have a superview.
    [super didReceiveMemoryWarning];
    
    // Release any cached data, images, etc that aren't in use.
}

- (NSManagedObjectContext *)managedObjectContext {
    RHITMobileAppDelegate *appDelegate = (RHITMobileAppDelegate *)[[UIApplication sharedApplication] delegate];
    return appDelegate.managedObjectContext; 
}

#pragma mark - View lifecycle

- (void)viewDidLoad {
    [super viewDidLoad];
    if ([self.navigationController.viewControllers objectAtIndex:0] == self) {
        self.fetchingData = YES;
        [self loadRootServiceCategories];
    } else {
        self.fetchingData = NO;
    }
}

- (void)viewDidUnload {
    [super viewDidUnload];
    // Release any retained subviews of the main view.
    // e.g. self.myOutlet = nil;
}

- (BOOL)shouldAutorotateToInterfaceOrientation:(UIInterfaceOrientation)io {
    return (io == UIInterfaceOrientationPortrait);
}

#pragma mark - UITableViewDataSource Methods

- (NSInteger)tableView:(UITableView *)tableView
 numberOfRowsInSection:(NSInteger)section {
    if (self.fetchingData) {
        return 1;
    }
    
    return self.serviceItems.count;
}

- (UITableViewCell *)tableView:(UITableView *)tableView
         cellForRowAtIndexPath:(NSIndexPath *)indexPath {
    if (self.fetchingData) {
        UITableViewCell *cell = [tableView dequeueReusableCellWithIdentifier:kLoadingReuseIdentifier];
        
        if (cell == nil) {
            cell = [[UITableViewCell alloc] initWithStyle:UITableViewCellStyleValue1 reuseIdentifier:kLoadingReuseIdentifier];
            cell.textLabel.text = @"Loading...";
            UIActivityIndicatorView *activityIndicator = [[UIActivityIndicatorView alloc] initWithActivityIndicatorStyle:UIActivityIndicatorViewStyleGray];
            cell.accessoryView = activityIndicator;
            [activityIndicator startAnimating];
        }
        
        return cell;
    }
    
    UITableViewCell *cell = [tableView dequeueReusableCellWithIdentifier:kReuseIdentifier];
    
    if (cell == nil) {
        cell = [[UITableViewCell alloc] initWithStyle:UITableViewCellStyleValue1 reuseIdentifier:kReuseIdentifier];
        cell.accessoryType = UITableViewCellAccessoryDisclosureIndicator;
    }
    
    RHServiceItem *serviceItem = [self.serviceItems objectAtIndex:indexPath.row];

    cell.textLabel.text = serviceItem.name;
    
    return cell;
}

- (void)loadRootServiceCategories {    
    NSFetchRequest *fetchRequest = [NSFetchRequest fetchRequestWithEntityName:kRHServiceItemEntityName];
    
    NSPredicate *predicate = [NSPredicate predicateWithFormat:@"parent = NULL"];
    
    fetchRequest.predicate = predicate;
    
    NSError *error;
    self.serviceItems = [self.managedObjectContext
                         executeFetchRequest:fetchRequest
                         error:&error];
    
    if (error) {
        NSLog(@"Problem loading root campus services: %@", error);
    }
    
    self.fetchingData = NO;
    [self.tableView reloadData];
}

- (void)tableView:(UITableView *)tableView didSelectRowAtIndexPath:(NSIndexPath *)indexPath {
    [tableView deselectRowAtIndexPath:indexPath animated:YES];
    
    RHServiceItem *serviceItem = [self.serviceItems objectAtIndex:indexPath.row];
    
    if ([serviceItem isKindOfClass:[RHServiceCategory class]]) {
        RHServiceCategory *category = (RHServiceCategory *) serviceItem;
        
        InfoViewController *nextViewController = [[InfoViewController alloc]
                                                  initWithNibName:@"InfoView"
                                                  bundle:nil];
        nextViewController.serviceItems = category.contents.allObjects;
        nextViewController.title = category.name;
        [self.navigationController pushViewController:nextViewController
                                             animated:YES];
    } else if ([serviceItem isKindOfClass:[RHServiceLink class]]) {
        RHServiceLink *link = (RHServiceLink *) serviceItem;
        
        WebViewController *webViewController = [[WebViewController alloc] initWithNibName:@"WebView" bundle:nil];
        webViewController.url = [NSURL URLWithString:link.url];
        [self.navigationController pushViewController:webViewController animated:YES];
    }
}

@end
