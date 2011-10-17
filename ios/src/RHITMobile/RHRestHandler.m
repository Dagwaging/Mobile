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
#import "RHConstants.h"
#import "RHPListStore.h"


#define kTopLevelServerPath @"/locations/data/top"

#define kLocationListKey @"Locations"
#define kNameKey @"Name"
#define kIdKey @"Id"
#define kDescriptionKey @"Description"
#define kMapAreaKey @"MapArea"
#define kMinZoomLevelKey @"MinZoomLevel"
#define kCenterKey @"Center"
#define kLatKey @"Lat"
#define kLongKey @"Long"
#define kCornersKey @"Corners"

#pragma mark Private Method Declarations

@interface RHRestHandler ()

@property (nonatomic, retain) NSManagedObjectContext *context;
@property (nonatomic, retain) NSOperationQueue *operations;
@property (nonatomic, retain) NSString *scheme;
@property (nonatomic, retain) NSString *host;
@property (nonatomic, retain) NSString *port;
@property (nonatomic, retain) RHPListStore *valueStore;

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

- (void)performCheckForLocationUpdates;

- (void)notifyDelegateViaSelector:(SEL)selector
               ofFailureWithError:(NSError *)error;

- (void)notifyDelegateViaSelector:(SEL)selector
             ofFailureWithMessage:(NSString *)message;

@end


#pragma mark -
#pragma mark Implementation

@implementation RHRestHandler

#pragma mark -
#pragma mark Generic Properties

@synthesize delegate;
@synthesize context;
@synthesize operations;
@synthesize scheme;
@synthesize host;
@synthesize port;
@synthesize valueStore;

#pragma mark -
#pragma mark General Methods

- (RHRestHandler *)initWithContext:(NSManagedObjectContext *)inContext
                          delegate:(RHRemoteHandlerDelegate *)inDelegate {
    self = [super init];
    
    if (self) {
        self.delegate = inDelegate;
        self.context = inContext;
        self.operations = [[NSOperationQueue new] autorelease];
        
        
        [NSUserDefaults resetStandardUserDefaults];
        NSUserDefaults *defaults = [NSUserDefaults standardUserDefaults];
        
        BOOL ssl = [defaults boolForKey:kRHPreferenceDebugServerProtocol];
        
        if (ssl) {
            self.scheme = @"https";
        } else {
            self.scheme = @"http";
        }
        
        self.host = [defaults objectForKey:kRHPreferenceDebugServerHostname];
        self.port = [defaults objectForKey:kRHPreferenceDebugServerPort];
        
        self.valueStore = [[[RHPListStore alloc] init] autorelease];
    }
    
    return self;
}

- (void)checkForLocationUpdates {
    if (self.delegate == nil) {
        return;
    }
    
    NSInvocationOperation* operation = [NSInvocationOperation alloc];
    operation = [[operation
                  initWithTarget:self
                  selector:@selector(performCheckForLocationUpdates)
                  object:nil] autorelease];
    [operations addOperation:operation];
}

- (void)dealloc {
    [delegate release];
    [context release];
    [operations release];
    [scheme release];
    [host release];
    [port release];
    [super dealloc];
}

#pragma mark -
#pragma mark Private Methods

- (void)performCheckForLocationUpdates {
    SEL failureSelector;
    failureSelector = @selector(didFailCheckingForLocationUpdatesWithError:);
    NSString *fullHost = [[NSString alloc] initWithFormat:@"%@:%@",
                          self.host,
                          self.port];
    
    NSString *currentVersion = self.valueStore.currentDataVersion;
    NSString *serverPath = nil;
    
    if (currentVersion == nil) {
        serverPath = kTopLevelServerPath;
    } else {
        NSString *escaped = [currentVersion
                             stringByAddingPercentEscapesUsingEncoding:NSUTF8StringEncoding];
        serverPath = [[[NSString alloc] initWithFormat:@"%@?version=%@",
                      kTopLevelServerPath, escaped] autorelease];
    }
    
    NSURL *url = [[NSURL alloc] initWithScheme:self.scheme
                                          host:fullHost
                                          path:serverPath];
    
    NSLog(@"Full URL: %@", url.absoluteString);
    
    [fullHost release];
    
    NSURLRequest *request = [NSURLRequest requestWithURL:url];
    [url release];
    
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

    
    NSInteger statusCode = ((NSHTTPURLResponse *) response).statusCode;
    
    if (statusCode == 204) {
        return;
    } else if (statusCode != 200) {
        NSString *errorString = [[NSString alloc] initWithFormat:@"Problem "
                                 "with server response:\nServer gave response "
                                 "code %d", statusCode];
        [self notifyDelegateViaSelector:failureSelector
                   ofFailureWithMessage:errorString];
        [errorString release];
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
    
    NSString *newVersion = [self stringFromDictionary:parsedData forKey:@"Version" withErrorSelector:failureSelector withErrorString:@"Couldn't find the thing"];
    
    self.valueStore.currentDataVersion = newVersion;
    
    NSArray *areas = [self arrayFromDictionary:parsedData
                                        forKey:kLocationListKey
                             withErrorSelector:failureSelector
                               withErrorString:@"Problem with server response:"
                                                "\nList of locations missing"];
    
    NSMutableSet *locations = [NSMutableSet setWithCapacity:[areas count]];
    
    // Populate new location objects for each retrieved dictionary
    for (NSDictionary *area in areas) {
        RHLocation *location = (RHLocation *) [RHLocation fromContext:context];
        
        // Name
        location.name = [self stringFromDictionary:area
                                            forKey:kNameKey
                                 withErrorSelector:failureSelector
                                   withErrorString:@"Problem with server "
                         "response:\nAt least one location is missings its "
                         "name"];
        
        // Server identifier
        location.serverIdentifier = [self numberFromDictionary:area
                                                        forKey:kIdKey
                                             withErrorSelector:failureSelector
                                               withErrorString:@"Problem with "
                                     "server response:\nAt least one location "
                                     "is missing its server identifier"];
        
        // Description
        location.quickDescription = [self stringFromDictionary:area
                                                        forKey:kDescriptionKey
                                             withErrorSelector:failureSelector
                                               withErrorString:@"Problem with "
                                     "server response:\nAt least one location "
                                     "is missing its description"];
        
        NSDictionary *mapArea = [self dictionaryFromDictionary:area
                                                        forKey:kMapAreaKey
                                             withErrorSelector:failureSelector
                                               withErrorString:@"Problem with "
                                 "server response:\nAt least one location is "
                                 "missing its map layout data"];
        
        if (![mapArea isKindOfClass:[NSNull class]]) {
            // Minimum zoom level
            location.visibleZoomLevel = [self numberFromDictionary:mapArea
                                                            forKey:kMinZoomLevelKey
                                                 withErrorSelector:failureSelector
                                                   withErrorString:@"Problem with "
                                         "server response:\nAt least one location "
                                         "is missing its minimum zoom level"];
            
            // Use boundary data to create boundary nodes
            NSArray *retrievedBoundary = [self arrayFromDictionary:mapArea
                                                            forKey:kCornersKey
                                                 withErrorSelector:failureSelector
                                                   withErrorString:@"Problem with "
                                          "server response:\nAt least one location "
                                          "is missings its boundary coordinates"];
            
            NSMutableArray *workingBoundary = [[[NSMutableArray alloc]
                                                initWithCapacity:[retrievedBoundary
                                                                  count]]
                                               autorelease];
            
            for (NSDictionary *nodeDict in retrievedBoundary) {
                RHBoundaryNode *node = (RHBoundaryNode *) [RHBoundaryNode
                                                           fromContext:context];
                
                node.latitude = [self numberFromDictionary:nodeDict
                                                    forKey:kLatKey
                                         withErrorSelector:failureSelector
                                           withErrorString:@"Problem with server "
                                 "response:\nAt least one location is missing a "
                                 "latitude component for one of its boundary "
                                 "coordinates"];
                
                node.longitude  = [self numberFromDictionary:nodeDict 
                                                      forKey:kLongKey
                                           withErrorSelector:failureSelector
                                             withErrorString:@"Problem with server "
                                   "response:\nAt least one location is missing a "
                                   "longitude component for one of its boundary "
                                   "coordinates"];
                
                [workingBoundary addObject:node];
            }
            
            location.orderedBoundaryNodes = workingBoundary;
        } else {
            location.visibleZoomLevel = [NSNumber numberWithInt:-1];
        }

        
        // Use center data to populate a center node
        NSDictionary *center = [self dictionaryFromDictionary:area
                                                       forKey:kCenterKey
                                            withErrorSelector:failureSelector
                                              withErrorString:@"Problem with "
                                "server response:\nAt least one location is "
                                "missing its center point"];
        
        RHLabelNode *centerNode = (RHLabelNode *) [RHLabelNode
                                                   fromContext:context];
        
        centerNode.latitude = [self numberFromDictionary:center
                                                  forKey:kLatKey
                                       withErrorSelector:failureSelector
                                         withErrorString:@"Problem with server "
                               "response:\nAt least one location is missing "
                               "the latitude component of its center point"];
        
        centerNode.longitude = [self numberFromDictionary:center
                                                   forKey:kLongKey
                                        withErrorSelector:failureSelector
                                          withErrorString:@"Problem with "
                                "server response:\nAt least one location is "
                                "missing the longitude component of its center "
                                "point"];
        
        location.labelLocation = centerNode;
        
        [locations addObject:location];
    }
    
    NSError *saveError = nil;
    [self.context save:&saveError];
    
    [delegate performSelectorOnMainThread:@selector(didFindMapLevelLocationUpdates:)
                               withObject:locations
                            waitUntilDone:NO];
}

#pragma mark-
#pragma mark Private Data Retrieval Methods

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
