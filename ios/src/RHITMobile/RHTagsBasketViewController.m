//
//  RHTagsBasketViewController.m
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

#import "RHTagsBasketViewController.h"
#import "RHAppDelegate.h"
#import "RHMapViewController.h"
#import "RHMapDirectionsManager.h"
#import "RHPathRequest.h"
#import "RHTagSelectionViewController.h"
#import "RHTourTagCategory.h"
#import "RHTourTag.h"

#define kTagCellReuseIdentifier @"TagCell"
#define kAddCellReuseIdentifier @"AddCell"


@implementation RHTagsBasketViewController

@synthesize tags = tags_;
@synthesize unusedTags = unusedTags_;
@synthesize tableView = tableView_;
@synthesize isEditing = isEditing_;
@synthesize isBuilding = isBuilding_;
@synthesize duration = _duration;

- (id)initWithNibName:(NSString *)nibNameOrNil bundle:(NSBundle *)nibBundleOrNil {
    self = [super initWithNibName:nibNameOrNil bundle:nibBundleOrNil];
    if (self) {
        self.tags = [NSMutableArray arrayWithCapacity:15];
    }
    return self;
}

- (NSManagedObjectContext *)managedObjectContext {
    return [((RHAppDelegate *) [[UIApplication sharedApplication] delegate]) managedObjectContext];
}

- (void)didReceiveMemoryWarning {
    // Releases the view if it doesn't have a superview.
    [super didReceiveMemoryWarning];
    
    // Release any cached data, images, etc that aren't in use.
}

#pragma mark - View lifecycle

- (void)viewDidLoad {
    [super viewDidLoad];
    
    self.isEditing = NO;
    
    self.navigationItem.rightBarButtonItem = [[UIBarButtonItem alloc] initWithTitle:@"Build Tour" style:UIBarButtonItemStyleDone target:self action:@selector(buildTour:)];
    
    // Load default tags
    NSFetchRequest *defaultRequest = [NSFetchRequest
                                      fetchRequestWithEntityName:kRHTourTagEntityName];
    NSArray *allTags = [self.managedObjectContext executeFetchRequest:defaultRequest error:nil];
    
    self.tags = [NSMutableArray arrayWithCapacity:allTags.count];
    self.unusedTags = [NSMutableArray arrayWithCapacity:allTags.count];
    
    for (RHTourTag *tag in allTags) {
        if (tag.isDefault.boolValue) {
            [self.tags addObject:tag];
        } else {
            [self.unusedTags addObject:tag];
        }
    }
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

- (NSString *)title {
    return @"Interests";
}

- (void)doneEditing:(id)sender {
    self.tableView.editing = NO;
    self.isEditing = NO;
    self.navigationItem.rightBarButtonItem = [[UIBarButtonItem alloc] initWithTitle:@"Build Tour" style:UIBarButtonItemStyleDone target:self action:@selector(buildTour:)];
    NSIndexPath *finalIndexPath = [NSIndexPath indexPathForRow:self.tags.count inSection:0];
    [self.tableView insertRowsAtIndexPaths:[NSArray arrayWithObject:finalIndexPath] withRowAnimation:UITableViewRowAnimationAutomatic];
}

- (void)buildTour:(id)sender {
    self.navigationItem.title = @"Building Tour";
    
    UIActivityIndicatorView *activityIndicator = [[UIActivityIndicatorView alloc] initWithActivityIndicatorStyle:UIActivityIndicatorViewStyleGray];
    self.navigationItem.rightBarButtonItem = [[UIBarButtonItem alloc] initWithCustomView:activityIndicator];
    [activityIndicator startAnimating];
    
    self.isBuilding = YES;
    NSIndexPath *finalIndexPath = [NSIndexPath indexPathForRow:self.tags.count inSection:0];
    [self.tableView deleteRowsAtIndexPaths:[NSArray arrayWithObject:finalIndexPath] withRowAnimation:UITableViewRowAnimationAutomatic];
    
    NSMutableArray *tagIds = [[NSMutableArray alloc] initWithCapacity:self.tags.count];
    
    for (RHTourTag *tag in self.tags) {
        [tagIds addObject:tag.serverIdentifier];
    }
    
    [RHPathRequest makeOnCampusTourRequestWithTagIds:tagIds
                                  fromLocationWithId:[NSNumber numberWithInt:111] // TODO
                                         forDuration:self.duration
                                        successBlock:^(RHPath *path) {
                                            [self didLoadPath:path];
                                        } failureBlock:^(NSError *error) {
                                            [[[UIAlertView alloc] initWithTitle:@"Error"
                                                                        message:@"Something went wrong building the tour. We're really sorry."
                                                                       delegate:nil
                                                              cancelButtonTitle:@"OK"
                                                              otherButtonTitles: nil] show];
                                            [self.navigationController popViewControllerAnimated:YES];
                                        }];
}

#pragma mark - UITableViewDelegate Methods

- (void)tableView:(UITableView *)tableView didSelectRowAtIndexPath:(NSIndexPath *)indexPath {
    [tableView deselectRowAtIndexPath:indexPath animated:YES];
    
    if (self.isBuilding) {
        return;
    }
    
    if (indexPath.row == self.tags.count && !tableView.editing) {
        // Find the root tag category
        NSFetchRequest *rootRequest = [NSFetchRequest fetchRequestWithEntityName:kRHTourTagCategoryEntityName];
        rootRequest.predicate = [NSPredicate predicateWithFormat:@"parent == nil"];
        NSArray *results = [self.managedObjectContext executeFetchRequest:rootRequest error:nil];
        
        // Create and display a tag selector view controller
        RHTagSelectionViewController *tagSelector = [[RHTagSelectionViewController alloc] initWithNibName:kRHTagSelectionViewControllerNibName bundle:nil];
        
        tagSelector.category = [results objectAtIndex:0];
        tagSelector.selectedTags = self.tags;
        tagSelector.deselectedTags = self.unusedTags;
        tagSelector.parentBasket = self;
        
        UINavigationController *navCon = [[UINavigationController alloc] initWithRootViewController:tagSelector];
        
        [self presentModalViewController:navCon animated:YES];
    } else {
        tableView.editing = YES;
        self.isEditing = YES;
        self.navigationItem.rightBarButtonItem = [[UIBarButtonItem alloc] initWithTitle:@"Done Editing" style:UIBarButtonItemStylePlain target:self action:@selector(doneEditing:)];
        NSIndexPath *finalIndexPath = [NSIndexPath indexPathForRow:self.tags.count inSection:0];
        [self.tableView deleteRowsAtIndexPaths:[NSArray arrayWithObject:finalIndexPath] withRowAnimation:UITableViewRowAnimationAutomatic];
    }
}

#pragma mark - UITableViewDataSource Methods

- (NSInteger)tableView:(UITableView *)tableView numberOfRowsInSection:(NSInteger)section {
    if (self.isEditing || self.isBuilding) {
        return self.tags.count;
    }
    return self.tags.count + 1;
}

- (UITableViewCell *)tableView:(UITableView *)tableView
         cellForRowAtIndexPath:(NSIndexPath *)indexPath {
    UITableViewCell *cell;
    if (indexPath.row == self.tags.count) {
        cell = [tableView dequeueReusableCellWithIdentifier:kAddCellReuseIdentifier];
        
        if (cell == nil) {
            cell = [[UITableViewCell alloc] initWithStyle:UITableViewCellStyleDefault 
                                          reuseIdentifier:kAddCellReuseIdentifier];
            cell.textLabel.textColor = [UIColor blueColor];
            cell.textLabel.text = @"Add Interests...";
            cell.textLabel.textAlignment = UITextAlignmentCenter;
        }
    } else {
        cell = [tableView dequeueReusableCellWithIdentifier:kTagCellReuseIdentifier];
        
        if (cell == nil) {
            cell = [[UITableViewCell alloc] initWithStyle:UITableViewCellStyleValue1
                                          reuseIdentifier:kTagCellReuseIdentifier];
            cell.selectionStyle = UITableViewCellSelectionStyleNone;
        }
        
        RHTourTag *tag = [self.tags objectAtIndex:indexPath.row];
        
        cell.textLabel.text = tag.name;
    }
    
    return cell;
}

- (BOOL)tableView:(UITableView *)tableView canEditRowAtIndexPath:(NSIndexPath *)indexPath {
    return !self.isBuilding && indexPath.row < self.tags.count;
}

- (BOOL)tableView:(UITableView *)tableView canMoveRowAtIndexPath:(NSIndexPath *)indexPath {
    return !self.isBuilding && indexPath.row < self.tags.count;
}

- (void)tableView:(UITableView *)tableView
commitEditingStyle:(UITableViewCellEditingStyle)editingStyle
forRowAtIndexPath:(NSIndexPath *)indexPath {
    [self.tags removeObjectAtIndex:indexPath.row];
    [tableView deleteRowsAtIndexPaths:[NSArray arrayWithObject:indexPath] withRowAnimation:UITableViewRowAnimationAutomatic];
    
    if (self.tags.count == 0 && self.isEditing) {
        [self doneEditing:nil];
    }
}

- (void)tableView:(UITableView *)tableView
moveRowAtIndexPath:(NSIndexPath *)sourceIndexPath
      toIndexPath:(NSIndexPath *)destinationIndexPath {
    id movedObject = [self.tags objectAtIndex:sourceIndexPath.row];
    [self.tags removeObjectAtIndex:sourceIndexPath.row];
    [self.tags insertObject:movedObject atIndex:destinationIndexPath.row];
}

- (void)didLoadPath:(RHPath *)path {
    [RHAppDelegate.instance.mapViewController.directionsManager displayPath:path];
    
    // Transition to view
    UITabBarController *tabBarController = RHAppDelegate.instance.tabBarController;
    
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
    
    [self.navigationController popToRootViewControllerAnimated:NO];
}

@end
