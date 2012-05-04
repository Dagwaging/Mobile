//
//  RHStudentDetailViewController.m
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

#import "RHStudentDetailViewController.h"
#import "RHDirectoryRequestsWrapper.h"
#import "RHFacultyDetailViewController.h"
#import "RHPerson.h"
#import "RHSchedule.h"
#import "RHScheduleViewController.h"
#import "RHUser.h"

#define kAdvisorSegueIdentifier @"StudentDetailToFacultyDetailSegue"
#define kScheduleSegueIdentifier @"StudentDetailToScheduleSegue"

@interface RHStudentDetailViewController () {
@private
    BOOL _loaded;
}

@property (nonatomic, strong) IBOutlet UILabel *standingLabel;
@property (nonatomic, strong) IBOutlet UILabel *majorsLabel;
@property (nonatomic, strong) IBOutlet UILabel *campusMailboxLabel;
@property (nonatomic, strong) IBOutlet UILabel *emailAddressLabel;
@property (nonatomic, strong) IBOutlet UILabel *telephoneNumberLabel;
@property (nonatomic, strong) IBOutlet UILabel *locationLabel;
@property (nonatomic, strong) IBOutlet UILabel *advisorLabel;

@property (nonatomic, strong) RHPerson *person;
@property (nonatomic, strong) NSArray *courses;

@end

@implementation RHStudentDetailViewController

@synthesize user = _user;
@synthesize standingLabel = _standingLabel;
@synthesize majorsLabel = _majorsLabel;
@synthesize campusMailboxLabel = _campusMailboxLabel;
@synthesize emailAddressLabel = _emailAddressLabel;
@synthesize telephoneNumberLabel = _telephoneNumberLabel;
@synthesize locationLabel = _locationLabel;
@synthesize advisorLabel = _advisorLabel;
@synthesize person = _person;
@synthesize courses = _courses;

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
    
    _loaded = NO;
    
    self.navigationItem.title = @"Loading...";
    
    UIActivityIndicatorView *activityIndicator = [[UIActivityIndicatorView alloc] initWithActivityIndicatorStyle:UIActivityIndicatorViewStyleGray];
    self.navigationItem.rightBarButtonItem = [[UIBarButtonItem alloc] initWithCustomView:activityIndicator];
    [activityIndicator startAnimating];
    
    [RHDirectoryRequestsWrapper makePersonDetailRequestForUser:self.user successBlock:^(RHPerson *person) {
        
        self.person = person;
        
        self.navigationItem.title = person.userAccount.fullName;
        
        
        self.standingLabel.text = person.userAccount.summary;
        self.majorsLabel.text = person.majors;
        self.campusMailboxLabel.text = person.campusMailbox.description;
        self.emailAddressLabel.text = person.emailAddress;
        self.telephoneNumberLabel.text = person.phoneNumber;
        self.locationLabel.text = person.location;
        
        self.advisorLabel.text = person.advisor.fullName;
        
        [RHDirectoryRequestsWrapper makeScheduleRequestForUser:person.userAccount successBlock:^(NSArray *courses) {
            self.navigationItem.rightBarButtonItem = nil;
            self.courses = courses;
            
            _loaded = YES;
        } failureBlock:^(NSError *error) {
            [self.navigationController popViewControllerAnimated:YES];
        }];
        
        [self.tableView reloadData];
        
    } failureBlock:^(NSError *error) {
        [self.navigationController popViewControllerAnimated:YES];
    }];
}

- (BOOL)shouldAutorotateToInterfaceOrientation:(UIInterfaceOrientation)interfaceOrientation
{
    return (interfaceOrientation == UIInterfaceOrientationPortrait);
}

- (void)prepareForSegue:(UIStoryboardSegue *)segue sender:(id)sender
{
    if ([segue.identifier isEqualToString:kAdvisorSegueIdentifier]) {
        RHFacultyDetailViewController *facultyDetail = segue.destinationViewController;
        facultyDetail.user = self.person.advisor;
    } else if ([segue.identifier isEqualToString:kScheduleSegueIdentifier]) {
        RHScheduleViewController *schedule = segue.destinationViewController;
        while (!_loaded);
        schedule.schedule = [[RHSchedule alloc] initWithCourses:self.courses];
    }
}

#pragma mark - Table view delegate

- (void)tableView:(UITableView *)tableView didSelectRowAtIndexPath:(NSIndexPath *)indexPath
{
    [tableView deselectRowAtIndexPath:indexPath animated:YES];
}

@end
