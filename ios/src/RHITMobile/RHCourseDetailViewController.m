//
//  RHCourseDetailViewController.m
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

#import "RHCourseDetailViewController.h"
#import "RHCourse.h"
#import "RHDirectoryRequestsWrapper.h"
#import "RHFacultyDetailViewController.h"
#import "RHRosterViewController.h"
#import "RHSchedule.h"
#import "RHScheduleViewController.h"
#import "RHUser.h"

#define kInstructorSegueIdentifier @"CourseDetailToFacultyDetailSegue"
#define kRosterSegueIdentifier @"CourseDetailToRosterSegue"
#define kScheduleSegueIdentifier @"CourseDetailToScheduleSegue"

@interface RHCourseDetailViewController () {
    @private
    BOOL _loaded;
}

@property (nonatomic, strong) IBOutlet UILabel *nameLabel;
@property (nonatomic, strong) IBOutlet UILabel *commentsLabel;
@property (nonatomic, strong) IBOutlet UILabel *creditHoursLabel;
@property (nonatomic, strong) IBOutlet UILabel *instructorLabel;
@property (nonatomic, strong) IBOutlet UILabel *finalLabel;
@property (nonatomic, strong) IBOutlet UILabel *enrolledLabel;
@property (nonatomic, strong) IBOutlet UILabel *maxEnrolledLabel;
@property (nonatomic, strong) IBOutlet UILabel *crnLabel;

@property (nonatomic, strong) RHCourse *course;

@end

@implementation RHCourseDetailViewController

@synthesize sourceCourse = _sourceCourse;
@synthesize nameLabel = _nameLabel;
@synthesize commentsLabel = _commentsLabel;
@synthesize creditHoursLabel = _creditHoursLabel;
@synthesize instructorLabel = _instructorLabel;
@synthesize finalLabel = _finalLabel;
@synthesize enrolledLabel = _enrolledLabel;
@synthesize maxEnrolledLabel = _maxEnrolledLabel;
@synthesize crnLabel = _crnLabel;
@synthesize course = _course;

- (void)viewDidLoad
{
    [super viewDidLoad];
    
    _loaded = NO;
    
    self.navigationItem.title = @"Loading...";
    
    UIActivityIndicatorView *activityIndicator = [[UIActivityIndicatorView alloc] initWithActivityIndicatorStyle:UIActivityIndicatorViewStyleGray];
    self.navigationItem.rightBarButtonItem = [[UIBarButtonItem alloc] initWithCustomView:activityIndicator];
    [activityIndicator startAnimating];
    
    [RHDirectoryRequestsWrapper makeCourseDetailRequestForCourse:self.sourceCourse successBlock:^(RHCourse *course) {
        
        self.course = course;
        
        self.navigationItem.title = course.courseNumber;
        self.navigationItem.rightBarButtonItem = nil;
        
        self.nameLabel.text = course.name;
        self.commentsLabel.text = course.comments;
        self.creditHoursLabel.text = course.credits.description;
        self.instructorLabel.text = course.instructor.fullName;
        self.finalLabel.text = course.finalRoom; // TODO
        self.enrolledLabel.text = course.enrolled.description;
        self.maxEnrolledLabel.text = course.maxEnrolled.description;
        self.crnLabel.text = course.crn.description;
        
        _loaded = YES;
        
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
    if ([segue.identifier isEqualToString:kInstructorSegueIdentifier]) {
        RHFacultyDetailViewController *facultyDetail = segue.destinationViewController;
        facultyDetail.user = self.course.instructor;
    } else if ([segue.identifier isEqualToString:kRosterSegueIdentifier]) {
        RHRosterViewController *roster = segue.destinationViewController;
        roster.course = self.course;
    } else if ([segue.identifier isEqualToString:kScheduleSegueIdentifier]) {
        RHScheduleViewController *schedule = segue.destinationViewController;
        schedule.schedule = [[RHSchedule alloc] initWithCourses:[NSArray arrayWithObject:self.course]];
    }
}

#pragma mark - Table view delegate

- (void)tableView:(UITableView *)tableView didSelectRowAtIndexPath:(NSIndexPath *)indexPath
{
    [tableView deselectRowAtIndexPath:indexPath animated:YES];
}

@end
