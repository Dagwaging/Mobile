//
//  RHPerson.h
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


typedef enum {
    RHPersonTypeStudent = 0,
    RHPersonTypeFacultyOrStaff
} RHPersonType;


@interface RHPerson : NSObject <RHDirectorySearchResult>

+ (id)personFromJSONDictionary:(NSDictionary *)jsonData;

@property (nonatomic, strong) NSString *firstName;
@property (nonatomic, strong) NSString *middleName;
@property (nonatomic, strong) NSString *lastName;

@property (nonatomic, strong) NSNumber *campusMailbox;
@property (nonatomic, strong) NSString *location;
@property (nonatomic, strong) NSString *phoneNumber;
@property (nonatomic, strong) NSString *emailAddress;

@property (nonatomic, strong) RHUser *userAccount;

@property (nonatomic, assign) RHPersonType type;

// Faculty and Staff only
@property (nonatomic, strong) NSString *department;

// Student only
@property (nonatomic, strong) RHUser *advisor;

@property (nonatomic, strong) NSString *grade;
@property (nonatomic, strong) NSString *year;
@property (nonatomic, strong) NSString *majors;

@end
