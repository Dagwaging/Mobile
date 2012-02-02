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
#import "RHITMobileAppDelegate.h"
#import "RHLocationLink.h"
#import "RHSearchViewController.h"
#import "RHWebRequestMaker.h"
#import "RHMapViewController.h"


#define kTopLevelServerPath @"/locations/data/top"
#define kUnderlyingLocationServerPath @"/locations/data/within/%d"
#define kSearchLocationsServerPath @"/locations/names"

#define kLocationListKey @"Locations"
#define kNameKey @"Name"
#define kIdKey @"Id"
#define kAltNamesKey @"AltNames"
#define kLinksKey @"Links"
#define kLinkNameKey @"Name"
#define kLinkURLKey @"Url"
#define kParentKey @"Parent"
#define kDescriptionKey @"Description"
#define kDisplayTypeKey @"Type"
#define kDisplayTypeNormal @"NL"
#define kDisplayTypePointOfInterest @"PI"
#define kDisplayTypeQuickList @"QL"
#define kMapAreaKey @"MapArea"
#define kMinZoomLevelKey @"MinZoomLevel"
#define kCenterKey @"Center"
#define kLatKey @"Lat"
#define kLongKey @"Lon"
#define kCornersKey @"Corners"
#define kNamesKey @"Names"

#pragma mark Private Method Declarations

@interface RHRestHandler ()

@property (nonatomic, strong) NSPersistentStoreCoordinator *coordinator;
@property (nonatomic, strong) NSOperationQueue *operations;
@property (nonatomic, strong) NSString *scheme;
@property (nonatomic, strong) NSString *host;
@property (nonatomic, strong) NSString *port;
@property (nonatomic, strong) RHPListStore *valueStore;
@property (nonatomic, strong) RHSearchViewController *searchViewController;

- (RHLocation *)locationFromDictionary:(NSDictionary *)dictionary
                           withContext:(NSManagedObjectContext *)context
                     withLocationsByID:(NSMutableDictionary *)locationsByID
                   withFailureSelector:(SEL)selector;

- (BOOL)booleanFromDictionary:(NSDictionary *)dictionary
                       forKey:(NSString *)key
            withErrorSelector:(SEL)selector
              withErrorString:(NSString *)errorString;

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

- (void)performPopulateUnderlyingLocations;

- (void)performRushPopulateLocationsUnderLocationWithID:(NSManagedObjectID *)objectID;

- (void)performSearchForLocations:(NSString *)searchTerm;

- (void)notifyDelegateViaSelector:(SEL)selector
               ofFailureWithError:(NSError *)error;

- (void)notifyDelegateViaSelector:(SEL)selector
             ofFailureWithMessage:(NSString *)message;

@end


#pragma mark - Implementation

@implementation RHRestHandler

#pragma mark - Generic Properties

@synthesize delegate;
@synthesize coordinator;
@synthesize operations;
@synthesize scheme;
@synthesize host;
@synthesize port;
@synthesize valueStore;
@synthesize searchViewController;

#pragma mark - General Methods

- (RHRestHandler *)initWithPersistantStoreCoordinator:(NSPersistentStoreCoordinator *)inCoordinator
                                             delegate:(RHMapViewController *)inDelegate {
    self = [super init];
    
    if (self) {
        self.delegate = inDelegate;
        self.coordinator = inCoordinator;
        self.operations = [NSOperationQueue new];
        
        [NSUserDefaults resetStandardUserDefaults];
        
        self.scheme = @"http";
        
        self.host = @"mobilewin.csse.rose-hulman.edu";
        self.port = @"5600";
        
        self.valueStore = [[RHPListStore alloc] init];
    }
    
    return self;
}

- (void)checkForLocationUpdates {
    if (self.delegate == nil) {
        return;
    }
    
    NSInvocationOperation* operation = [NSInvocationOperation alloc];
    operation = [operation
                  initWithTarget:self
                  selector:@selector(performCheckForLocationUpdates)
                  object:nil];
    [operations addOperation:operation];
}

- (void)populateUnderlyingLocations {
    if (self.delegate == nil) {
        return;
    }
    
    NSInvocationOperation* operation = [NSInvocationOperation alloc];
    operation = [operation
                  initWithTarget:self
                  selector:@selector(performPopulateUnderlyingLocations)
                  object:nil];
    [operations addOperation:operation];
}

- (void)rushPopulateLocationsUnderLocationWithID:(NSManagedObjectID *)objectID {
    if (self.delegate == nil) {
        return;
    }
    
    NSInvocationOperation* operation = [NSInvocationOperation alloc];
    operation = [operation
                  initWithTarget:self
                  selector:@selector(performRushPopulateLocationsUnderLocationWithID:)
                  object:objectID];
    [operations addOperation:operation];
}


#pragma mark - Private Methods

- (RHLocation *)locationFromDictionary:(NSDictionary *)dictionary
                           withContext:(NSManagedObjectContext *)context
                     withLocationsByID:(NSMutableDictionary *)locationsByID
                   withFailureSelector:(SEL)failureSelector {
    
    // Fetch the server identifier first to see if we need to fetch at all
    NSNumber *serverIdentifier = [self numberFromDictionary:dictionary
                                                    forKey:kIdKey
                                         withErrorSelector:failureSelector
                                           withErrorString:@"Problem with "
                                 "server response:\nAt least one location "
                                 "is missing its server identifier"];
    
    // Check to see if this location already exists
    NSEntityDescription *entityDescription = [NSEntityDescription
                                              entityForName:@"Location"
                                              inManagedObjectContext:context];
    NSFetchRequest *fetchRequest = [[NSFetchRequest alloc] init];
    [fetchRequest setEntity:entityDescription];
    
    NSPredicate *predicate = [NSPredicate predicateWithFormat:
                              @"serverIdentifier == %d", serverIdentifier.intValue];
    [fetchRequest setPredicate:predicate];
    
    NSArray *potentialMatches = [context executeFetchRequest:fetchRequest
                                                       error:nil];
    
    if (potentialMatches.count > 0) {
        return nil;
    }
    
    RHLocation *location = (RHLocation *) [RHLocation fromContext:context];
    
    location.serverIdentifier = serverIdentifier;
    
    // Name
    location.name = [self stringFromDictionary:dictionary
                                        forKey:kNameKey
                             withErrorSelector:failureSelector
                               withErrorString:@"Problem with server "
                     "response:\nAt least one location is missings its "
                     "name"];

    
    // Alternate Names
    location.alternateNames = [self arrayFromDictionary:dictionary
                                                 forKey:kAltNamesKey
                                      withErrorSelector:failureSelector
                                        withErrorString:@"Problem with server "
                               "response:\nAt least one location is missing "
                               "its alternate names attribute"];
    
    // Links
    NSArray *links = [self arrayFromDictionary:dictionary
                                        forKey:kLinksKey
                             withErrorSelector:failureSelector
                               withErrorString:@"Problem with server "
                      "response:\nAt least one location is missints its "
                      "links attribute"];
    
    for (NSDictionary *dictionary in links) {
        RHLocationLink *link = [RHLocationLink linkFromContext:context];
        
        link.name = [self stringFromDictionary:dictionary forKey:kLinkNameKey withErrorSelector:failureSelector withErrorString:@"Problem with server response:\nMissing link name"];
        link.url = [self stringFromDictionary:dictionary forKey:kLinkURLKey withErrorSelector:failureSelector withErrorString:@"Problem with server response:\nMissing link URL"];
        link.owner = location;
    }
    
    // Description
    location.quickDescription = [self stringFromDictionary:dictionary
                                                    forKey:kDescriptionKey
                                         withErrorSelector:failureSelector
                                           withErrorString:@"Problem with "
                                 "server response:\nAt least one location "
                                 "is missing its description"];
    
    NSString *displayType = [self stringFromDictionary:dictionary
                                                forKey:kDisplayTypeKey
                                     withErrorSelector:failureSelector
                                       withErrorString:@"Problem with "
                             "server response:\nAt least one location "
                             "is missings its display type"];
    
    if ([displayType isEqual:kDisplayTypeNormal]) {
        location.displayType = RHLocationDisplayTypeNone;
    } else if ([displayType isEqual:kDisplayTypePointOfInterest]) {
        location.displayType = RHLocationDisplayTypePointOfInterest;
    } else if ([displayType isEqual:kDisplayTypeQuickList]) {
        location.displayType = RHLocationDisplayTypeQuickList;
    } else {
        [self notifyDelegateViaSelector:failureSelector
                   ofFailureWithMessage:@"Problem with server "
         "response:\nInvalid location display type"];
    }
    
    NSDictionary *mapArea = [self dictionaryFromDictionary:dictionary
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
        
        NSMutableArray *workingBoundary = [[NSMutableArray alloc]
                                            initWithCapacity:[retrievedBoundary
                                                              count]];
        
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
    NSDictionary *center = [self dictionaryFromDictionary:dictionary
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
    
    location.retrievalStatus = RHLocationRetrievalStatusNoChildren;
    
    id parentId = [dictionary objectForKey:kParentKey];
    
    if (![parentId isKindOfClass:[NSNull class]] && locationsByID != nil) {
        NSNumber *parentIdentifer = (NSNumber *)parentId;
        RHLocation *potentialParent = (RHLocation *)[locationsByID
                                                      objectForKey:parentIdentifer.stringValue];
        if (potentialParent == nil) {
            RHLocation *parent = [RHLocation fromContext:context];
            parent.serverIdentifier = parentIdentifer;
            parent.retrievalStatus = RHLocationRetrievalStatusServerIDOnly;
            [parent addEnclosedLocationsObject:location];
            location.parent = parent;
        } else {
            [potentialParent addEnclosedLocationsObject:location];
            location.parent = potentialParent;
        }
    }
    
    [locationsByID setValue:location
                     forKey:location.serverIdentifier.stringValue];
    
    return location;
}

- (void)performCheckForLocationUpdates {
    
    @synchronized(self) {
        NSManagedObjectContext *context = [[NSManagedObjectContext alloc] init];
        context.persistentStoreCoordinator = self.coordinator;
        
        SEL failureSelector;
        failureSelector = @selector(didFailCheckingForLocationUpdatesWithError:);
        NSString *fullHost = [[NSString alloc] initWithFormat:@"%@:%@",
                              self.host,
                              self.port];
        
        NSString *currentVersion = self.valueStore.currentMapDataVersion;
        NSString *serverPath = nil;
        
        if (currentVersion == nil) {
            serverPath = kTopLevelServerPath;
        } else {
            NSString *escaped = [currentVersion
                                 stringByAddingPercentEscapesUsingEncoding:NSUTF8StringEncoding];
            serverPath = [[NSString alloc] initWithFormat:@"%@?version=%@",
                           kTopLevelServerPath, escaped];
        }
        
        NSURL *url = [[NSURL alloc] initWithScheme:self.scheme
                                              host:fullHost
                                              path:serverPath];
        
        
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
        
        
        NSInteger statusCode = ((NSHTTPURLResponse *) response).statusCode;
        
        if (statusCode == 204) {
            return;
        } else if (statusCode != 200) {
            NSString *errorString = [[NSString alloc] initWithFormat:@"Problem "
                                     "with server response:\nServer gave response "
                                     "code %d", statusCode];
            [self notifyDelegateViaSelector:failureSelector
                       ofFailureWithMessage:errorString];
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
        
        RHITMobileAppDelegate *appDelegate;
        appDelegate = (RHITMobileAppDelegate *)[[UIApplication
                                                 sharedApplication] delegate];
        
        [appDelegate performSelectorOnMainThread:@selector(clearDatabase)
                                      withObject:nil
                                   waitUntilDone:YES];
        
        NSString *newVersion = [self stringFromDictionary:parsedData
                                                   forKey:@"Version"
                                        withErrorSelector:failureSelector
                                          withErrorString:@"Problem with server "
                                "response:\nNo data version number specified"];
        
        NSArray *areas = [self arrayFromDictionary:parsedData
                                            forKey:kLocationListKey
                                 withErrorSelector:failureSelector
                                   withErrorString:@"Problem with server response:"
                          "\nList of locations missing"];
        
        NSMutableSet *locations = [NSMutableSet setWithCapacity:[areas count]];
        
        NSMutableDictionary *locationsByID = [[NSMutableDictionary alloc] init];
        
        // Populate new location objects for each retrieved dictionary
        for (NSDictionary *area in areas) {
            RHLocation *location = [self locationFromDictionary:area
                                                    withContext:context
                                               withLocationsByID:locationsByID
                                             withFailureSelector:failureSelector];
            if (location != nil) {
                [locations addObject:location];    
            }
        }
        
        NSError *saveError = nil;
        [context save:&saveError];
        
        self.valueStore.currentMapDataVersion = newVersion;
        
        [delegate performSelectorOnMainThread:@selector(didFindMapLevelLocationUpdates)
                                   withObject:nil
                                waitUntilDone:NO];
        
        [self populateUnderlyingLocations];
    }
}

- (void)performPopulateUnderlyingLocations {
    @synchronized(self) {
        SEL failureSelector;
        failureSelector = @selector(didFailCheckingForLocationUpdatesWithError:);
        
        NSManagedObjectContext *context = [[NSManagedObjectContext alloc] init];
        context.persistentStoreCoordinator = self.coordinator;
        
        NSString *fullHost = [[NSString alloc] initWithFormat:@"%@:%@",
                              self.host,
                              self.port];
        
        // Get all locations that we may need to populate
        NSEntityDescription *entityDescription = [NSEntityDescription
                                                  entityForName:@"Location"
                                                  inManagedObjectContext:context];
        NSFetchRequest *fetchRequest = [[NSFetchRequest alloc] init];
        [fetchRequest setEntity:entityDescription];

        NSPredicate *predicate = [NSPredicate predicateWithFormat:
                                  @"retrievalStatusNumber == %d",
                                  RHLocationRetrievalStatusNoChildren];
        [fetchRequest setPredicate:predicate];
        
        NSArray *locationsToPopulate = [context executeFetchRequest:fetchRequest
                                                              error:nil];
        
        // Populate and save each location
        for (__strong RHLocation *location in locationsToPopulate) {
            
            location = (RHLocation *)[context objectWithID:location.objectID];
            
            if (location.retrievalStatus == RHLocationRetrievalStatusFull) {
                continue;
            }
            
            NSString *serverPath = [[NSString alloc] initWithFormat:kUnderlyingLocationServerPath, location.serverIdentifier.intValue];
            
            
            NSURL *url = [[NSURL alloc] initWithScheme:self.scheme
                                                  host:fullHost
                                                  path:serverPath];

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
            
            NSDictionary *parsedData = [NSDictionary dictionaryWithJSONData:data
                                                                      error:nil];
            
            NSArray* children = [self arrayFromDictionary:parsedData
                                                   forKey:kLocationListKey
                                        withErrorSelector:failureSelector
                                          withErrorString:@"Problem with "
                                 "server response:\nNo children locations "
                                 "attribute found"];
            
            for (NSDictionary *dictionary in children) {
                RHLocation *childLocation = [self
                                             locationFromDictionary:dictionary
                                             withContext:context
                                             withLocationsByID:nil
                                             withFailureSelector:failureSelector];
                if (childLocation != nil) {
                    childLocation.parent = location;
                }
            }
            
            location.retrievalStatus = RHLocationRetrievalStatusFull;
            [context save:nil];
        }
    }
    
    [RHITMobileAppDelegate.instance prefetchLocationNames];
}

- (void)performRushPopulateLocationsUnderLocationWithID:(NSManagedObjectID *)objectID {
    SEL failureSelector = @selector(didFailPopulatingLocationsWithError:);
    
    NSManagedObjectContext *context = [[NSManagedObjectContext alloc] init];
    context.persistentStoreCoordinator = self.coordinator;
    
    NSString *fullHost = [[NSString alloc] initWithFormat:@"%@:%@",
                           self.host,
                           self.port];
    
    RHLocation *location = (RHLocation *) [context objectWithID:objectID];
    
    if (location.retrievalStatus == RHLocationRetrievalStatusFull) {
        return;
    }

    NSString *serverPath = [[NSString alloc] initWithFormat:kUnderlyingLocationServerPath, location.serverIdentifier.intValue];
    
    
    NSURL *url = [[NSURL alloc] initWithScheme:self.scheme
                                          host:fullHost
                                          path:serverPath];
    
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
    
    NSDictionary *parsedData = [NSDictionary dictionaryWithJSONData:data
                                                              error:nil];
    
    NSArray* children = [self arrayFromDictionary:parsedData
                                           forKey:kLocationListKey
                                withErrorSelector:failureSelector
                                  withErrorString:@"Problem with "
                         "server response:\nNo children locations "
                         "attribute found"];
    
    for (NSDictionary *dictionary in children) {
        RHLocation *childLocation = [self
                                     locationFromDictionary:dictionary
                                     withContext:context
                                     withLocationsByID:nil
                                     withFailureSelector:failureSelector];
        if (childLocation != nil) {
            childLocation.parent = location;
        }
    }
    
    location.retrievalStatus = RHLocationRetrievalStatusFull;
    [context save:nil];
}

- (void)searchForLocations:(NSString *)searchTerm searchViewController:(RHSearchViewController *)inSearchViewController {
    if (self.delegate == nil) {
        return;
    }
    
    self.searchViewController = inSearchViewController;

    NSInvocationOperation* operation = [NSInvocationOperation alloc];
    operation = [operation initWithTarget:self
                                  selector:@selector(performSearchForLocations:)
                                    object:searchTerm];
    [operations addOperation:operation];
}

- (void)performSearchForLocations:(NSString *)searchTerm {
    
    NSManagedObjectContext *context = [[NSManagedObjectContext alloc] init];
    context.persistentStoreCoordinator = self.coordinator;
    
    NSString *tokenizedTerm = [searchTerm stringByReplacingOccurrencesOfString:@" " withString:@"+"];
    NSString *sanitizedSearchTerm = [tokenizedTerm stringByAddingPercentEscapesUsingEncoding:NSUTF8StringEncoding];
    NSString *urlArgs = [NSString stringWithFormat:@"?s=%@", sanitizedSearchTerm];
    
    NSDictionary *jsonData = [RHWebRequestMaker JSONGetRequestWithPath:kSearchLocationsServerPath URLargs:urlArgs];
    
    if (jsonData == nil) {
        NSLog(@"No search data returned. Bailing out...");
        return;
    }
    
    NSArray *locations = [jsonData objectForKey:kNamesKey];
    
    NSMutableArray *result = [NSMutableArray arrayWithCapacity:locations.count];    
    
    for (NSDictionary *location in locations) {
        NSEntityDescription *entityDescription = [NSEntityDescription
                                                  entityForName:@"Location"
                                                  inManagedObjectContext:context];
        
        NSFetchRequest *fetchRequest = [[NSFetchRequest alloc] init];
        [fetchRequest setEntity:entityDescription];
        
        NSPredicate *predicate = [NSPredicate predicateWithFormat:
                                  @"serverIdentifier == %d", [[location objectForKey:kIdKey] intValue]];
        [fetchRequest setPredicate:predicate];
        
        NSArray *results = [context executeFetchRequest:fetchRequest error:nil];
        
        if (results.count > 0) {
            RHLocation *matchingLocation = [results objectAtIndex:0];
            [result addObject:matchingLocation.objectID];
        }
    }
    
    [self.searchViewController performSelectorOnMainThread:@selector(didFindSearchResults:) 
                                                withObject:result
                                             waitUntilDone:NO];
}

#pragma mark - Private Data Retrieval Methods

- (BOOL)booleanFromDictionary:(NSDictionary *)dictionary
                       forKey:(NSString *)key
            withErrorSelector:(SEL)selector
              withErrorString:(NSString *)errorString {
    NSNumber *result = [dictionary valueForKey:key];
    
    if (result == nil) {
        [self notifyDelegateViaSelector:selector
                   ofFailureWithMessage:errorString];
    }
    
    return [result boolValue];
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
    
    if (result == nil) {
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
