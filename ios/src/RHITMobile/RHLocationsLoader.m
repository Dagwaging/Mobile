//
//  RHLocationsLoader.m
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

#import "RHLocationsLoader.h"
#import "RHAppDelegate.h"
#import "RHBoundaryNode.h"
#import "RHConstants.h"
#import "RHDataVersionsLoader.h"
#import "RHLabelNode.h"
#import "RHLoaderRequestsWrapper.h"
#import "RHLocation.h"
#import "RHLocationLink.h"

#define kLocationsKey @"Locations"
#define kVersionKey @"Version"
#define kIDKey @"Id"
#define kNameKey @"Name"
#define kAltNamesKey @"AltNames"
#define kLinksKey @"Links"
#define kLinkNameKey @"Name"
#define KLinkURLKey @"Url"
#define kDescriptionKey @"Description"
#define kDepartableKey @"IsDepartable"
#define kDisplayTypeKey @"Type"
#define kDisplayTypeNormal @"NL"
#define kDisplayTypePointOfInterest @"PI"
#define kDisplayTypeQuickList @"QL"
#define kMapAreaKey @"MapArea"
#define kMinZoomLevelKey @"MinZoomLevel"
#define kCornersKey @"Corners"
#define kNodeLatKey @"Lat"
#define kNodeLongKey @"Lon"
#define kCenterKey @"Center"
#define kParentKey @"Parent"


@interface RHLocationsLoader ()

@property (nonatomic, strong) NSMutableSet *topLevelDelegates;

@property (nonatomic, strong) NSMutableSet *allInternalDelegates;

@property (nonatomic, strong) NSMutableDictionary *individualLocationDelegates;

- (RHLocation *)locationFromJSONResponse:(NSDictionary *)jsonResponse
                  inManagedObjectContext:(NSManagedObjectContext *)localContext;

- (void)deleteAllLocations:(NSManagedObjectContext *)managedObjectContext;

- (NSManagedObjectContext *)createThreadSafeManagedObjectContext;

@end


@implementation RHLocationsLoader

@synthesize currentlyUpdating = _currentlyUpdating;
@synthesize topLevelDelegates = _topLevelDelegates;
@synthesize allInternalDelegates = _allInternalDelegates;
@synthesize individualLocationDelegates = _individualLocationDelegates;

static RHLocationsLoader *_instance;

+ (void)initialize
{
    static BOOL initialized = NO;
    if(!initialized)
    {
        initialized = YES;
        _instance = [[RHLocationsLoader alloc] init];
    }
}

+ (id)instance
{
    return _instance;
}

- (id)init
{
    if (self = [super init]) {
        _currentlyUpdating = NO;
    }
    
    return self;
}

- (NSMutableSet *)topLevelDelegates
{
    if (_topLevelDelegates == nil) {
        _topLevelDelegates = [NSMutableSet set];
    }
    
    return _topLevelDelegates;
}

- (NSMutableSet *)allInternalDelegates
{
    if (_allInternalDelegates == nil) {
        _allInternalDelegates = [NSMutableSet set];
    }
    
    return _allInternalDelegates;
}

- (NSMutableDictionary *)individualLocationDelegates
{
    if (_individualLocationDelegates == nil) {
        _individualLocationDelegates = [NSMutableDictionary dictionary];
    }
    
    return _individualLocationDelegates;
}

- (void)updateLocations:(NSNumber *)version
{
    if (_currentlyUpdating) {
        return;
    }
    
    // Only operate on background thread
    if ([NSThread isMainThread]) {
        [self performSelectorInBackground:@selector(updateLocations:) withObject:nil];
        return;
    }
    
    _currentlyUpdating = YES;
    
    NSManagedObjectContext *localContext = [self createThreadSafeManagedObjectContext];
    NSError *currentError = nil;
    
    NSDictionary *jsonResponse = [RHLoaderRequestsWrapper makeSynchronousTopLocationsRequestWithWithVersion:version error:&currentError];
    
    if (currentError) {
        NSLog(@"Problem updating top level locations: %@", currentError);
        _currentlyUpdating = NO;
        return;
    }
    
    // Retrieve the new version
    NSNumber *newVersion = [jsonResponse objectForKey:kVersionKey];
    
    if (newVersion.doubleValue <= 0) {
        NSLog(@"New version not specified in top locations. Bailing");
        _currentlyUpdating = NO;
        return;
    }
    
    // Retrieve locations list
    NSArray *locations = [jsonResponse objectForKey:kLocationsKey];
    
    if (locations.count == 0) {
        NSLog(@"Empty or improper top level location response. Bailing.");
        _currentlyUpdating = NO;
        return;
    }
    
#ifdef RHITMobile_RHLoaderDebug
    NSLog(@"Deleting all locations");
#endif
    
    // Delete all locations first
    [self deleteAllLocations:localContext];
    
    NSMutableArray *serverIds = [NSMutableArray arrayWithCapacity:locations.count];
    
#ifdef RHITMobile_RHLoaderDebug
    NSLog(@"Saving new top level locations");
#endif
    
    // Create new top level locations from response
    for (NSDictionary *locationDict in locations) {
        RHLocation *location = [self locationFromJSONResponse:locationDict
                                       inManagedObjectContext:localContext];
        location.retrievalStatus = RHLocationRetrievalStatusNoChildren;
        
        [serverIds addObject:location.serverIdentifier.copy];
    }
    
    // Save the new top level locations
    currentError = nil;
    [localContext save:&currentError];
    
    // Delegate callbacks on main thread
    for (NSObject<RHLoaderDelegate> *delegate in self.topLevelDelegates) {
#ifdef RHITMobile_RHLoaderDebug
        NSLog(@"Top level locations delegate found: %@. Notifying of update.", delegate);
#endif
        [delegate performSelectorOnMainThread:@selector(loaderDidUpdateUnderlyingData) withObject:nil waitUntilDone:NO];
    }
    
    if (currentError) {
        NSLog(@"Problem saving top level locations: %@", currentError);
        _currentlyUpdating = NO;
        return;
    }
    
#ifdef RHITMobile_RHLoaderDebug
    NSLog(@"Done updating top level locations");
#endif
    
    //int locationsToPopulate = serverIds.count;
    __block int locationsPopulated = 0;
    
    NSOperationQueue *operationQueue = [[NSOperationQueue alloc] init];
    
#ifdef RHITMobile_RHLoaderDebug
    NSLog(@"Kicking off internal location updates");
#endif
    
    // Queue internal location updates
    for (NSNumber *serverId in serverIds) {
        
        int parentId = serverId.intValue;
        
        [operationQueue addOperationWithBlock:^(void) {
#ifdef RHITMobile_RHLoaderDebug
            NSLog(@"Starting internal location update for location %d", parentId);
#endif
            // Create a thread safe managed object context
            NSManagedObjectContext *blockContext = [[NSManagedObjectContext alloc] init];
            blockContext.persistentStoreCoordinator = [(RHAppDelegate *) [[UIApplication sharedApplication] delegate] persistentStoreCoordinator];
            
            // Load the parent
            NSFetchRequest *parentFetchRequest = [NSFetchRequest fetchRequestWithEntityName:kRHLocationEntityName];
            parentFetchRequest.predicate = [NSPredicate predicateWithFormat:@"serverIdentifier == %d", parentId];
            
            NSError *blockError = nil;
            NSArray *parentResults = [blockContext executeFetchRequest:parentFetchRequest error:&blockError];
            
            if (blockError) {
                NSLog(@"Problem locating parent location for internal request: %@", blockError);
                return;
            }
            
            if (parentResults.count == 0) {
                NSLog(@"Parent location not found: %d", parentId);
                return;
            }
            
            RHLocation *parent = [parentResults objectAtIndex:0];
            
            blockError = nil;
            
            // Request and create new internal locations
            NSDictionary *internalJsonDict = [RHLoaderRequestsWrapper makeSynchronousInternalLocationsRequestForParentLocationID:[NSNumber numberWithInt:parentId] version:version error:&blockError];
            
            if (blockError) {
                NSLog(@"Problem loading internal locations for %d: %@", parentId, blockError);
                return;
            }
            
            NSArray *locations = [internalJsonDict objectForKey:kLocationsKey];
            
            for (NSDictionary *locationDict in locations) {
                RHLocation *location = [self locationFromJSONResponse:locationDict inManagedObjectContext:blockContext];
                location.retrievalStatus = RHLocationRetrievalStatusFull;
            }
            
            parent.retrievalStatus = RHLocationRetrievalStatusFull;
            
            // Save our progress
            blockError = nil;
            [blockContext save:&blockError];
            
            if (blockError) {
                NSLog(@"Error saving internal locations: %@", blockError);
                return;
            }
            
            // Delegate callbacks on main thread
            for (NSObject<RHLocationsLoaderSpecificLocationDelegate> *delegate in [self.individualLocationDelegates objectForKey:parent.serverIdentifier]) {
#ifdef RHITMobile_RHLoaderDebug
                NSLog(@"Internal location delegate found for %@: %@. Notifying of update.", parent.name, delegate);
#endif
                [NSOperationQueue.mainQueue addOperationWithBlock:^{
                    [delegate loaderDidFinishUpdatingLocationWithID:parent.objectID];
                }];
            }
            
            locationsPopulated ++;
            
#ifdef RHITMobile_RHLoaderDebug
            NSLog(@"Finished internal location update for location %d", parentId);
            NSLog(@"Internal location status: %d/%d locations complete", locationsPopulated, serverIds.count);
#endif
        }];
        
    }
    
#ifdef RHITMobile_RHLoaderDebug
    NSLog(@"Waiting for internal location creation to finish");
#endif
    
    [operationQueue waitUntilAllOperationsAreFinished];
    
#ifdef RHITMobile_RHLoaderDebug
    NSLog(@"Saving new internal locations");
#endif
    
    // Save the new top level locations
    currentError = nil;
    [localContext save:&currentError];
    
    if (currentError) {
        NSLog(@"Problem saving top level locations: %@", currentError);
        _currentlyUpdating = NO;
        return;
    }
    
#ifdef RHITMobile_RHLoaderDebug
    NSLog(@"Done updating internal locations");
#endif
    
    // Set the stored version
    [RHDataVersionsLoader.instance setLocationsVersion:newVersion];
    
    _currentlyUpdating = NO;
    
#ifdef RHITMobile_RHLoaderDebug
    NSLog(@"Notifying delegates and performing callbacks");
#endif
    
    // Final callback on main thread
    for (NSObject<RHLoaderDelegate> *delegate in self.allInternalDelegates) {
#ifdef RHITMobile_RHLoaderDebug
        NSLog(@"All internal locations delegate found: %@. Notifying of update.", delegate);
#endif
        [delegate performSelectorOnMainThread:@selector(loaderDidUpdateUnderlyingData) withObject:nil waitUntilDone:NO];
    }
}

- (void)addDelegate:(NSObject<RHLoaderDelegate> *)delegate {
    [self addDelegateForAllInternalLocations:delegate];
}

- (void)removeDelegate:(NSObject<RHLoaderDelegate> *)delegate {
    [self removeDelegateForAllInternalLocations:delegate];
}

- (void)addDelegateForTopLevelLocations:(NSObject<RHLoaderDelegate> *)delegate
{
#ifdef RHITMobile_RHLoaderDebug
    NSLog(@"Adding top level locations delegate: %@", delegate);
#endif
    [self.topLevelDelegates addObject:delegate];
}

- (void)removeDelegateForTopLevelLocations:(NSObject<RHLoaderDelegate> *)delegate
{
#ifdef RHITMobile_RHLoaderDebug
    NSLog(@"Removing top level locations delegate: %@", delegate);
#endif
    [self.topLevelDelegates removeObject:delegate];
#ifdef RHITMobile_RHLoaderDebug
    NSLog(@"After removal: %@", self.topLevelDelegates);
#endif
}

- (void)addDelegateForAllInternalLocations:(NSObject<RHLoaderDelegate> *)delegate
{
#ifdef RHITMobile_RHLoaderDebug
    NSLog(@"Adding all internal locations delegate: %@", delegate);
#endif
    [self.allInternalDelegates addObject:delegate];
}

- (void)removeDelegateForAllInternalLocations:(NSObject<RHLoaderDelegate> *)delegate
{
#ifdef RHITMobile_RHLoaderDebug
    NSLog(@"Removing all internal locations delegate: %@", delegate);
#endif
    [self.allInternalDelegates removeObject:delegate];
#ifdef RHITMobile_RHLoaderDebug
    NSLog(@"After removal: %@", self.allInternalDelegates);
#endif
}

- (void)addDelegate:(NSObject<RHLocationsLoaderSpecificLocationDelegate> *)delegate forLocationWithServerID:(NSNumber *)serverID
{
#ifdef RHITMobile_RHLoaderDebug
    NSLog(@"Adding internal location delegate for location %@: %@", serverID, delegate);
#endif
    NSArray *delegates = [self.individualLocationDelegates objectForKey:serverID];
    
    if (delegates == nil) {
#ifdef RHITMobile_RHLoaderDebug
        NSLog(@"Creating new array for delegate");
#endif
        delegates = [NSArray array];
    }
    
    [self.individualLocationDelegates setObject:[delegates arrayByAddingObject:delegate]
                                         forKey:serverID];
}

- (void)removeDelegate:(NSObject<RHLocationsLoaderSpecificLocationDelegate> *)delegate forLocationWithServerID:(NSNumber *)serverID
{
#ifdef RHITMobile_RHLoaderDebug
    NSLog(@"Removing internal location delegate for location %@: %@", serverID, delegate);
#endif
    NSArray *delegates = [self.individualLocationDelegates objectForKey:serverID];
    
    if (delegates == nil) {
#ifdef RHITMobile_RHLoaderDebug
        NSLog(@"No delegates for that location found anyway");
#endif
        return;
    }
    
    NSMutableArray *mutableDelegates = [NSMutableArray arrayWithArray:delegates];
    [mutableDelegates removeObject:delegate];
    
    [self.individualLocationDelegates setObject:mutableDelegates forKey:serverID];
#ifdef RHITMobile_RHLoaderDebug
    NSLog(@"After removal: %@", mutableDelegates);
#endif
}

- (NSManagedObjectContext *)createThreadSafeManagedObjectContext
{
    NSManagedObjectContext *localContext = [[NSManagedObjectContext alloc] init];
    localContext.persistentStoreCoordinator = [(RHAppDelegate *) [[UIApplication sharedApplication] delegate] persistentStoreCoordinator];
    return localContext;
}

- (RHLocation *)locationFromJSONResponse:(NSDictionary *)jsonResponse
                  inManagedObjectContext:(NSManagedObjectContext *)localContext
{
    NSNumber *serverIdentifier = [jsonResponse objectForKey:kIDKey];
    
    NSFetchRequest *duplicateRequest = [NSFetchRequest fetchRequestWithEntityName:kRHLocationEntityName];
    
    duplicateRequest.predicate = [NSPredicate predicateWithFormat:@"serverIdentifier == %d",serverIdentifier.intValue];
    
    NSArray *possibleDuplicates = [localContext executeFetchRequest:duplicateRequest error:nil];
    
    if (possibleDuplicates.count > 0) {
#ifdef RHITMobile_RHLoaderDebug
        NSLog(@"Prevented duplicate location: %d", serverIdentifier.intValue);
#endif
        return nil;
    }
    
    RHLocation *location = [NSEntityDescription insertNewObjectForEntityForName:kRHLocationEntityName 
                                                         inManagedObjectContext:localContext];
    
    location.serverIdentifier = [jsonResponse objectForKey:kIDKey];
    
    location.name = [jsonResponse objectForKey:kNameKey];
    location.alternateNames = [jsonResponse objectForKey:kAltNamesKey];
    
    NSArray *links = [jsonResponse objectForKey:kLinksKey];
    
    for (NSDictionary *linkDict in links) {
        RHLocationLink *link = [NSEntityDescription insertNewObjectForEntityForName:kRHLocationLinkEntityName inManagedObjectContext:localContext];
        
        link.name = [linkDict objectForKey:kLinkNameKey];
        link.url = [linkDict objectForKey:KLinkURLKey];
        link.owner = location;
    }
    
    location.quickDescription = [jsonResponse objectForKey:kDescriptionKey];
    
    NSString *displayType = [jsonResponse objectForKey:kDisplayTypeKey];
    
    if ([displayType isEqualToString:kDisplayTypeNormal]) {
        location.displayType = RHLocationDisplayTypeNone;
    } else if ([displayType isEqualToString:kDisplayTypePointOfInterest]) {
        location.displayType = RHLocationDisplayTypePointOfInterest;
    } else if ([displayType isEqualToString:kDisplayTypeQuickList]) {
        location.displayType = RHLocationDisplayTypeQuickList;
    }
    
    location.departable = [jsonResponse objectForKey:kDepartableKey];
    
    NSDictionary *mapArea = [jsonResponse objectForKey:kMapAreaKey];
    
    if (mapArea != (id)[NSNull null]) {
        location.visibleZoomLevel = [mapArea objectForKey:kMinZoomLevelKey];
        
        NSArray *retrievedBoundary = [mapArea objectForKey:kCornersKey];
        
        NSMutableArray *workingBoundary = [[NSMutableArray alloc]
                                           initWithCapacity:retrievedBoundary.count];
        
        for (NSDictionary *nodeDict in retrievedBoundary) {
            RHBoundaryNode *node = [NSEntityDescription insertNewObjectForEntityForName:kRHBoundaryNodeEntityName inManagedObjectContext:localContext];
            
            node.latitude = [nodeDict objectForKey:kNodeLatKey];
            node.longitude  = [nodeDict objectForKey:kNodeLongKey];
            
            [workingBoundary addObject:node];
        }
        
        location.orderedBoundaryNodes = workingBoundary;
    } else {
        location.visibleZoomLevel = [NSNumber numberWithInt:-1];
    }
    
    NSDictionary *centerDict = [jsonResponse objectForKey:kCenterKey];
    
    RHLabelNode *centerNode = [NSEntityDescription insertNewObjectForEntityForName:kRHLabelNodeEntityname inManagedObjectContext:localContext];
    
    centerNode.latitude = [centerDict objectForKey:kNodeLatKey];
    centerNode.longitude = [centerDict objectForKey:kNodeLongKey];
    
    location.labelLocation = centerNode;
    
    location.retrievalStatus = RHLocationRetrievalStatusNoChildren;
    
    id parentId = [jsonResponse objectForKey:kParentKey];
    
    if (parentId != [NSNull null]) {
        NSNumber *parentServerIdentifier = (NSNumber *) parentId;
        
        NSFetchRequest *parentFetchRequest = [NSFetchRequest fetchRequestWithEntityName:kRHLocationEntityName];
        
        NSPredicate *predicate = [NSPredicate predicateWithFormat:@"serverIdentifier == %d", parentServerIdentifier.intValue];
        
        parentFetchRequest.predicate = predicate;
        
        NSError *parentError;
        
        NSArray *potentialParents = [localContext executeFetchRequest:parentFetchRequest
                                                                error:&parentError];
        
        if (parentError) {
            NSLog(@"Problem looking for location parent: %@", parentError);
            return location;
        }
        
        if (potentialParents.count > 0) {
            location.parent = [potentialParents objectAtIndex:0];
        }
    }
    
    return location;
}

- (void)deleteAllLocations:(NSManagedObjectContext *)managedObjectContext
{
    NSFetchRequest *fetchRequst = [NSFetchRequest fetchRequestWithEntityName:kRHLocationEntityName];
    
    NSError *fetchError;
    NSArray *oldLocations = [managedObjectContext executeFetchRequest:fetchRequst error:&fetchError];
    
    if (fetchError) {
        NSLog(@"Problem finding old locations: %@", fetchError);
    }
    
    for (RHLocation *location in oldLocations) {
        [managedObjectContext deleteObject:location];
    }
}

@end