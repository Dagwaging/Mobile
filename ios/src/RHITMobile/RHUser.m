//
//  RHUser.m
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

#import "RHUser.h"

#define kFullNameKey @"FullName"
#define kSubtitleKey @"Subtitle"
#define kUsernameKey @"Username"

@implementation RHUser

@synthesize fullName = _fullName;
@synthesize summary = _summary;
@synthesize username = _username;
@synthesize type = _type;

+ (id)userFromJSONDictionary:(NSDictionary *)jsonData
{
    RHUser *user = [[RHUser alloc] init];
    
    user.fullName = [jsonData objectForKey:kFullNameKey];
    user.summary = [jsonData objectForKey:kSubtitleKey];
    user.username = [jsonData objectForKey:kUsernameKey];
    
    if ([user.summary rangeOfString:@"Student"].location == NSNotFound) {
        user.type = RHUserTypeFacultyOrStaff;
    } else {
        user.type = RHUserTypeStudent;
    }
    
    return user;
}

- (NSString *)title
{
    return self.fullName;
}

- (NSString *)subtitle
{
    return self.summary;
}

@end
