//
//  RHPathRequest.h
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
#import <CoreLocation/CoreLocation.h>


@class RHPath;


@interface RHPathRequest : NSObject

+ (void)makeOffCampusTourRequestWithTagIds:(NSArray *)tags
                              successBlock:(void (^)(RHPath *))successBlock
                              failureBlock:(void (^)(NSError *))failureBlock;


+ (void)makeOnCampusTourRequestWithTagIds:(NSArray *)tags
                       fromGPSCoordinages:(CLLocation *)location
                               toLocation:(NSNumber *)endLocationServerId
                              forDuration:(NSNumber *)duration
                             successBlock:(void (^)(RHPath *))successBlock
                             failureBlock:(void (^)(NSError *))failureBlock;

+ (void)makeOnCampusTourRequestWithTagIds:(NSArray *)tags
                       fromLocationWithId:(NSNumber *)startLocationServerId
                              forDuration:(NSNumber *)duration
                             successBlock:(void (^)(RHPath *))successBlock
                             failureBlock:(void (^)(NSError *))failureBlock;

+ (void)makeDirectionsRequestFromGPSCoordinates:(CLLocation *)location
                                     toLocation:(NSNumber *)endLocationServerId
                                   successBlock:(void (^)(RHPath *))successBlock
                                   failureBlock:(void (^)(NSError *))failureBlock;

+ (void)makeDirectionsRequestFromLocationWithId:(NSNumber *)startLocationServerId
                                     toLocation:(NSNumber *)endLocationServerId
                                   successBlock:(void (^)(RHPath *))successBlock
                                   failureBlock:(void (^)(NSError *))failureBlock;

@end
