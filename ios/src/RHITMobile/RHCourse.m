//
//  RHCourse.m
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

#import "RHCourse.h"
#import "RHCourseMeeting.h"
#import "RHUser.h"


#define kCommentsKey @"Comments"
#define kCourseNumberKey @"CourseNumber"
#define kCreditsKey @"Credits"
#define kCRNKey @"CRN"
#define kEnrolledKey @"Enrolled"
#define kFinalDayKey @"FinalDay"
#define kFinalHourKey @"FinalHour"
#define kFinalRoomkey @"FinalRoom"
#define kInstructorKey @"Instructor"
#define kMaxEnrolledKey @"MaxEnrolled"
#define kScheduleKey @"Schedule"
#define kStudentsKey @"Students"
#define kTermKey @"Term"
#define kTitleKey @"Title"

@implementation RHCourse

@synthesize crn = _crn;
@synthesize name = _name;
@synthesize courseNumber = _courseNumber;
@synthesize comments = _comments;
@synthesize credits = _credits;
@synthesize term = _term;
@synthesize enrolled = _enrolled;
@synthesize maxEnrolled = _maxEnrolled;
@synthesize finalDay = _finalDay;
@synthesize finalHour = _finalHour;
@synthesize finalRoom = _finalRoom;
@synthesize instructor = _instructor;
@synthesize students = _students;
@synthesize meetings = _meetings;

+ (id)courseFromJSONDictionary:(NSDictionary *)jsonData
{
    RHCourse *course = [[RHCourse alloc] init];
    
    course.crn = [jsonData objectForKey:kCRNKey];
    course.name = [jsonData objectForKey:kTitleKey];
    course.courseNumber = [jsonData objectForKey:kCourseNumberKey];
    course.comments = [jsonData objectForKey:kCommentsKey];
    course.credits = [jsonData objectForKey:kCreditsKey];
    course.term = [jsonData objectForKey:kTermKey];
    course.enrolled = [jsonData objectForKey:kEnrolledKey];
    course.maxEnrolled = [jsonData objectForKey:kMaxEnrolledKey];
    course.finalDay = [jsonData objectForKey:kFinalDayKey];
    course.finalHour = [jsonData objectForKey:kFinalHourKey];
    course.finalRoom = [jsonData objectForKey:kFinalRoomkey];
    
    if ((id) [jsonData objectForKey:kInstructorKey] != [NSNull null]) {
        course.instructor = [RHUser userFromJSONDictionary:[jsonData objectForKey:kInstructorKey]];
    }
    
    NSMutableArray *students = [NSMutableArray array];
    
    if ((id) [jsonData objectForKey:kStudentsKey] != [NSNull null]) {
        for (NSDictionary *studentDict in [jsonData objectForKey:kStudentsKey]) {
            [students addObject:[RHUser userFromJSONDictionary:studentDict]];
        }
    }
    
    course.students = students;
    
    NSMutableArray *meetings = [NSMutableArray array];
    
    if ((id) [jsonData objectForKey:kScheduleKey] != [NSNull null]) {
        for (NSDictionary *meetingDict in [jsonData objectForKey:kScheduleKey]) {
            RHCourseMeeting *meeting = [RHCourseMeeting courseMeetingFromJSONDictionary:meetingDict];
            meeting.course = course;
            [meetings addObject:meeting];
        }
    }
    
    course.meetings = meetings;
    
    return course;
}

- (NSString *)title
{
    return self.courseNumber;
}

- (NSString *)subtitle
{
    return self.name;
}

@end
