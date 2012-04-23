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

#define kVersionKey @"version"


@implementation RHLoaderRequestsWrapper

#pragma mark - DataVersionsLoader Requests

+ (void)makeDataVersionsRequestWithSuccessBlock:(void (^)(NSDictionary *))successBlock
                                   failureBlock:(void (^)(NSError *))failureBlock {
    [RHJSONRequest makeRequestWithPath:kDataVersionsPath
                               urlArgs:nil
                          successBlock:successBlock
                          failureBlock:failureBlock];
}

#pragma mark - LocationsLoader Requests

+ (void)makeTopLocationsRequestWithVersion:(double)version
                              successBlock:(void (^)(NSDictionary *))successBlock
                              failureBlock:(void (^)(NSError *))failureBlock {
    NSString *versionString = [NSString stringWithFormat:@"%f", version];
    NSDictionary *urlArgs = [NSDictionary dictionaryWithObject:versionString
                                                        forKey:kVersionKey];
    [RHJSONRequest makeRequestWithPath:kTopLocationsPath
                               urlArgs:urlArgs
                          successBlock:successBlock
                          failureBlock:failureBlock];
}

+ (void)makeInternalLocationsRequestWithVersion:(double)version
                               parentLocationId:(NSInteger)parentId
                                   successBlock:(void (^)(NSDictionary *))successBlock
                                   failureBlock:(void (^)(NSError *))failureBlock {
    NSString *versionString = [NSString stringWithFormat:@"%f", version];
    NSDictionary *urlArgs = [NSDictionary dictionaryWithObject:versionString
                                                        forKey:kVersionKey];
    NSString *path = [NSString stringWithFormat:kInternalLocationsPath, parentId];
    [RHJSONRequest makeRequestWithPath:path
                               urlArgs:urlArgs
                          successBlock:successBlock
                          failureBlock:failureBlock];
}

#pragma mark - CampusServicesLoader Requests

+ (void)makeCampusServicesRequestWithVersion:(double)version
                                successBlock:(void (^)(NSDictionary *))successBlock
                                failureBlock:(void (^)(NSError *))failureBlock {
    NSString *versionString = [NSString stringWithFormat:@"%f", version];
    NSDictionary *urlArgs = [NSDictionary dictionaryWithObject:versionString
                                                        forKey:kVersionKey];
    [RHJSONRequest makeRequestWithPath:kCampusServicesPath
                               urlArgs:urlArgs
                          successBlock:successBlock
                          failureBlock:failureBlock];   
}

#pragma mark - TourTagsLoader Requests

+ (void)makeTourTagsRequestWithVersion:(double)version
                          SuccessBlock:(void (^)(NSDictionary *))successBlock
                          failureBlock:(void (^)(NSError *))failureBlock {
    NSString *versionString = [NSString stringWithFormat:@"%f", version];
    NSDictionary *urlArgs = [NSDictionary dictionaryWithObject:versionString
                                                        forKey:kVersionKey];
    [RHJSONRequest makeRequestWithPath:kTourTagsPath
                               urlArgs:urlArgs
                          successBlock:successBlock
                          failureBlock:failureBlock];
}

@end
