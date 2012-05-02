//
//  RHJSONRequest.m
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

#import "RHJSONRequest.h"
#import "CJSONDeserializer.h"
#import "NSDictionary_JSONExtensions.h"
#import "RHConstants.h"


#define kScheme @"https"
#define kHost @"mobilewin.csse.rose-hulman.edu:5601"
#define kHostSansPort @"mobilewin.csse.rose-hulman.edu"


@interface NSURLRequest (DummyInterface)
+ (void)setAllowsAnyHTTPSCertificate:(BOOL)allow forHost:(NSString*)host;
@end


@interface RHJSONRequest () {
    void (^_successBlock)(NSDictionary *);
    void (^_failureBlock)(NSError *);
    NSMutableData *_responseData;
    NSURLConnection *_connection;
}

+ (NSURL *)finalURLWithPath:(NSString *)path
                    urlArgs:(NSDictionary *)urlArgs;

- (id)initWithSuccessBlock:(void (^)(NSDictionary *))successBlock
              failureBlock:(void (^)(NSError *))failureBlock;

+ (void)updateNotificationStatus;

@end


@implementation RHJSONRequest

static int _numRequests = 0;

+ (void)makeRequestWithPath:(NSString *)path
                    urlArgs:(NSDictionary *)urlArgs
               successBlock:(void (^)(NSDictionary *))successBlock
               failureBlock:(void (^)(NSError *))failureBlock
{
    [NSURLRequest setAllowsAnyHTTPSCertificate:YES forHost:kHostSansPort];
    
    NSURL *requestUrl = [RHJSONRequest finalURLWithPath:path urlArgs:urlArgs];
    
#ifdef RHITMobile_RHNetworkDebug
    NSLog(@"Making request: %@", requestUrl.absoluteString);
#endif
    
    NSURLRequest *request = [NSURLRequest requestWithURL:requestUrl];
    
    RHJSONRequest *rhRequest = [[RHJSONRequest alloc] initWithSuccessBlock:successBlock failureBlock:failureBlock];
    
    @synchronized ([RHJSONRequest class]) {
        _numRequests ++;
    }
    
    [RHJSONRequest updateNotificationStatus];
    
    NSURLConnection *connection = [NSURLConnection connectionWithRequest:request delegate:rhRequest];
    rhRequest.connection = connection;
}

+ (NSDictionary *)makeSynchronousRequestWithPath:(NSString *)path
                                         urlArgs:(NSDictionary *)urlArgs
                                           error:(NSError *__strong *)error
{
    [NSURLRequest setAllowsAnyHTTPSCertificate:YES forHost:kHostSansPort];
    
    NSURL *finalUrl = [RHJSONRequest finalURLWithPath:path urlArgs:urlArgs];
    NSError *currentError;
    
#ifdef RHITMobile_RHNetworkDebug
    NSLog(@"Making request: %@", finalUrl.absoluteString);
#endif
    
    @synchronized ([RHJSONRequest class]) {
        _numRequests ++;
    }
    
    [RHJSONRequest updateNotificationStatus];
    
    NSData *responseData = [NSURLConnection sendSynchronousRequest:[NSURLRequest requestWithURL:finalUrl] returningResponse:nil error:&currentError];
    
    @synchronized ([RHJSONRequest class]) {
        _numRequests --;
    }
    
    [RHJSONRequest updateNotificationStatus];
    
    if (currentError) {
        error = &currentError;
        return nil;
    }
    
    NSDictionary *response = [NSDictionary dictionaryWithJSONData:responseData error:&currentError];
    
    if (currentError) {
        error = &currentError;
        return nil;
    }
    
#ifdef RHITMobile_RHNetworkDebug
    NSLog(@"Successful response from server");
#endif
    
#ifdef RHITMobile_RHJSONDebug
    NSLog(@"Successful response from server: %@", response);
#endif
    
    return response;
}

+ (NSURL *)finalURLWithPath:(NSString *)path
                    urlArgs:(NSDictionary *)urlArgs
{
    NSString *allArgs = @"";
    
    for (NSString *key in urlArgs.keyEnumerator) {
        if (allArgs.length != 0) {
            allArgs = [allArgs stringByAppendingString:@"&"];
        }
        
        NSString *encodedKey = (__bridge NSString *)CFURLCreateStringByAddingPercentEscapes(NULL,(__bridge CFStringRef)key, NULL, (CFStringRef)@"!*'();:@&=$,/?%#[]",kCFStringEncodingUTF8 );
        NSString *encodedValue = (__bridge NSString *)CFURLCreateStringByAddingPercentEscapes(NULL,(__bridge CFStringRef)[urlArgs objectForKey:key], NULL, (CFStringRef)@"!*'();:@&=+$,/?%#[]",kCFStringEncodingUTF8 );
        
        allArgs = [allArgs stringByAppendingFormat:@"%@=%@", encodedKey, encodedValue];
    }
    
    NSString *urlPath = allArgs.length == 0 ? path : [path stringByAppendingFormat:@"?%@", allArgs];
    
    return [[NSURL alloc] initWithScheme:kScheme host:kHost path:urlPath];
}

- (id)initWithSuccessBlock:(void (^)(NSDictionary *))successBlock
              failureBlock:(void (^)(NSError *))failureBlock
{
    self = [super init];
    
    if (self) {
        _successBlock = successBlock;
        _failureBlock = failureBlock;
        
        _responseData = [NSMutableData data];
    }
    
    return self;
}

- (void)setConnection:(NSURLConnection *)connection
{
    _connection = connection;
}

- (void)connection:(NSURLConnection *)connection didReceiveResponse:(NSURLResponse *)response
{
    [_responseData setLength:0];
}

- (void)connection:(NSURLConnection *)connection didReceiveData:(NSData *)data
{
    [_responseData appendData:data];
}

- (void)connection:(NSURLConnection *)connection
  didFailWithError:(NSError *)error
{
    @synchronized ([RHJSONRequest class]) {
        _numRequests --;
    }
    
    [RHJSONRequest updateNotificationStatus];
    
#ifdef RHITMobile_RHNetworkDebug
    NSLog(@"Error from NSURLConnection: %@", error);
#endif
    _failureBlock(error);
}

- (void)connectionDidFinishLoading:(NSURLConnection *)connection
{
    @synchronized ([RHJSONRequest class]) {
        _numRequests --;
    }
    
    [RHJSONRequest updateNotificationStatus];
    
    NSError *potentialError;
    NSDictionary *jsonDict = [NSDictionary dictionaryWithJSONData:_responseData
                                                            error:&potentialError];
    
    if (potentialError) {
#ifdef RHITMobile_RHNetworkDebug
        NSLog(@"Error from TouchJSON: %@", potentialError);
#endif
        _failureBlock(potentialError);
        return;
    }
    
#ifdef RHITMobile_RHNetworkDebug
    NSLog(@"Successful response from server");
#endif
    
#ifdef RHITMobile_RHJSONDebug
    NSLog(@"Successful response from server: %@", jsonDict);
#endif
    _successBlock(jsonDict);
}

+ (void)updateNotificationStatus
{
    @synchronized ([RHJSONRequest class]) {
        if (_numRequests == 1) {
            [[UIApplication sharedApplication] setNetworkActivityIndicatorVisible:YES];
        } else if (_numRequests < 1) {
            [[UIApplication sharedApplication] setNetworkActivityIndicatorVisible:NO];
        }
    }
}

@end
