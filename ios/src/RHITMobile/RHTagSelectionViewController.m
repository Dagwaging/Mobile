//
//  RHTagSelectionViewController.m
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

#import "RHTagSelectionViewController.h"
#import "RHTagsBasketViewController.h"
#import "RHTourTag.h"
#import "RHTourTagCategory.h"


#define kSegueIdentifier @"TagSelectionToTagSelectionSegue"
#define kCategoryReuseIdentifier @"TagSelectionCategoryCell"
#define kTagReuseIdentifier @"TagSelectionTagCell"


@implementation RHTagSelectionViewController

@synthesize category = category_;
@synthesize selectedTags = selectedTags_;
@synthesize deselectedTags = deselectedTags_;
@synthesize contents = contents_;

- (void)prepareForSegue:(UIStoryboardSegue *)segue sender:(id)sender
{
    if ([segue.identifier isEqualToString:kSegueIdentifier]) {
        RHTagSelectionViewController *nextController = segue.destinationViewController;
        nextController.category = [self.contents objectAtIndex:self.tableView.indexPathForSelectedRow.row];
        nextController.selectedTags = self.selectedTags;
        nextController.deselectedTags = self.deselectedTags;
    }
}

- (NSString *)title
{
    return self.category.name;
}

- (NSArray *)contents
{
    if (contents_ == nil) {
        contents_ = [[self.category.children allObjects] sortedArrayUsingComparator:^(id a, id b) {
            RHTourItem *itemA = (RHTourItem *) a;
            RHTourItem *itemB = (RHTourItem *) b;
            
            // Prefer tags to categories, then names
            if ([itemA isKindOfClass:[RHTourTag class]] && [itemB isKindOfClass:[RHTourTagCategory class]]) {
                return NSOrderedAscending;
            } else if ([itemA isKindOfClass:[RHTourTagCategory class]] && [itemB isKindOfClass:[RHTourTag class]]) {
                return NSOrderedDescending;
            } else {
                return [itemA.name compare:itemB.name];
            }
        }];
    }
    
    return contents_;
}

- (void)donePressed:(id)sender
{
    [self dismissModalViewControllerAnimated:YES];
}

#pragma mark - View lifecycle

- (BOOL)shouldAutorotateToInterfaceOrientation:(UIInterfaceOrientation)interfaceOrientation
{
    // Return YES for supported orientations
    return (interfaceOrientation == UIInterfaceOrientationPortrait);
}

#pragma mark - UITableViewDelegate Methods

- (void)tableView:(UITableView *)tableView didSelectRowAtIndexPath:(NSIndexPath *)indexPath
{
    [tableView deselectRowAtIndexPath:indexPath animated:YES];
    
    RHTourItem *item = [self.contents objectAtIndex:indexPath.row];
    
    if ([item isKindOfClass:[RHTourTag class]]) {
        UITableViewCell *currentCell = [tableView cellForRowAtIndexPath:indexPath];
        RHTourTag *tag = (RHTourTag *) item;
        if (currentCell.accessoryType == UITableViewCellAccessoryCheckmark) {
            [self.selectedTags removeObject:tag];
            [self.deselectedTags addObject:tag];
            currentCell.accessoryType = UITableViewCellAccessoryNone;
        } else {
            [self.deselectedTags removeObject:tag];
            [self.selectedTags addObject:tag];
            currentCell.accessoryType = UITableViewCellAccessoryCheckmark;
        }
    }
}

#pragma mark - UITableViewDataSource Methods

- (NSInteger)tableView:(UITableView *)tableView numberOfRowsInSection:(NSInteger)section
{
    return self.contents.count;
}

- (UITableViewCell *)tableView:(UITableView *)tableView
         cellForRowAtIndexPath:(NSIndexPath *)indexPath
{
    UITableViewCell *cell;
    
    RHTourItem *item = [self.contents objectAtIndex:indexPath.row];
    
    if ([item isKindOfClass:[RHTourTagCategory class]]) {
        RHTourTagCategory *category = (RHTourTagCategory *) item;
        
        cell = [tableView dequeueReusableCellWithIdentifier:kCategoryReuseIdentifier];
        cell.textLabel.text = category.name;
        
    } else if ([item isKindOfClass:[RHTourTag class]]) {
        RHTourTag *tag = (RHTourTag *) item;
        
        cell = [tableView dequeueReusableCellWithIdentifier:kTagReuseIdentifier];
        cell.textLabel.text = tag.name;
        if ([self.selectedTags containsObject:tag]) {
            cell.accessoryType = UITableViewCellAccessoryCheckmark;
        } else {
            cell.accessoryType = UITableViewCellAccessoryNone;
        }
    }
    
    return cell;
}

@end
