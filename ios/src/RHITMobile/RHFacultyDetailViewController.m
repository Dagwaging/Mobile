//
//  RHFacultyDetailViewController.m
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

#import "RHFacultyDetailViewController.h"
#import "RHDirectoryRequestsWrapper.h"
#import "RHPerson.h"
#import "RHUser.h"

@interface RHFacultyDetailViewController () {
    @private
    BOOL _loaded;
}

@property (nonatomic, strong) IBOutlet UILabel *departmentLabel;

@property (nonatomic, strong) IBOutlet UILabel *campusMailboxLabel;

@property (nonatomic, strong) IBOutlet UILabel *emailAddressLabel;

@property (nonatomic, strong) IBOutlet UILabel *telephoneNumberLabel;

@property (nonatomic, strong) IBOutlet UILabel *locationLabel;

@property (nonatomic, strong) RHPerson *person;

@end

@implementation RHFacultyDetailViewController

@synthesize user = _user;
@synthesize departmentLabel = _departmentLabel;
@synthesize campusMailboxLabel = _campusMailboxLabel;
@synthesize emailAddressLabel = _emailAddressLabel;
@synthesize telephoneNumberLabel = _telephoneNumberLabel;
@synthesize locationLabel = _locationLabel;
@synthesize person = _person;

- (id)initWithStyle:(UITableViewStyle)style
{
    self = [super initWithStyle:style];
    if (self) {
        // Custom initialization
    }
    return self;
}

- (void)viewDidLoad
{
    [super viewDidLoad];
    
    [RHDirectoryRequestsWrapper makePersonDetailRequestForUser:self.user successBlock:^(RHPerson *person) {
        
        self.person = person;
        
        self.navigationItem.title = person.userAccount.fullName;
        self.navigationItem.rightBarButtonItem = nil;
        
        self.departmentLabel.text = person.department;
        self.campusMailboxLabel.text = person.campusMailbox.description;
        self.emailAddressLabel.text = person.emailAddress;
        self.telephoneNumberLabel.text = person.phoneNumber;
        self.locationLabel.text = person.location;
        
        [self.tableView reloadData];
        
    } failureBlock:^(NSError *error) {
        [self.navigationController popViewControllerAnimated:YES];
    }];
    
    // Uncomment the following line to preserve selection between presentations.
    // self.clearsSelectionOnViewWillAppear = NO;
    
    // Uncomment the following line to display an Edit button in the navigation bar for this view controller.
    // self.navigationItem.rightBarButtonItem = self.editButtonItem;
}

- (void)viewDidUnload
{
    [super viewDidUnload];
    // Release any retained subviews of the main view.
    // e.g. self.myOutlet = nil;
}

- (BOOL)shouldAutorotateToInterfaceOrientation:(UIInterfaceOrientation)interfaceOrientation
{
    return (interfaceOrientation == UIInterfaceOrientationPortrait);
}

#pragma mark - Table view delegate

- (void)tableView:(UITableView *)tableView didSelectRowAtIndexPath:(NSIndexPath *)indexPath
{
    [tableView deselectRowAtIndexPath:indexPath animated:YES];
    
    // Navigation logic may go here. Create and push another view controller.
    /*
     <#DetailViewController#> *detailViewController = [[<#DetailViewController#> alloc] initWithNibName:@"<#Nib name#>" bundle:nil];
     // ...
     // Pass the selected object to the new view controller.
     [self.navigationController pushViewController:detailViewController animated:YES];
     */
}

@end
