//
//  RHMeetingDetailViewController.m
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

#import "RHMeetingDetailViewController.h"
#import "RHCourse.h"
#import "RHCourseDetailViewController.h"
#import "RHCourseMeeting.h"

#define kCourseSegueIdentifier @"MeetingDetailToCourseDegailSegue"

@interface RHMeetingDetailViewController ()

@property (nonatomic, strong) IBOutlet UILabel *courseNameLabel;

@property (nonatomic, strong) IBOutlet UILabel *locationLabel;

@property (nonatomic, strong) IBOutlet UILabel *periodsLabel;

@end

@implementation RHMeetingDetailViewController

@synthesize meeting = _meeting;
@synthesize courseNameLabel = _courseNameLabel;
@synthesize locationLabel = _locationLabel;
@synthesize periodsLabel = _periodsLabel;

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
    
    self.courseNameLabel.text = [NSString stringWithFormat:@"%@: %@", self.meeting.course.courseNumber, self.meeting.course.name];
    
    self.locationLabel.text = self.meeting.room;
    
    if (self.meeting.startPeriod.intValue == self.meeting.endPeriod.intValue) {
        self.periodsLabel.text = self.meeting.startPeriod.description;
    } else {
        self.periodsLabel.text = [NSString stringWithFormat:@"%d-%d", self.meeting.startPeriod.intValue, self.meeting.endPeriod.intValue];
    }
}

- (void)viewDidUnload
{
    [super viewDidUnload];
    // Release any retained subviews of the main view.
    // e.g. self.myOutlet = nil;
}

- (void)prepareForSegue:(UIStoryboardSegue *)segue sender:(id)sender
{
    if ([segue.identifier isEqualToString:kCourseSegueIdentifier]) {
        RHCourseDetailViewController *courseDetail = segue.destinationViewController;
        courseDetail.sourceCourse = self.meeting.course;
    }
}

- (BOOL)shouldAutorotateToInterfaceOrientation:(UIInterfaceOrientation)interfaceOrientation
{
    return (interfaceOrientation == UIInterfaceOrientationPortrait);
}

#pragma mark - Table view delegate

- (void)tableView:(UITableView *)tableView didSelectRowAtIndexPath:(NSIndexPath *)indexPath
{
    [tableView deselectRowAtIndexPath:indexPath animated:YES];
}

@end
