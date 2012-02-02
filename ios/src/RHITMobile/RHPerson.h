//
//  RHPerson.h
//  RHIT Mobile Campus Directory
//
//  Copyright 2011 Rose-Hulman Institute of Technology
// 
//  Licensed under the Apache License, Version 2.0 (the "License");
//  you may not use this file except in compliance with the License.
//  You may obtain a copy of the License at
//
//  http://www.apache.org/licenses/LICENSE-2.0
//
//  Unless required by applicable law or agreed to in writing, software
//  distributed under the License is distributed on an "AS IS" BASIS,
//  WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//  See the License for the specific language governing permissions and
//  limitations under the License.
//

#import <Foundation/Foundation.h>
#import <CoreData/CoreData.h>

#define kRHPersonEntityName @"RHPerson"


@class RHLocation;

@interface RHPerson : NSManagedObject

@property (nonatomic, strong) NSString * firstName;
@property (nonatomic, strong) NSString * lastName;
@property (nonatomic, strong) NSString * middleName;
@property (nonatomic, strong) NSNumber * role;
@property (nonatomic, strong) NSString * emailAddress;
@property (nonatomic, strong) NSString * campusMailbox;
@property (nonatomic, strong) NSString * position;
@property (nonatomic, strong) NSString * phoneNumber;
@property (nonatomic, strong) RHLocation *location;

@end
