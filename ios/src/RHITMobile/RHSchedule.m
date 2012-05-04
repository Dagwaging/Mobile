//
//  RHSchedule.m
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

#import "RHSchedule.h"
#import "RHCourse.h"
#import "RHCourseMeeting.h"

@interface RHScheduleEntry ()

- (NSComparisonResult)compare:(RHScheduleEntry *)other;

@end


@implementation RHScheduleEntry

@synthesize period = _period;
@synthesize courseMeeting = _courseMeeting;

- (id)initWithPeriod:(NSNumber *)period meeting:(RHCourseMeeting *)meeting
{
    if (self = [super init]) {
        self.period = period;
        self.courseMeeting = meeting;
    }
    
    return self;
}

- (NSComparisonResult)compare:(RHScheduleEntry *)other
{
    return [self.period compare:other.period];
}

@end


@implementation RHSchedule

@synthesize courses = _courses;

- (id)initWithCourses:(NSArray *)courses
{
    if (self = [super init]) {
        self.courses = courses;
    }
    
    return self;
}

- (NSArray *)scheduleEntriesForDayIndex:(NSNumber *)dayIndex
{
    NSArray *dayInitials = [NSArray arrayWithObjects:@"M", @"T", @"W", @"R", @"F", @"S", nil];
    
    NSMutableArray *entries = [NSMutableArray array];
    
    if (dayIndex.intValue < 0 || dayIndex.intValue > 5 || self.courses == nil) {
        return entries;
    }
    
    // Add any course meetings to our returned entries, creating separate
    // schedule entries for each period.
    for (RHCourse *course in self.courses.copy) {
        for (RHCourseMeeting *meeting in course.meetings) {
            
            NSInteger meetingDay = [dayInitials indexOfObject:meeting.day];
            
            if (dayIndex.intValue == meetingDay) {
                for (int i = meeting.startPeriod.intValue; i <= meeting.endPeriod.intValue; i ++) {
                    [entries addObject:[[RHScheduleEntry alloc] initWithPeriod:[NSNumber numberWithInt:i] meeting:meeting]];
                }
            }
        }
    }
    
    // Sort the entries by period for displaying
    [entries sortUsingSelector:@selector(compare:)];
    
    // Insert empty entries for any missing periods
    NSMutableArray *fullEntries = [NSMutableArray array];
    int currentIndex = 0;
    
    for (int currentPeriod = 1; currentPeriod <= 10; currentPeriod ++) {
        RHScheduleEntry *entry;
        
        if (currentIndex < entries.count) {
            entry = [entries objectAtIndex:currentIndex];
        }
        
        if (entry != nil && entry.period.intValue == currentPeriod) {
            while (entry.period.intValue == currentPeriod) {
                [fullEntries addObject:entry];
                
                currentIndex ++;
                
                if (currentIndex < entries.count) {
                    entry = [entries objectAtIndex:currentIndex];
                } else {
                    break;
                }
            }
        } else {
            RHScheduleEntry *emptyEntry = [[RHScheduleEntry alloc] initWithPeriod:[NSNumber numberWithInt:currentPeriod] meeting:nil];
            [fullEntries addObject:emptyEntry];
        }
    }
    
    
    return fullEntries;
}

@end
