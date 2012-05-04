//
//  RHCourseMeeting.m
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

#import "RHCourseMeeting.h"

#define kDayKey @"Day"
#define kEndPeriodKey @"EndPeriod"
#define kStartPeriodKey @"StartPeriod"
#define kRoomKey @"Room"

@implementation RHCourseMeeting

@synthesize day = _day;
@synthesize room = _room;
@synthesize startPeriod = _startPeriod;
@synthesize endPeriod = _endPeriod;
@synthesize course = _course;

+ (id)courseMeetingFromJSONDictionary:(NSDictionary *)jsonData
{
    RHCourseMeeting *courseMeeting = [[RHCourseMeeting alloc] init];
    
    courseMeeting.day = (id)[jsonData objectForKey:kDayKey] == [NSNull null] ? @"" : [jsonData objectForKey:kDayKey];
    courseMeeting.room = (id)[jsonData objectForKey:kRoomKey] == [NSNull null] ? @"" : [jsonData objectForKey:kRoomKey];
    courseMeeting.startPeriod = (id)[jsonData objectForKey:kStartPeriodKey] == [NSNull null] ? [NSNumber numberWithInt:-1] : [jsonData objectForKey:kStartPeriodKey];
    courseMeeting.endPeriod = (id)[jsonData objectForKey:kEndPeriodKey] == [NSNull null] ? [NSNumber numberWithInt:-1] : [jsonData objectForKey:kEndPeriodKey];
    
    return courseMeeting;
}

@end
