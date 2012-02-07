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
#import "RHTagSelectionViewController.h"
#import "RHTourTagCategory.h"
#import "RHTourTag.h"

#define kTagCellReuseIdentifier @"TagCell"
#define kAddCellReuseIdentifier @"AddCell"


@implementation RHTagsBasketViewController

@synthesize tags;
@synthesize tableView = tableView_;

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
    
    self.navigationItem.rightBarButtonItem = [[UIBarButtonItem alloc] initWithTitle:@"Build Tour" style:UIBarButtonItemStyleDone target:nil action:NULL];
    
    // Load default tags
    NSFetchRequest *defaultRequest = [NSFetchRequest fetchRequestWithEntityName:kRHTourTagEntityName];
    //defaultRequest.predicate = [NSPredicate predicateWithFormat:@"isDefault == YES"];
    self.tags = [NSMutableArray arrayWithArray:[self.managedObjectContext executeFetchRequest:defaultRequest error:nil]];
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

#pragma mark - UITableViewDelegate Methods

- (void)tableView:(UITableView *)tableView didSelectRowAtIndexPath:(NSIndexPath *)indexPath {
    [tableView deselectRowAtIndexPath:indexPath animated:YES];
    
    if (indexPath.row == self.tags.count && !tableView.editing) {
        // Find the root tag category
        NSFetchRequest *rootRequest = [NSFetchRequest fetchRequestWithEntityName:kRHTourTagCategoryEntityName];
        rootRequest.predicate = [NSPredicate predicateWithFormat:@"parent == nil"];
        NSArray *results = [self.managedObjectContext executeFetchRequest:rootRequest error:nil];
        
        // Create and display a tag selector view controller
        RHTagSelectionViewController *tagSelector = [[RHTagSelectionViewController alloc] initWithNibName:kRHTagSelectionViewControllerNibName bundle:nil];
        tagSelector.category = [results objectAtIndex:0];
        UINavigationController *navCon = [[UINavigationController alloc] initWithRootViewController:tagSelector];
        
        [self presentModalViewController:navCon animated:YES];
    } else {
        tableView.editing = YES;
        self.navigationItem.rightBarButtonItem = [[UIBarButtonItem alloc] initWithTitle:@"Done Editing" style:UIBarButtonItemStylePlain target:self action:@selector(doneEditing:)];
    }
}

- (void)doneEditing:(id)sender {
    self.tableView.editing = NO;
    self.navigationItem.rightBarButtonItem = [[UIBarButtonItem alloc] initWithTitle:@"Build Tour" style:UIBarButtonItemStyleDone target:nil action:NULL];
}

#pragma mark - UITableViewDataSource Methods

- (NSInteger)tableView:(UITableView *)tableView numberOfRowsInSection:(NSInteger)section {
    return tags.count + 1;
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
    return indexPath.row < self.tags.count;
}

- (BOOL)tableView:(UITableView *)tableView canMoveRowAtIndexPath:(NSIndexPath *)indexPath {
    return indexPath.row < self.tags.count;
}

- (void)tableView:(UITableView *)tableView
commitEditingStyle:(UITableViewCellEditingStyle)editingStyle
forRowAtIndexPath:(NSIndexPath *)indexPath {
    [self.tags removeObjectAtIndex:indexPath.row];
    [tableView deleteRowsAtIndexPaths:[NSArray arrayWithObject:indexPath] withRowAnimation:UITableViewRowAnimationAutomatic];
}

- (void)tableView:(UITableView *)tableView moveRowAtIndexPath:(NSIndexPath *)sourceIndexPath toIndexPath:(NSIndexPath *)destinationIndexPath {
    //TODO: Update data source to reflect the change
}

@end
