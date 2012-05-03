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

#import <Security/Security.h>

#import "RHAuthenticationLoader.h"
#import "RHLoaderRequestsWrapper.h"

#define kAuthTokenKey @"Token"
#define kCurrentTermKey @"CurrentTerm"
#define kTermsKey @"Terms"
#define kTermIDKey @"Id"
#define kTermNameKey @"Name"

#define kKeychainIdentifier @"RoseHulmanMobile"
#define kKeychainUsernameKey @"KerberosUsername"
#define kKeychainPasswordKey @"KerberosPassword"

@interface RHAuthenticationLoader ()

@property (nonatomic, readwrite, strong) NSString *authToken;

+ (void)saveToKeyChain:(NSString *)service data:(id)data;

+ (id)loadFromKeyChain:(NSString *)service;

+ (void)deleteFromKeyChain:(NSString *)service;

@end

@implementation RHAuthenticationLoader

@synthesize authToken = _authToken;

static RHAuthenticationLoader *_instance;

+ (void)initialize
{
    static BOOL initialized = NO;
    if(!initialized)
    {
        initialized = YES;
        _instance = [[RHAuthenticationLoader alloc] init];
    }
}

- (id)init
{
    if (self = [super init]) {
        
    }
    
    return self;
}

+ (id)instance
{
    return _instance;
}

- (BOOL)hasCredentials
{
    NSString *username = [RHAuthenticationLoader loadFromKeyChain:kKeychainUsernameKey];
    NSString *password = [RHAuthenticationLoader loadFromKeyChain:kKeychainPasswordKey];
    
    return username.length > 0 && password.length > 0;
}

- (BOOL)hasAuthToken
{
    return self.authToken.length > 0;
}

- (NSString *)username
{
    return [RHAuthenticationLoader loadFromKeyChain:kKeychainUsernameKey];
}

- (void)clearCredentials
{
    [RHAuthenticationLoader deleteFromKeyChain:kKeychainUsernameKey];
    [RHAuthenticationLoader deleteFromKeyChain:kKeychainPasswordKey];
    
    self.authToken = nil;
}

- (void)clearAuthToken
{
    self.authToken = nil;
}

- (void)attemptReauthenticationWithSuccessBlock:(void (^)(void))successBlock
                                   failureBlock:(void (^)(NSError *))failureBlock
{
    NSString *username = [RHAuthenticationLoader loadFromKeyChain:kKeychainUsernameKey];
    NSString *password = [RHAuthenticationLoader loadFromKeyChain:kKeychainPasswordKey];
    
    [RHLoaderRequestsWrapper makeAuthenticationRequestWithUsername:username password:password successBlock:^(NSDictionary *jsonDict) {
        
        NSString *authToken = [jsonDict objectForKey:kAuthTokenKey];

        [RHAuthenticationLoader saveToKeyChain:kAuthTokenKey data:authToken];
        
        successBlock();
        
    } failureBlock:failureBlock];
}

- (void)authenticateWithUsername:(NSString *)username
                        password:(NSString *)password
                    successBlock:(void (^)(void))successBlock
                    failureBlock:(void (^)(NSError *))failureBlock
{
    [RHLoaderRequestsWrapper makeAuthenticationRequestWithUsername:username password:password successBlock:^(NSDictionary *jsonDict) {
        
        self.authToken = [jsonDict objectForKey:kAuthTokenKey];
        
        [RHAuthenticationLoader saveToKeyChain:kKeychainUsernameKey data:username];
        [RHAuthenticationLoader saveToKeyChain:kKeychainPasswordKey data:password];
        
        successBlock();
        
    } failureBlock:failureBlock];
}

#pragma mark - Private Keychain Methods

+ (NSMutableDictionary *)getKeychainQuery:(NSString *)service {
    return [NSMutableDictionary dictionaryWithObjectsAndKeys:
            (__bridge id)kSecClassGenericPassword, (__bridge id)kSecClass,
            service, (__bridge id)kSecAttrService,
            service, (__bridge id)kSecAttrAccount,
            (__bridge id)kSecAttrAccessibleAfterFirstUnlock, (__bridge id)kSecAttrAccessible,
            nil];
}

+ (void)saveToKeyChain:(NSString *)service data:(id)data {
    NSMutableDictionary *keychainQuery = [self getKeychainQuery:service];
    SecItemDelete((__bridge CFDictionaryRef)keychainQuery);
    [keychainQuery setObject:[NSKeyedArchiver archivedDataWithRootObject:data] forKey:(__bridge id)kSecValueData];
    SecItemAdd((__bridge CFDictionaryRef)keychainQuery, NULL);
}

+ (id)loadFromKeyChain:(NSString *)service {
    id ret = nil;
    NSMutableDictionary *keychainQuery = [self getKeychainQuery:service];
    [keychainQuery setObject:(id)kCFBooleanTrue forKey:(__bridge id)kSecReturnData];
    [keychainQuery setObject:(__bridge id)kSecMatchLimitOne forKey:(__bridge id)kSecMatchLimit];
    CFDataRef keyData = NULL;
    if (SecItemCopyMatching((__bridge CFDictionaryRef)keychainQuery, (CFTypeRef *)&keyData) == noErr) {
        @try {
            ret = [NSKeyedUnarchiver unarchiveObjectWithData:(__bridge NSData *)keyData];
        }
        @catch (NSException *e) {
            NSLog(@"Unarchive of %@ failed: %@", service, e);
        }
        @finally {}
    }
    if (keyData) CFRelease(keyData);
    return ret;
}

+ (void)deleteFromKeyChain:(NSString *)service {
    NSMutableDictionary *keychainQuery = [self getKeychainQuery:service];
    SecItemDelete((__bridge CFDictionaryRef)keychainQuery);
}

@end
