//
//  LocationDetailViewController.m
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


#import "LocationDetailViewController.h"
#import "RHLocation.h"
#import "RHLocationLink.h"

#define kAltNamesLabel @"Also Known As"
#define kAboutLabel @"About"
#define kLinksLabel @"More Info"
#define kParentLabel @"Where It Is"
#define kEnclosedLabel @"What's Inside"

#define kAltNameCellKey @"AltNameCell"
#define kLinkCellKey @"LinkCell"
#define kAboutCellKey @"AboutCell"
#define kParentCellKey @"ParentCell"
#define kEnclosedCellKey @"EnclosedCell"


@interface LocationDetailViewController ()

@property (nonatomic, retain) NSMutableArray *sections;

@end


@implementation LocationDetailViewController

@synthesize location = location_;
@synthesize links = links_;
@synthesize enclosedLocations;

@synthesize sections;

- (id)initWithNibName:(NSString *)nibNameOrNil
               bundle:(NSBundle *)nibBundleOrNil {
    self = [super initWithNibName:nibNameOrNil bundle:nibBundleOrNil];
    if (self) {
        self.sections = [[[NSMutableArray alloc] initWithCapacity:10]
                         autorelease];
    }
    return self;
}

- (void)didReceiveMemoryWarning {
    // Releases the view if it doesn't have a superview.
    [super didReceiveMemoryWarning];
    
    // Release any cached data, images, etc that aren't in use.
}

#pragma mark - View lifecycle

- (void)viewDidLoad {
    [super viewDidLoad];
    self.navigationItem.title = self.location.name;
    // Do any additional setup after loading the view from its nib.
}

- (void)viewDidUnload
{
    [super viewDidUnload];
    // Release any retained subviews of the main view.
    // e.g. self.myOutlet = nil;
}

- (BOOL)shouldAutorotateToInterfaceOrientation:(UIInterfaceOrientation)io
{
    // Return YES for supported orientations
    return (io == UIInterfaceOrientationPortrait);
}

#pragma mark - Property Methods

- (void)setLocation:(RHLocation *)location {
    if (location.alternateNames.count > 0) {
        [self.sections addObject:kAltNamesLabel];
    }
    
    if (location.quickDescription.length > 0) {
        [self.sections addObject:kAboutLabel];
    }
    
    if (location.links.count > 0) {
        [self.sections addObject:kLinksLabel];
        self.links = location.links.allObjects;
    }
    
    if (location.parent != nil) {
        [self.sections addObject:kParentLabel];
    }
    
    if (location.enclosedLocations.count > 0) {
        [self.sections addObject:kEnclosedLabel];
        self.enclosedLocations = [location.enclosedLocations.allObjects
                                  sortedArrayUsingComparator: ^(id l1, id l2) {
            return [[l1 name] caseInsensitiveCompare:[l2 name]];
        }];
    }
    
    location_ = location;
}

#pragma mark - UITableViewDelegate Methods


#pragma mark - UITableViewDataSource Methods


- (UITableViewCell *)tableView:(UITableView *)tableView
         cellForRowAtIndexPath:(NSIndexPath *)indexPath {
    
    NSString *sectionLabel = [self.sections objectAtIndex:[indexPath
                                                           indexAtPosition:0]];
    UITableViewCell *cell = nil;
    
    if (sectionLabel == kAboutLabel) {
        static NSString *cellIdentifier = kAboutCellKey;
        
        cell = [tableView dequeueReusableCellWithIdentifier:cellIdentifier];
        
        if (cell == nil) {
            cell = [[[UITableViewCell alloc]
                     initWithStyle:UITableViewCellStyleDefault
                     reuseIdentifier:cellIdentifier] autorelease];
            cell.textLabel.font = [UIFont systemFontOfSize:[UIFont
                                                            systemFontSize]];
            cell.textLabel.numberOfLines = 5;
            cell.selectionStyle = UITableViewCellSelectionStyleNone;
        }
        
        cell.textLabel.text = self.location.quickDescription;
        
    } else if (sectionLabel == kParentLabel) {
        static NSString *cellIdentifier = kParentCellKey;
        
        cell = [tableView dequeueReusableCellWithIdentifier:cellIdentifier];
        
        if (cell == nil) {
            cell = [[[UITableViewCell alloc]
                     initWithStyle:UITableViewCellStyleDefault
                     reuseIdentifier:cellIdentifier] autorelease];
            cell.selectionStyle = UITableViewCellSelectionStyleNone;
            cell.accessoryType = UITableViewCellAccessoryDisclosureIndicator;
        }
        
        cell.textLabel.text = self.location.parent.name;
        
    } else if (sectionLabel == kEnclosedLabel) {
        static NSString *cellIdentifier = kEnclosedCellKey;
        
        cell = [tableView dequeueReusableCellWithIdentifier:cellIdentifier];
        
        if (cell == nil) {
            cell = [[[UITableViewCell alloc]
                     initWithStyle:UITableViewCellStyleDefault
                     reuseIdentifier:cellIdentifier] autorelease];
            cell.accessoryType = UITableViewCellAccessoryDisclosureIndicator;
        }
        
        RHLocation *child = (RHLocation *) [self.enclosedLocations
                                            objectAtIndex:[indexPath
                                                           indexAtPosition:1]];
        
        cell.textLabel.text = child.name;
    } else if (sectionLabel == kAltNamesLabel) {
        static NSString *cellIdentifier = kAltNameCellKey;
        
        cell = [tableView dequeueReusableCellWithIdentifier:cellIdentifier];
        
        if (cell == nil) {
            cell = [[[UITableViewCell alloc]
                     initWithStyle:UITableViewCellStyleDefault
                     reuseIdentifier:cellIdentifier] autorelease];
            cell.textLabel.font = [UIFont systemFontOfSize:[UIFont
                                                            systemFontSize]];
            cell.textLabel.numberOfLines = 5;
            cell.selectionStyle = UITableViewCellSelectionStyleNone;
        }
        
        cell.textLabel.text = [self.location.alternateNames
                               objectAtIndex:[indexPath
                                              indexAtPosition:1]];
    } else if (sectionLabel == kLinksLabel) {
        static NSString *cellIdentifier = kLinkCellKey;
        
        cell = [tableView dequeueReusableCellWithIdentifier:cellIdentifier];
        
        if (cell == nil) {
            cell = [[[UITableViewCell alloc]
                     initWithStyle:UITableViewCellStyleDefault
                     reuseIdentifier:cellIdentifier] autorelease];
            cell.accessoryType = UITableViewCellAccessoryDetailDisclosureButton;
        }
        
        RHLocationLink *link = [self.links objectAtIndex:[indexPath indexAtPosition:1]];
        
        cell.textLabel.text = link.name;
    }
    
    return cell;
}

- (NSInteger)tableView:(UITableView *)tableView
 numberOfRowsInSection:(NSInteger)section {
    NSString *sectionLabel = [self.sections objectAtIndex:section];
    
    if (sectionLabel == kAboutLabel) {
        return 1;
    } else if (sectionLabel == kParentLabel) {
        return 1;
    } else if (sectionLabel == kEnclosedLabel) {
        return self.location.enclosedLocations.count;
    } else if (sectionLabel == kAltNamesLabel) {
        return self.location.alternateNames.count;
    } else if (sectionLabel == kLinksLabel) {
        return self.location.links.count;
    }
    
    return 0;
}

- (void)tableView:(UITableView *)inTableView
didSelectRowAtIndexPath:(NSIndexPath *)indexPath {
    NSString *sectionLabel = [self.sections objectAtIndex:[indexPath indexAtPosition:0]];
    
    if (sectionLabel == kEnclosedLabel) {
        RHLocation *child = [self.enclosedLocations objectAtIndex:[indexPath indexAtPosition:1]];
        
        [inTableView deselectRowAtIndexPath:indexPath animated:YES];
        
        LocationDetailViewController *detailViewController = [[[LocationDetailViewController alloc] initWithNibName:@"LocationDetailView" bundle:nil] autorelease];
        detailViewController.location = child;
        [self.navigationController pushViewController:detailViewController animated:YES];
    } else if (sectionLabel == kParentLabel) {
        [inTableView deselectRowAtIndexPath:indexPath animated:YES];
        
        LocationDetailViewController *detailViewController = [[[LocationDetailViewController alloc] initWithNibName:@"LocationDetailView" bundle:nil] autorelease];
        detailViewController.location = self.location.parent;
        [self.navigationController pushViewController:detailViewController animated:YES];
    } else if (sectionLabel == kLinksLabel) {
        [inTableView deselectRowAtIndexPath:indexPath animated:YES];
        
        RHLocationLink *link = [self.links objectAtIndex:[indexPath indexAtPosition:1]];
        [[UIApplication sharedApplication] openURL:[NSURL URLWithString:link.url]];
    }
}

- (void)tableView:(UITableView *)tableView accessoryButtonTappedForRowWithIndexPath:(NSIndexPath *)indexPath {
    NSString *sectionLabel = [self.sections objectAtIndex:[indexPath indexAtPosition:0]];
    
    if (sectionLabel == kLinksLabel) {
        RHLocationLink *link = [self.links objectAtIndex:[indexPath indexAtPosition:1]];
        [[UIApplication sharedApplication] openURL:[NSURL URLWithString:link.url]];
    }
}

- (NSInteger)numberOfSectionsInTableView:(UITableView *)tableView {
    return self.sections.count;
}

- (NSString *)tableView:(UITableView *)tableView
titleForHeaderInSection:(NSInteger)section {
    return [self.sections objectAtIndex:section];
}

@end
