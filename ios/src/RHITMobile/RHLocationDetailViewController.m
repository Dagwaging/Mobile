//
//  RHLocationDetailViewController.m
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


#import "RHLocationDetailViewController.h"
#import "RHLocation.h"
#import "RHLocationLink.h"
#import "RHAppDelegate.h"
#import "RHLocationsLoader.h"
#import "RHMapViewController.h"
#import "RHWebViewController.h"
#import "RHWrappedCoordinate.h"
#import "RHMapDirectionsManager.h"


#define kAltNamesLabel @"Also Known As"
#define kAboutLabel @"About"
#define kLinksLabel @"More Info"
#define kParentLabel @"Where It Is"
#define kEnclosedLabel @"What's Inside"

#define kTextCell @"LocationDetailTextCell"
#define kLocationCell @"LocationDetailLocationCell"
#define kLinkCell @"LocationDetailLinkCell"
#define kLoadingCell @"LocationDetailLoadingCell"

#define kDirectionsSegue @"LocationDetailToDepartureSegue"
#define kLocationSegue @"LocationDetailToLocationDetailSegue"
#define kWebSegue @"LocationDetailToWebSegue"


@interface RHLocationDetailViewController ()

@property (nonatomic, strong) NSMutableArray *sections;

@end


@implementation RHLocationDetailViewController

@synthesize location = location_;
@synthesize links = links_;
@synthesize enclosedLocations;
@synthesize tableView;

@synthesize sections;

- (id)initWithNibName:(NSString *)nibNameOrNil
               bundle:(NSBundle *)nibBundleOrNil {
    self = [super initWithNibName:nibNameOrNil bundle:nibBundleOrNil];
    if (self) {
        self.sections = [[NSMutableArray alloc] initWithCapacity:10];
    }
    return self;
}

- (void)didReceiveMemoryWarning {
    // Releases the view if it doesn't have a superview.
    [super didReceiveMemoryWarning];
    
    // Release any cached data, images, etc that aren't in use.
}

- (IBAction)displayCurrentLocationOnMap:(id)sender {
    [self.navigationController popToRootViewControllerAnimated:YES];
    [RHMapViewController.instance focusMapViewToLocation:self.location];
}

- (IBAction)getDirectionsToCurrentLocation:(id)sender
{
    [self performSegueWithIdentifier:kDirectionsSegue sender:self];
}

#pragma mark - View lifecycle

- (void)viewDidLoad {
    [super viewDidLoad];
    self.navigationItem.title = self.location.name;
    // Do any additional setup after loading the view from its nib.
}

- (void)viewDidAppear:(BOOL)animated {
    self.location = self.location;
    [self.tableView reloadData];
    
    if (self.location.retrievalStatus != RHLocationRetrievalStatusFull) {
        [RHLocationsLoader.instance registerCallbackForLocationWithId:self.location.serverIdentifier callback:^(void) {
            [self setLocation:self.location];
        }];
    }
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

- (void)prepareForSegue:(UIStoryboardSegue *)segue sender:(id)sender
{
    if ([segue.identifier isEqualToString:kLocationSegue]) {
        
        RHLocationDetailViewController *detailController = segue.destinationViewController;
        
        if (self.tableView.indexPathForSelectedRow.section == [self.sections indexOfObject:kParentLabel]) {
            detailController.location = self.location.parent;
        } else {
            detailController.location = [self.enclosedLocations objectAtIndex:self.tableView.indexPathForSelectedRow.row];
        }
        
    } else if ([segue.identifier isEqualToString:kWebSegue]) {
        
        RHLocationLink *link = [self.links objectAtIndex:self.tableView.indexPathForSelectedRow.row];
        
        NSURL *url = [NSURL URLWithString:link.url];
        
        RHWebViewController *webViewController = segue.destinationViewController;
        
        webViewController.url = url;
        webViewController.title = link.name;
        
    } else if ([segue.identifier isEqualToString:kDirectionsSegue]) {
        
    }
}

#pragma mark - Property Methods

- (void)setLocation:(RHLocation *)location {
    // Describe the type of entity we'd like to retrieve
    NSEntityDescription *entityDescription;
    entityDescription = [NSEntityDescription
                         entityForName:kRHLocationEntityName
                         inManagedObjectContext:location.managedObjectContext];
    
    NSFetchRequest *request = [[NSFetchRequest alloc] init];
    [request setEntity:entityDescription];
    
    NSPredicate *predicate = [NSPredicate predicateWithFormat:
                              @"serverIdentifier == %@", location.serverIdentifier];
    [request setPredicate:predicate];
    
    NSArray *results = [location.managedObjectContext executeFetchRequest:request
                                                                    error:nil];
    
    if (results.count > 0) {
        location = [results objectAtIndex:0];
    }
    
    // Initialize sections
    self.sections = [[NSMutableArray alloc] initWithCapacity:10];
    
    // Populate sections
    if (location.alternateNames.count > 0) {
        [self.sections addObject:kAltNamesLabel];
    }
    
    if (location.quickDescription.length > 0) {
        [self.sections addObject:kAboutLabel];
    }
    
    [self.sections addObject:kParentLabel];
    
    if (location.links.count > 0) {
        [self.sections addObject:kLinksLabel];
        self.links = location.links.allObjects;
    }
    
    if (location.enclosedLocations.count > 0) {
        [self.sections addObject:kEnclosedLabel];
        self.enclosedLocations = [location.enclosedLocations.allObjects
                                  sortedArrayUsingComparator: ^(id l1, id l2) {
                                      return [[l1 name] caseInsensitiveCompare:[l2 name]];
                                  }];
    }
    
    [self.tableView reloadData];
    
    location_ = location;
}

#pragma mark - UITableViewDelegate Methods

- (UIView *)tableView:(UITableView *)tableView
viewForFooterInSection:(NSInteger)section {
    NSString *sectionLabel = [self.sections objectAtIndex:section];
    
    if (sectionLabel == kParentLabel) {
        UIView *parentView = [[UIView alloc] initWithFrame:CGRectZero];
        parentView.backgroundColor = [UIColor clearColor];
        
        UIButton *mapButton = [UIButton buttonWithType:UIButtonTypeRoundedRect];
        mapButton.frame = CGRectMake(10.0, 10.0, 145.0, 44.0);
        [mapButton addTarget:self
                      action:@selector(displayCurrentLocationOnMap:)
            forControlEvents:UIControlEventTouchUpInside];
        
        [mapButton setTitle:@"Go to Map" forState:UIControlStateNormal];
        
        UIButton *directionsButton = [UIButton buttonWithType:UIButtonTypeRoundedRect];
        directionsButton.frame = CGRectMake(165.0, 10.0, 145.0, 44.0);
        [directionsButton addTarget:self
                             action:@selector(getDirectionsToCurrentLocation:)
                   forControlEvents:UIControlEventTouchUpInside];
        
        [directionsButton setTitle:@"Get Directions" forState:UIControlStateNormal];
        
        [parentView addSubview:mapButton];
        [parentView addSubview:directionsButton];
        return parentView;
    }
    
    return nil;
}

- (CGFloat)tableView:(UITableView *)tableView
heightForFooterInSection:(NSInteger)section {
    NSString *sectionLabel = [self.sections objectAtIndex:section];
    
    if (sectionLabel == kParentLabel) {
        return 64;
    }
    
    return 0;
}

- (CGFloat)tableView:(UITableView *)tableView
heightForRowAtIndexPath:(NSIndexPath *)indexPath {
    NSString *sectionLabel = [self.sections objectAtIndex:[indexPath
                                                           indexAtPosition:0]];
    
    if (sectionLabel == kAboutLabel) {
        CGSize maximumLabelSize = CGSizeMake(290, 9999);
        
        CGSize expectedLabelSize = [self.location.quickDescription
                                    sizeWithFont:[UIFont systemFontOfSize:UIFont.systemFontSize]
                                    constrainedToSize:maximumLabelSize 
                                    lineBreakMode:UILineBreakModeTailTruncation]; 
        
        return expectedLabelSize.height + 20;
    }
    
    return 44;
}

#pragma mark - UITableViewDataSource Methods

- (UITableViewCell *)tableView:(UITableView *)inTableView
         cellForRowAtIndexPath:(NSIndexPath *)indexPath {
    
    NSString *sectionLabel = [self.sections objectAtIndex:[indexPath section]];
    UITableViewCell *cell = nil;
    
    if (sectionLabel == kAboutLabel) {
        
        cell = [inTableView dequeueReusableCellWithIdentifier:kTextCell];
        cell.textLabel.text = self.location.quickDescription;
        
    } else if (sectionLabel == kParentLabel) {
        
        cell = [inTableView dequeueReusableCellWithIdentifier:kLocationCell];
        cell.textLabel.text = self.location.parent.name;
        
    } else if (sectionLabel == kEnclosedLabel &&
               self.location.retrievalStatus == RHLocationRetrievalStatusFull) {
        
        cell = [inTableView dequeueReusableCellWithIdentifier:kLocationCell];
        RHLocation *child = [self.enclosedLocations objectAtIndex:indexPath.row];
        cell.textLabel.text = child.name;
        
    } else if (sectionLabel == kEnclosedLabel &&
               self.location.retrievalStatus != RHLocationRetrievalStatusFull) {
        
        cell = [inTableView dequeueReusableCellWithIdentifier:kLoadingCell];
        
    } else if (sectionLabel == kAltNamesLabel) {
        
        cell = [inTableView dequeueReusableCellWithIdentifier:kTextCell];
        cell.textLabel.text = [self.location.alternateNames objectAtIndex:indexPath.row];
        
    } else if (sectionLabel == kLinksLabel) {
        
        cell = [inTableView dequeueReusableCellWithIdentifier:kLinkCell];
        RHLocationLink *link = [self.links objectAtIndex:indexPath.row];
        cell.textLabel.text = link.name;
        cell.detailTextLabel.text = link.url;
    }
    
    return cell;
}

- (NSInteger)tableView:(UITableView *)tableView
 numberOfRowsInSection:(NSInteger)section {
    NSString *sectionLabel = [self.sections objectAtIndex:section];
    
    if (sectionLabel == kAboutLabel) {
        return 1;
    } else if (sectionLabel == kParentLabel) {
        return self.location.parent == nil ? 0 : 1;
    } else if (sectionLabel == kEnclosedLabel) {
        if (self.location.retrievalStatus != RHLocationRetrievalStatusFull) {
            return 1;
        }
        return self.location.enclosedLocations.count;
    } else if (sectionLabel == kAltNamesLabel) {
        return self.location.alternateNames.count;
    } else if (sectionLabel == kLinksLabel) {
        return self.location.links.count;
    }
    
    return 0;
}

- (void)tableView:(UITableView *)inTableView
didSelectRowAtIndexPath:(NSIndexPath *)indexPath
{
    [tableView deselectRowAtIndexPath:indexPath animated:YES];
}

- (NSInteger)numberOfSectionsInTableView:(UITableView *)tableView {
    return self.sections.count;
}

- (NSString *)tableView:(UITableView *)tableView
titleForHeaderInSection:(NSInteger)section {
    return [self.sections objectAtIndex:section];
}

# pragma mark - RHDirectionsRequestDelegate Methods

- (void)didFinishLoadingDirections:(NSArray *)directions {
    [self.navigationController popToRootViewControllerAnimated:YES];
    // TODO display directions
}

@end
