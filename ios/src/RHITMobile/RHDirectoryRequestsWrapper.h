//
//  RHDirectoryRequestsWrapper.h
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

@class RHUser, RHPerson, RHCourse;

@interface RHDirectoryRequestsWrapper : NSObject

+ (void)makeUserSearchRequestForSearchTerm:(NSString *)searchTerm
                              successBlock:(void (^)(NSArray *))successBlock
                              failureBlock:(void (^)(NSError *))failureBlock;

+ (void)makeCourseSearchRequestForSearchTerm:(NSString *)searchTerm
                                        term:(NSString *)term
                                successBlock:(void (^)(NSArray *))successBlock
                                failureBlock:(void (^)(NSError *))failureBlock;

+ (void)makePersonDetailRequestForUser:(RHUser *)user
                          successBlock:(void (^)(RHPerson *))successBlock
                          failureBlock:(void (^)(NSError *))failureBlock;

+ (void)makeCourseDetailRequestForCourse:(RHCourse *)course
                            successBlock:(void (^)(RHCourse *))successBlock
                            failureBlock:(void (^)(NSError *))failureBlock;

+ (void)makeScheduleRequestForUser:(RHUser *)user
                      successBlock:(void (^)(NSArray *))successBlock
                      failureBlock:(void (^)(NSError *))failureBlock;

@end
