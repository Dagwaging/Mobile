//
//  RHLoaderRequestsWrapper.m
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

#import "RHLoaderRequestsWrapper.h"
#import "RHJSONRequest.h"


#define kDataVersionsPath @"/"
#define kTopLocationsPath @"/locations/data/top"
#define kInternalLocationsPath @"/locations/data/within/%d"
#define kCampusServicesPath @"/services"
#define kTourTagsPath @"/tours/tags"
#define kBannerLoginPath @"/banner/authenticate"

#define kVersionKey @"version"

#define kUsernameHeader @"Login-Username"
#define kPasswordHeader @"Login-Password"
#define kAuthTokenHeader @"Auth-Token"


@implementation RHLoaderRequestsWrapper

#pragma mark - DataVersionsLoader Requests

+ (NSDictionary *)makeSynchronousDataVersionsRequestWithError:(NSError *__strong *)error
{
    return [RHJSONRequest makeSynchronousRequestWithPath:kDataVersionsPath
                                                 urlArgs:nil
                                                   error:error];
}


#pragma mark - LocationsLoader Requests

+ (NSDictionary *)makeSynchronousTopLocationsRequestWithWithVersion:(NSNumber *)version
                                                              error:(NSError *__strong *)error
{
    NSString *versionString = [NSString stringWithFormat:@"%f", version.doubleValue];
    NSDictionary *urlArgs = [NSDictionary dictionaryWithObject:versionString
                                                        forKey:kVersionKey];
    
    return [RHJSONRequest makeSynchronousRequestWithPath:kTopLocationsPath
                                                 urlArgs:urlArgs
                                                   error:error];
}

+ (NSDictionary *)makeSynchronousInternalLocationsRequestForParentLocationID:(NSNumber *)parentId 
                                                                     version:(NSNumber *)version 
                                                                       error:(NSError *__strong *)error
{
    NSString *versionString = [NSString stringWithFormat:@"%f", version.doubleValue];
    NSDictionary *urlArgs = [NSDictionary dictionaryWithObject:versionString
                                                        forKey:kVersionKey];
    NSString *path = [NSString stringWithFormat:kInternalLocationsPath, parentId.intValue];
    
    return [RHJSONRequest makeSynchronousRequestWithPath:path urlArgs:urlArgs error:error];
}


#pragma mark - CampusServicesLoader Requests

+ (NSDictionary *)makeSynchronousCampusServicesRequestWithWithVersion:(NSNumber *)version
                                                                error:(NSError *__strong *)error
{
    NSString *versionString = [NSString stringWithFormat:@"%f", version];
    NSDictionary *urlArgs = [NSDictionary dictionaryWithObject:versionString
                                                        forKey:kVersionKey];
    
    return [RHJSONRequest makeSynchronousRequestWithPath:kCampusServicesPath
                                                 urlArgs:urlArgs
                                                   error:error];
}


#pragma mark - TourTagsLoader Requests

+ (NSDictionary *)makeSynchronousTourTagsRequestWithWithVersion:(NSNumber *)version error:(NSError *__strong *)error
{
    NSString *versionString = [NSString stringWithFormat:@"%f", version];
    NSDictionary *urlArgs = [NSDictionary dictionaryWithObject:versionString
                                                        forKey:kVersionKey];
    
    return [RHJSONRequest makeSynchronousRequestWithPath:kTourTagsPath
                                                 urlArgs:urlArgs
                                                   error:error];
}

#pragma mark - AuthenticationLoader Requests

+ (void)makeAuthenticationRequestWithUsername:(NSString *)username
                                     password:(NSString *)password
                                 successBlock:(void (^)(NSDictionary *))successBlock 
                                 failureBlock:(void (^)(NSError *))failureBlock
{
    NSDictionary *headers = [NSMutableDictionary dictionary];
    [headers setValue:username forKey:kUsernameHeader];
    [headers setValue:password forKey:kPasswordHeader];
    
    [RHJSONRequest makeRequestWithPath:kBannerLoginPath
                               headers:headers
                               urlArgs:nil
                          successBlock:successBlock
                          failureBlock:failureBlock];
    
}

@end
