//
//  RHTourTagsLoader.h
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

#import "RHLoader.h"

@interface RHAuthenticationLoader : RHLoader

+ (id)instance;

@property (nonatomic, readonly) NSString *username;

@property (nonatomic, strong, readonly) NSString *authToken;

@property (nonatomic, strong, readonly) NSString *currentTerm;

@property (nonatomic, strong, readonly) NSDictionary *validTerms;

- (BOOL)hasCredentials;

- (BOOL)hasAuthToken;

- (void)clearCredentials;

- (void)clearAuthToken;

- (void)attemptReauthenticationWithSuccessBlock:(void (^)(void))successBlock
                                   failureBlock:(void (^)(NSError *))failureBlock;

- (void)authenticateWithUsername:(NSString *)username
                        password:(NSString *)password
                    successBlock:(void (^)(void))successBlock
                    failureBlock:(void (^)(NSError *))failureBlock;

@end
