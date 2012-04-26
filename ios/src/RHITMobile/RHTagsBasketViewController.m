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

#define kSegueIdentifier @"TagsBasketToTagSelectionSegue"
#define kTagCellReuseIdentifier @"TagsBasketTagCell"
#define kAddCellReuseIdentifier @"TagsBasketAddCell"

@interface RHTagsBasketViewController ()

@property (nonatomic, readonly) NSManagedObjectContext *managedObjectContext;

@property (nonatomic, strong) NSMutableArray *tags;

@property (nonatomic, strong) NSMutableArray *unusedTags;

@property (nonatomic, assign) BOOL isEditing;

@property (nonatomic, assign) BOOL isBuilding;

@end


@implementation RHTagsBasketViewController

@synthesize tags = tags_;
@synthesize unusedTags = unusedTags_;
@synthesize isEditing = isEditing_;
@synthesize isBuilding = isBuilding_;
@synthesize duration = _duration;
@synthesize useGPS = _useGPS;
@synthesize onCampus = _onCampus;

#pragma mark - View lifecycle

- (void)viewDidLoad
{
    [super viewDidLoad];
    
    self.tags = [NSMutableArray array];
    
    self.isEditing = NO;
    
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

- (void)viewWillAppear:(BOOL)animated
{
    [super viewWillAppear:animated];
    
    [self.tableView reloadData];
}

- (BOOL)shouldAutorotateToInterfaceOrientation:(UIInterfaceOrientation)interfaceOrientation
{
    // Return YES for supported orientations
    return (interfaceOrientation == UIInterfaceOrientationPortrait);
}

- (void)prepareForSegue:(UIStoryboardSegue *)segue sender:(id)sender
{
    if ([segue.identifier isEqualToString:kSegueIdentifier]) {
        UINavigationController *navCon = segue.destinationViewController;
        RHTagSelectionViewController *selectionController = [navCon.viewControllers objectAtIndex:0];
        
        NSFetchRequest *rootRequest = [NSFetchRequest fetchRequestWithEntityName:kRHTourTagCategoryEntityName];
        rootRequest.predicate = [NSPredicate predicateWithFormat:@"parent == nil"];
        NSArray *results = [self.managedObjectContext executeFetchRequest:rootRequest
                                                                    error:nil];
        
        selectionController.category = [results objectAtIndex:0];
        selectionController.selectedTags = self.tags;
        selectionController.deselectedTags = self.unusedTags;
    }
}

- (void)doneEditing:(id)sender
{
    self.tableView.editing = NO;
    self.isEditing = NO;
    
    self.navigationItem.rightBarButtonItem = [[UIBarButtonItem alloc] initWithTitle:@"Build Tour" style:UIBarButtonItemStyleDone target:self action:@selector(buildTour:)];
    
    NSIndexPath *finalIndexPath = [NSIndexPath indexPathForRow:self.tags.count inSection:0];
    
    [self.tableView insertRowsAtIndexPaths:[NSArray arrayWithObject:finalIndexPath] withRowAnimation:UITableViewRowAnimationAutomatic];
}

- (void)buildTour:(id)sender
{
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
                                            //[self didLoadPath:path];
                                            NSLog(@"Tour successful");
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

- (void)tableView:(UITableView *)tableView didSelectRowAtIndexPath:(NSIndexPath *)indexPath
{
    [self.tableView deselectRowAtIndexPath:indexPath animated:YES];
    
    if (self.isBuilding) {
        return;
    }
    
    if (indexPath.row < self.tags.count) {
        self.tableView.editing = YES;
        self.isEditing = YES;
        self.navigationItem.rightBarButtonItem = [[UIBarButtonItem alloc] initWithTitle:@"Done Editing" style:UIBarButtonItemStylePlain target:self action:@selector(doneEditing:)];
        NSIndexPath *finalIndexPath = [NSIndexPath indexPathForRow:self.tags.count inSection:0];
        [self.tableView deleteRowsAtIndexPaths:[NSArray arrayWithObject:finalIndexPath] withRowAnimation:UITableViewRowAnimationAutomatic];
    }
}

#pragma mark - UITableViewDataSource Methods

- (NSInteger)tableView:(UITableView *)tableView numberOfRowsInSection:(NSInteger)section
{
    if (self.isEditing || self.isBuilding) {
        return self.tags.count;
    }
    return self.tags.count + 1;
}

- (UITableViewCell *)tableView:(UITableView *)tableView
         cellForRowAtIndexPath:(NSIndexPath *)indexPath
{
    UITableViewCell *cell;
    if (indexPath.row == self.tags.count) {
        cell = [tableView dequeueReusableCellWithIdentifier:kAddCellReuseIdentifier];
    } else {
        cell = [tableView dequeueReusableCellWithIdentifier:kTagCellReuseIdentifier];
        
        RHTourTag *tag = [self.tags objectAtIndex:indexPath.row];
        cell.textLabel.text = tag.name;
    }
    
    return cell;
}

- (BOOL)tableView:(UITableView *)tableView canEditRowAtIndexPath:(NSIndexPath *)indexPath
{
    return !self.isBuilding && indexPath.row < self.tags.count;
}

- (BOOL)tableView:(UITableView *)tableView canMoveRowAtIndexPath:(NSIndexPath *)indexPath
{
    return !self.isBuilding && indexPath.row < self.tags.count;
}

- (void)tableView:(UITableView *)tableView
commitEditingStyle:(UITableViewCellEditingStyle)editingStyle
forRowAtIndexPath:(NSIndexPath *)indexPath {
    [self.tags removeObjectAtIndex:indexPath.row];
    [self.tableView deleteRowsAtIndexPaths:[NSArray arrayWithObject:indexPath] withRowAnimation:UITableViewRowAnimationAutomatic];
    
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

- (void)didLoadPath:(RHPath *)path
{
    // FIXME
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

- (NSManagedObjectContext *)managedObjectContext
{
    return [((RHAppDelegate *) [[UIApplication sharedApplication] delegate]) managedObjectContext];
}

@end
