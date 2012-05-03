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
#import "RHUser.h"

#define kAdvisorKey @"Advisor"
#define kGradeKey @"Class"
#define kCampusMailboxKey @"CM"
#define kDepartmentKey @"Department"
#define kEmailAddressKey @"Email"
#define kFirstNameKey @"FirstName"
#define kLastNameKey @"LastName"
#define kMajorsKey @"Majors"
#define kMiddleNameKey @"MiddleName"
#define kLocationKey @"Office"
#define kTelephoneNumberKey @"Telephone"
#define kUserKey @"User"
#define kYearKey @"Year"


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
    RHPerson *person = [[RHPerson alloc] init];
    
    person.firstName = [jsonData objectForKey:kFirstNameKey];
    person.middleName = [jsonData objectForKey:kMiddleNameKey];
    person.lastName = [jsonData objectForKey:kLastNameKey];
    
    person.campusMailbox = [jsonData objectForKey:kCampusMailboxKey];
    person.location = [jsonData objectForKey:kLocationKey];
    person.phoneNumber = [jsonData objectForKey:kTelephoneNumberKey];
    person.emailAddress = [jsonData objectForKey:kEmailAddressKey];
    
    person.userAccount = [RHUser userFromJSONDictionary:[jsonData objectForKey:kUserKey]];
    
    if ((id)[jsonData objectForKey:kAdvisorKey] == [NSNull null]) {
        // Faculty/Staff
        person.type = RHPersonTypeFacultyOrStaff;
        person.department = [jsonData objectForKey:kDepartmentKey];
    } else {
        // Student
        person.type = RHPersonTypeStudent;
        person.grade = [jsonData objectForKey:kGradeKey];
        person.year = [jsonData objectForKey:kYearKey];
        person.majors = [jsonData objectForKey:kMajorsKey];
        person.advisor = [RHUser userFromJSONDictionary:[jsonData objectForKey:kAdvisorKey]];
    }
    
    return person;
}

- (NSString *)title
{
    return self.userAccount.title;
}

- (NSString *)subtitle
{
    return self.userAccount.subtitle;
}

@end
