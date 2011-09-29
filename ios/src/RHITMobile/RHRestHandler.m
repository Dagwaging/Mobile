//
//  RHRestHandler.m
//  RHIT Mobile Campus Directory
//
//  Copyright 2011 Rose-Hulman Institute of Technology
// 
//  Licensed under the Apache License, Version 2.0 (the "License");
//  you may not use this file except in compliance with the License.
//  You may obtain a copy of the License at
//
//  http://www.apache.org/licenses/LICENSE-2.0
//
//  Unless required by applicable law or agreed to in writing, software
//  distributed under the License is distributed on an "AS IS" BASIS,
//  WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//  See the License for the specific language governing permissions and
//  limitations under the License.
//

#import "CJSONDeserializer.h"
#import "NSDictionary_JSONExtensions.h"

#import "RHRestHandler.h"
#import "RHRemoteHandlerDelegate.h"
#import "RHBoundaryNode.h"
#import "RHLabelNode.h"
#import "RHLocation.h"


@interface RHRestHandler ()

@property (nonatomic, retain) NSManagedObjectContext *context;
@property (nonatomic, retain) NSOperationQueue *operations;

- (NSString *)stringFromDictionary:(NSDictionary *)dictionary
                            forKey:(NSString *)key
                 withErrorSelector:(SEL)selector
                   withErrorString:(NSString *)errorString;

- (NSNumber *)numberFromDictionary:(NSDictionary *)dictionary
                            forKey:(NSString *)key
                 withErrorSelector:(SEL)selector
                   withErrorString:(NSString *)erorString;

- (NSArray *)arrayFromDictionary:(NSDictionary *)dictionary
                          forKey:(NSString *)key
               withErrorSelector:(SEL)selector
                 withErrorString:(NSString *)errorString;

- (NSDictionary *)dictionaryFromDictionary:(NSDictionary *)dictionary
                                    forKey:(NSString *)key
                         withErrorSelector:(SEL)selector
                           withErrorString:(NSString *)errorString;

- (void)performFetchAllLocations;

- (void)performCheckForNewLocations;

- (void)notifyDelegateViaSelector:(SEL)selector
               ofFailureWithError:(NSError *)error;

- (void)notifyDelegateViaSelector:(SEL)selector
             ofFailureWithMessage:(NSString *)message;

@end


@implementation RHRestHandler

@synthesize delegate;
@synthesize context;
@synthesize operations;

- (id)initWithContext:(NSManagedObjectContext *)inContext
             delegate:(RHRemoteHandlerDelegate *)inDelegate {
    self = [super init];
    
    if (self) {
        self.delegate = inDelegate;
        self.context = inContext;
        self.operations = [[NSOperationQueue new] autorelease];
    }
    
    return self;
}

- (void)fetchAllLocations {
    if (self.delegate == nil) {
        return;
    }
    
    NSInvocationOperation* operation = [NSInvocationOperation alloc];
    operation = [[operation initWithTarget:self
                                  selector:@selector(performFetchAllLocations)
                                    object:nil] autorelease];
    [operations addOperation:operation];
}

- (void)checkForNewLocations {
    if (self.delegate == nil) {
        return;
    }
    
    NSInvocationOperation* operation = [NSInvocationOperation alloc];
    operation = [[operation initWithTarget:self
                                  selector:@selector(performCheckForNewLocations)
                                    object:nil] autorelease];
    [operations addOperation:operation];
}

#pragma mark -
#pragma mark Private Methods

- (void)performFetchAllLocations {
    SEL failureSelector = @selector(didFailFetchingAllLocationsWithError:);
    NSURL *url = [NSURL URLWithString:@"http://mobilewin.csse.rose-hulman.edu:5600/mapareas"];
    
    NSURLRequest *request = [NSURLRequest requestWithURL:url];
    NSURLResponse *response = nil;
    NSError *error = nil;
    
    // Because this will be done on a background thread, we can use a
    // synchronous request for our data retrieval step.
    NSData *data = [NSURLConnection sendSynchronousRequest:request
                                         returningResponse:&response
                                                     error:&error];
    if (data == nil) {
        [self notifyDelegateViaSelector:failureSelector
                     ofFailureWithError:error];
        return;
    }
    
    error = nil;
    NSDictionary *parsedData = [NSDictionary dictionaryWithJSONData:data
                                                              error:&error];
    
    if (parsedData == nil) {
        [self notifyDelegateViaSelector:failureSelector
                     ofFailureWithError:error];
        return;
    }
    
    NSArray *areas = [self arrayFromDictionary:parsedData
                                        forKey:@"Areas"
                             withErrorSelector:failureSelector
                               withErrorString:@"Problem with server response:"
                                                "\nList of locations missing"];
    
    NSMutableSet *locations = [NSMutableSet setWithCapacity:[areas count]];
    
    for (NSDictionary *area in areas) {
        NSString *name = [area valueForKey:@"Name"];
        if (name == nil) {
            NSString *message = @"Problem with server response:\nAt least one "
            "location is missing the location of its name";
            [self notifyDelegateViaSelector:failureSelector
                       ofFailureWithMessage:message];
            return;
        }
        
        NSNumber *serverId = [area valueForKey:@"Id"];
        
        if (serverId == nil) {
            NSString *message = [NSString
                                 stringWithFormat:@"Problem with server "
                                 "response:\nLocation \"%@\" is missing its "
                                 "server ID", name];
            [self notifyDelegateViaSelector:failureSelector
                       ofFailureWithMessage:message];
            return;
        }
        
        NSString *description = [area valueForKey:@"Description"];
        
        if (description == nil) {
            NSString *message = [NSString
                                 stringWithFormat:@"Problem with server "
                                 "response:\nLocation \"%@\" is missing its "
                                 "description", name];
            [self notifyDelegateViaSelector:failureSelector
                       ofFailureWithMessage:message];
            return;
        }
        
        NSNumber *minZoomLevel = [area valueForKey:@"MinZoomLevel"];
        
        if (minZoomLevel == nil) {
            NSString *message = [NSString
                                 stringWithFormat:@"Problem with server "
                                 "response:\nLocation \"%@\" is missing its "
                                 "minimum zoom level", name];
            [self notifyDelegateViaSelector:failureSelector
                       ofFailureWithMessage:message];
            return;
        }
        
        NSDictionary *center = [area valueForKey:@"Center"];
        
        if (center == nil) {
            NSString *message = [NSString
                                 stringWithFormat:@"Problem with server "
                                 "response:\nLocation \"%@\" is missing the "
                                 "coordinates of its label.", name];
            [self notifyDelegateViaSelector:failureSelector
                       ofFailureWithMessage:message];
            return;
        }
        
        RHLabelNode *centerNode = (RHLabelNode *) [RHLabelNode
                                                   fromContext:context];
        
        centerNode.latitude = [center valueForKey:@"Lat"];
        
        if (centerNode.latitude == nil) {
            NSString *message = [NSString
                                 stringWithFormat:@"Problem with server "
                                 "response:\nLocation \"%@\" is missing the "
                                 "latitude component of its label coordinate.",
                                 name];
            [self notifyDelegateViaSelector:failureSelector
                       ofFailureWithMessage:message];
            return;
        }
        
        centerNode.longitude = [center valueForKey:@"Long"];
        
        if (centerNode.longitude == nil) {
            NSString *message = [NSString
                                 stringWithFormat:@"Problem with server "
                                 "response:\nLocation \"%@\" is missing the "
                                 "longitude component of its label coordinate.",
                                 name];
            [self notifyDelegateViaSelector:failureSelector
                       ofFailureWithMessage:message];
            return;
        }
        
        NSArray *retrievedBoundary = [area valueForKey:@"Corners"];
        
        if (retrievedBoundary == nil) {
            NSString *message = [NSString
                                 stringWithFormat:@"Problem with server "
                                 "response:\nLocation \"%@\" is missing its "
                                 "boundary coordinates",
                                 name];
            [self notifyDelegateViaSelector:failureSelector
                       ofFailureWithMessage:message];
            return;
        }
        
        NSMutableArray *workingBoundary = [[[NSMutableArray alloc]
                                            initWithCapacity:[retrievedBoundary
                                                              count]]
                                           autorelease];
        
        for (NSDictionary *nodeDict in retrievedBoundary) {
            RHBoundaryNode *node = (RHBoundaryNode *) [RHBoundaryNode
                                                       fromContext:context];
            
            NSNumber *nodeLat = [nodeDict valueForKey:@"Lat"];
            
            if (nodeLat == nil) {
                NSString *message = [NSString
                                     stringWithFormat:@"Problem with server "
                                     "response:\nLocation \"%@\" is missing "
                                     "the latitude component of one of its "
                                     "boundary coordinates", name];
                [self notifyDelegateViaSelector:failureSelector
                           ofFailureWithMessage:message];
                return;
            }
            
            NSNumber *nodeLong = [nodeDict valueForKey:@"Long"];
            
            if (nodeLong == nil) {
                NSString *message = [NSString
                                     stringWithFormat:@"Problem with server "
                                     "response:\nLocation \"%@\" is missing "
                                     "the longitude component of one of its "
                                     "boundary coordinates", name];
                [self notifyDelegateViaSelector:failureSelector
                           ofFailureWithMessage:message];
                return;
            }
            
            node.latitude = nodeLat;
            node.longitude = nodeLong;
            
            [workingBoundary addObject:node];
        }
        
        RHLocation *location = (RHLocation *) [RHLocation fromContext:context];
        
        location.labelLocation = centerNode;
        location.serverIdentifier = serverId;
        location.name = name;
        location.quickDescription = description;
        location.visibleZoomLevel = minZoomLevel;
        location.orderedBoundaryNodes = workingBoundary;
        
        [locations addObject:location];
    }
    
    [delegate performSelectorOnMainThread:@selector(didFetchAllLocations:)
                               withObject:locations
                            waitUntilDone:NO];
}

- (void)performCheckForNewLocations {
    // TODO
}

- (NSString *)stringFromDictionary:(NSDictionary *)dictionary
                            forKey:(NSString *)key
                 withErrorSelector:(SEL)selector
                   withErrorString:(NSString *)errorString {
    NSString *result = [dictionary valueForKey:key];
    
    if (result == nil) {
        [self notifyDelegateViaSelector:selector
                   ofFailureWithMessage:errorString];
    }
    
    return result;
}

- (NSNumber *)numberFromDictionary:(NSDictionary *)dictionary
                            forKey:(NSString *)key
                 withErrorSelector:(SEL)selector
                   withErrorString:(NSString *)erorString {
    NSNumber *result = [dictionary valueForKey:key];
    
    if (result == nil) {
        [self notifyDelegateViaSelector:selector
                   ofFailureWithMessage:erorString];
    }
    
    return result;
}

- (NSArray *)arrayFromDictionary:(NSDictionary *)dictionary
                          forKey:(NSString *)key
               withErrorSelector:(SEL)selector
                 withErrorString:(NSString *)errorString {
    NSArray *result = [dictionary valueForKey:key];
    
    if (result ==  nil) {
        [self notifyDelegateViaSelector:selector
                   ofFailureWithMessage:errorString];
    }
    
    return result;
}

- (NSDictionary *)dictionaryFromDictionary:(NSDictionary *)dictionary
                                    forKey:(NSString *)key
                         withErrorSelector:(SEL)selector
                           withErrorString:(NSString *)errorString {
    NSDictionary *result = [dictionary valueForKey:key];
    
    if (result == nil) {
        [self notifyDelegateViaSelector:selector
                   ofFailureWithMessage:errorString];
    }
    
    return result;
}

- (void)notifyDelegateViaSelector:(SEL)selector
               ofFailureWithError:(NSError *)error {
    [delegate performSelectorOnMainThread:selector
                               withObject:error
                            waitUntilDone:NO];
}

- (void)notifyDelegateViaSelector:(SEL)selector
             ofFailureWithMessage:(NSString *)message {
    NSDictionary *userInfo = [NSDictionary
                              dictionaryWithObject:message
                              forKey:NSLocalizedDescriptionKey];
    NSError *error = [NSError errorWithDomain:@"edu.rosehulman.ios.mobile"
                                         code:0
                                     userInfo:userInfo];
    [delegate performSelectorOnMainThread:selector
                               withObject:error
                            waitUntilDone:NO];
}

@end
