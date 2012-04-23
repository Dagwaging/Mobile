//
//  RHWebRequestMaker.m
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

#import "CJSONDeserializer.h"
#import "NSDictionary_JSONExtensions.h"
#import "RHWebRequestMaker.h"


#define kURLScheme @"http"
#define kURLHost @"mobilewin.csse.rose-hulman.edu:5600"


@implementation RHWebRequestMaker

+ (NSDictionary *)JSONGetRequestWithPath:(NSString *)path
                                 URLargs:(NSString *)args {
    
    NSURL *url = [[NSURL alloc] initWithScheme:kURLScheme
                                          host:kURLHost
                                          path:[path stringByAppendingString:args]];
    
    //NSLog(@"Web request: %@", url.absoluteString);
    
    NSURLRequest *request = [NSURLRequest requestWithURL:url];
    
    NSURLResponse *response = nil;
    
    // Because this will be done on a background thread, we can use a
    // synchronous request for our data retrieval step.
    NSData *data = [NSURLConnection sendSynchronousRequest:request
                                         returningResponse:&response
                                                     error:nil];
    
    
    NSInteger statusCode = ((NSHTTPURLResponse *) response).statusCode;
    
    if (statusCode != 200) {
        NSLog(@"Non-okay status code: %d", statusCode);
        return [NSDictionary dictionary];
    }

    return [NSDictionary dictionaryWithJSONData:data error:nil];
}

@end
