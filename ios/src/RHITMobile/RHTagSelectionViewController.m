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
#import "RHTourTag.h"
#import "RHTourTagCategory.h"

#define kCategoryReuseIdentifier @"CategoryCell"
#define kTagReuseIdentifier @"TagCell"


@implementation RHTagSelectionViewController

@synthesize category = category_;

- (id)initWithNibName:(NSString *)nibNameOrNil bundle:(NSBundle *)nibBundleOrNil {
    self = [super initWithNibName:nibNameOrNil bundle:nibBundleOrNil];
    if (self) {
        // Custom initialization
    }
    return self;
}

- (NSString *)title {
    if (self.category.name == nil || self.category.name.length == 0) {
        return @"Add Interest";
    }
    
    return self.category.name;
}

- (NSArray *)contents {
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

- (void)didReceiveMemoryWarning {
    // Releases the view if it doesn't have a superview.
    [super didReceiveMemoryWarning];
    
    // Release any cached data, images, etc that aren't in use.
}

- (void)cancelSelection:(id)sender {
    [self dismissModalViewControllerAnimated:YES];
}

#pragma mark - View lifecycle

- (void)viewDidLoad {
    [super viewDidLoad];
    
    self.navigationItem.rightBarButtonItem = [[UIBarButtonItem alloc] initWithTitle:@"Done" style:UIBarButtonItemStyleDone target:self action:@selector(cancelSelection:)];
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

#pragma mark - UITableViewDelegate Methods

- (void)tableView:(UITableView *)tableView didSelectRowAtIndexPath:(NSIndexPath *)indexPath {
    [tableView deselectRowAtIndexPath:indexPath animated:YES];
    
    RHTourItem *item = [self.contents objectAtIndex:indexPath.row];
    
    if ([item isKindOfClass:[RHTourTagCategory class]]) {
        RHTagSelectionViewController *nextViewController = [[RHTagSelectionViewController alloc] initWithNibName:kRHTagSelectionViewControllerNibName bundle:nil];
        nextViewController.category = (RHTourTagCategory *) item;
        [self.navigationController pushViewController:nextViewController animated:YES];
    }
}


#pragma mark - UITableViewDataSource Methods

- (NSInteger)tableView:(UITableView *)tableView numberOfRowsInSection:(NSInteger)section {
    return self.contents.count;
}

- (UITableViewCell *)tableView:(UITableView *)tableView
         cellForRowAtIndexPath:(NSIndexPath *)indexPath {
    UITableViewCell *cell;
    
    RHTourItem *item = [self.contents objectAtIndex:indexPath.row];
    
    if ([item isKindOfClass:[RHTourTagCategory class]]) {
        RHTourTagCategory *category = (RHTourTagCategory *) item;
        
        cell = [tableView dequeueReusableCellWithIdentifier:kCategoryReuseIdentifier];
        
        if (cell == nil) {
            cell = [[UITableViewCell alloc] initWithStyle:UITableViewCellStyleValue1 reuseIdentifier:kCategoryReuseIdentifier];
            cell.accessoryType = UITableViewCellAccessoryDisclosureIndicator;
        }
        
        cell.textLabel.text = category.name;
        
    } else if ([item isKindOfClass:[RHTourTag class]]) {
        RHTourTag *tag = (RHTourTag *) item;
        
        cell = [tableView dequeueReusableCellWithIdentifier:kTagReuseIdentifier];
        
        if (cell == nil) {
            cell = [[UITableViewCell alloc] initWithStyle:UITableViewCellStyleValue1 reuseIdentifier:kTagReuseIdentifier];
            cell.textLabel.textColor = [UIColor blueColor];
        }
        
        cell.textLabel.text = tag.name;
    }
    
    return cell;
}

@end
