//
//  RHScheduleTableViewController.m
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

#import "RHScheduleTableViewController.h"
#import "RHCourse.h"
#import "RHCourseMeeting.h"
#import "RHSchedule.h"


#define kClassPeriodCellIdentifier @"ScheduleClassPeriodCell"
#define kNoClassPeriodCellIdentifier @"ScheduleNoClassPeriodCell"


@interface RHScheduleTableViewCell : UITableViewCell

@property (nonatomic, strong) IBOutlet UILabel *periodLabel;
@property (nonatomic, strong) IBOutlet UILabel *courseNameLabel;
@property (nonatomic, strong) IBOutlet UILabel *timesLabel;

@end


@implementation RHScheduleTableViewCell

@synthesize periodLabel = _periodLabel;
@synthesize courseNameLabel = _courseNameLabel;
@synthesize timesLabel = _timesLabel;

@end


@implementation RHScheduleTableViewController

@synthesize scheduleEntries = _scheduleEntries;
@synthesize delegate = _delegate;

- (BOOL)shouldAutorotateToInterfaceOrientation:(UIInterfaceOrientation)interfaceOrientation
{
    return (interfaceOrientation == UIInterfaceOrientationPortrait);
}

#pragma mark - Table view data source

- (NSInteger)numberOfSectionsInTableView:(UITableView *)tableView
{
    return 1;
}

- (NSInteger)tableView:(UITableView *)tableView numberOfRowsInSection:(NSInteger)section
{
    return self.scheduleEntries.count;
}

- (UITableViewCell *)tableView:(UITableView *)tableView
         cellForRowAtIndexPath:(NSIndexPath *)indexPath
{
    NSArray *times = [NSArray arrayWithObjects:@"8:05 AM - 8:55 AM", @"9:00 AM - 9:50 AM", @"9:55 AM - 10:45 AM", @"10:50 AM - 11:40 AM", @"11:45 AM - 12:35 PM", @"12:40 PM - 1:30 PM", @"1:35 PM - 2:25 PM", @"2:30 PM - 3:20 PM", @"3:25 PM - 4:15 PM", @"4:20 PM - 5:10 PM", nil];
    
    RHScheduleEntry *entry = [self.scheduleEntries objectAtIndex:indexPath.row];
    
    if (entry.courseMeeting == nil) {
        RHScheduleTableViewCell *cell = [tableView dequeueReusableCellWithIdentifier:kNoClassPeriodCellIdentifier];
        
        cell.periodLabel.text = entry.period.description;
        
        return cell;
    }
    
    RHScheduleTableViewCell *cell = [tableView dequeueReusableCellWithIdentifier:kClassPeriodCellIdentifier];
    
    cell.courseNameLabel.text = [NSString stringWithFormat:@"%@ in %@", entry.courseMeeting.course.courseNumber, entry.courseMeeting.room];
    cell.periodLabel.text = entry.period.description;
    cell.timesLabel.text = [times objectAtIndex:entry.period.intValue - 1];
    
    return cell;
}

#pragma mark - Table view delegate

- (void)tableView:(UITableView *)tableView didSelectRowAtIndexPath:(NSIndexPath *)indexPath
{
    [tableView deselectRowAtIndexPath:indexPath animated:YES];
    
    RHScheduleEntry *entry = [self.scheduleEntries objectAtIndex:indexPath.row];
    
    if (entry.courseMeeting != nil && self.delegate != nil) {
        [self.delegate didSelectCourseMeeting:entry.courseMeeting];
    }
}

@end
