//
//  RHPerson.m
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

#import "RHPerson.h"

@implementation RHPerson

@synthesize firstName = _firstName;
@synthesize middleName = _middleName;
@synthesize lastName = _lastName;

@synthesize campusMailbox = _campusMailbox;
@synthesize location = _location;
@synthesize phoneNumber = _phoneNumber;
@synthesize emailAddress = _emailAddress;

@synthesize userAccount = _userAccount;
@synthesize type = _type;

@synthesize department = _department;

@synthesize grade = _grade;
@synthesize year = _year;
@synthesize majors = _majors;
@synthesize advisor = _advisor;

+ (id)personFromJSONDictionary:(NSDictionary *)jsonData
{
    return nil;
}

- (NSString *)title
{
    return @"TODO";
}

- (NSString *)subtitle
{
    return @"TODO";
}

@end
