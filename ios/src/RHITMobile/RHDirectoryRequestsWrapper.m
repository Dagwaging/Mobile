//
//  RHDirectoryRequestsWrapper.m
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

#import "RHDirectoryRequestsWrapper.h"
#import "RHAuthenticationLoader.h"
#import "RHCourse.h"
#import "RHJSONRequest.h"
#import "RHPerson.h"
#import "RHUser.h"


#define kAuthTokenHeader @"Auth-Token"

#define kUserSearchPath @"/banner/user/search/%@"
#define kCourseSearchPath @"/banner/course/search/%@/%@"
#define kPersonDataPath @"/banner/user/data/%@"
#define kCourseDataPath @"/banner/course/data/%@/%@"

#define kUsersKey @"Users"
#define kCoursesKey @"Courses"


@interface RHDirectoryRequestsWrapper ()

+ (NSDictionary *)makeAuthTokenHeaders;

@end


@implementation RHDirectoryRequestsWrapper

+ (void)makeUserSearchRequestForSearchTerm:(NSString *)searchTerm
                              successBlock:(void (^)(NSArray *))successBlock
                              failureBlock:(void (^)(NSError *))failureBlock
{
    searchTerm = [searchTerm stringByReplacingOccurrencesOfString:@" " withString:@"+"];
    NSString *fullPath = [NSString stringWithFormat:kUserSearchPath, searchTerm];
    
    [RHJSONRequest makeRequestWithPath:fullPath headers:[self makeAuthTokenHeaders] urlArgs:nil successBlock:^(NSDictionary *jsonDict) {
        
        NSMutableArray *results = [NSMutableArray array];
        
        for (NSDictionary *userDict in [jsonDict objectForKey:kUsersKey]) {
            [results addObject:[RHUser userFromJSONDictionary:userDict]];
        }
        
        successBlock(results);
        
    } failureBlock:failureBlock];
}

+ (void)makeCourseSearchRequestForSearchTerm:(NSString *)searchTerm
                                        term:(NSString *)term
                                successBlock:(void (^)(NSArray *))successBlock 
                                failureBlock:(void (^)(NSError *))failureBlock
{
    searchTerm = [searchTerm stringByReplacingOccurrencesOfString:@" " withString:@"+"];
    NSString *fullPath = [NSString stringWithFormat:kCourseSearchPath, term, searchTerm];
    
    [RHJSONRequest makeRequestWithPath:fullPath headers:[self makeAuthTokenHeaders] urlArgs:nil successBlock:^(NSDictionary *jsonDict) {
        
        NSMutableArray *results = [NSMutableArray array];
        
        for (NSDictionary *courseDict in [jsonDict objectForKey:kCoursesKey]) {
            [results addObject:[RHCourse courseFromJSONDictionary:courseDict]];
        }
        
        successBlock(results);
        
    } failureBlock:failureBlock];
}

+ (void)makePersonDetailRequestForUser:(RHUser *)user
                          successBlock:(void (^)(RHPerson *))successBlock
                          failureBlock:(void (^)(NSError *))failureBlock
{
    NSString *fullPath = [NSString stringWithFormat:kPersonDataPath, user.username];
    
    [RHJSONRequest makeRequestWithPath:fullPath headers:[self makeAuthTokenHeaders] urlArgs:nil successBlock:^(NSDictionary *jsonData) {
        
        successBlock([RHPerson personFromJSONDictionary:jsonData]);
        
    } failureBlock:failureBlock];
}

+ (void)makeCourseDetailRequestForCourse:(RHCourse *)course
                            successBlock:(void (^)(RHCourse *))successBlock
                            failureBlock:(void (^)(NSError *))failureBlock
{
    NSString *fullPath = [NSString stringWithFormat:kCourseDataPath, course.term, course.crn];
    
    [RHJSONRequest makeRequestWithPath:fullPath headers:[self makeAuthTokenHeaders] urlArgs:nil successBlock:^(NSDictionary *jsonDict) {
        
        NSArray *courses = [jsonDict objectForKey:kCoursesKey];
        
        successBlock([RHCourse courseFromJSONDictionary:[courses objectAtIndex:0]]);
        
    } failureBlock:failureBlock];
}

#pragma mark - Private Methods

+ (NSDictionary *)makeAuthTokenHeaders
{
    return [NSDictionary dictionaryWithObject:[RHAuthenticationLoader.instance authToken] forKey:kAuthTokenHeader];
}

@end
