//
//  RHCourse.h
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

#import <Foundation/Foundation.h>

#import "RHDirectorySearchResult.h"

@class RHUser;

@interface RHCourse : NSObject <RHDirectorySearchResult>

+ (id)courseFromJSONDictionary:(NSDictionary *)jsonData;

@property (nonatomic, strong) NSNumber *crn;

@property (nonatomic, strong) NSString *name;
@property (nonatomic, strong) NSString *courseNumber;
@property (nonatomic, strong) NSString *comments;
@property (nonatomic, strong) NSNumber *credits;
@property (nonatomic, strong) NSNumber *term;
@property (nonatomic, strong) NSNumber *enrolled;
@property (nonatomic, strong) NSNumber *maxEnrolled;
@property (nonatomic, strong) NSString *finalDay;
@property (nonatomic, strong) NSNumber *finalHour;
@property (nonatomic, strong) NSString *finalRoom;

@property (nonatomic, strong) RHUser *instructor;
@property (nonatomic, strong) NSArray *students;
@property (nonatomic, strong) NSArray *meetings;


@end
