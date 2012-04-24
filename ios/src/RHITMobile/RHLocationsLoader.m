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
#import "RHLabelNode.h"
#import "RHLoaderRequestsWrapper.h"
#import "RHLocation.h"
#import "RHLocationLink.h"

#define kLocationsKey @"Locations"
#define kIDKey @"Id"
#define kNameKey @"Name"
#define kAltNamesKey @"AltNames"
#define kLinksKey @"Links"
#define kLinkNameKey @"Name"
#define KLinkURLKey @"Url"
#define kDescriptionKey @"Description"
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

@interface RHLocationsLoader () {
@private
    BOOL _currentlyUpdating;
}

- (RHLocation *)locationFromJSONResponse:(NSDictionary *)jsonResponse
                  inManagedObjectContext:(NSManagedObjectContext *)localContext;

- (void)deleteAllLocations:(NSManagedObjectContext *)managedObjectContext;

- (NSManagedObjectContext *)createThreadSafeManagedObjectContext;

@end


@implementation RHLocationsLoader

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

- (id)init
{
    if (self = [super init]) {
        _currentlyUpdating = NO;
    }
    
    return self;
}

+ (id)instance
{
    return _instance;
}

- (BOOL)currentlyUpdating
{
    return _currentlyUpdating;
}

- (void)updateLocations:(NSNumber *)version
{
    // Only operate on background thread
    if ([NSThread isMainThread]) {
        [self performSelectorInBackground:@selector(updateLocations:) withObject:nil];
        return;
    }
    
    NSManagedObjectContext *localContext = [self createThreadSafeManagedObjectContext];
    NSError *currentError = nil;
    
    NSDictionary *jsonResponse = [RHLoaderRequestsWrapper makeSynchronousTopLocationsRequestWithWithVersion:version error:&currentError];
    
    if (currentError) {
        NSLog(@"Problem updating top level locations: %@", currentError);
        return;
    }
    
    // Retrieve locations list
    NSArray *locations = [jsonResponse objectForKey:kLocationsKey];
    
    if (locations.count == 0) {
        NSLog(@"Empty or improper top level location response. Bailing.");
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
    
    if (currentError) {
        NSLog(@"Problem saving top level locations: %@", currentError);
        return;
    }
    
#ifdef RHITMobile_RHLoaderDebug
    NSLog(@"Done updating top level locations");
#endif
    
    int locationsToPopulate = serverIds.count;
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
            
            // TODO: Notify if applicable
            
            locationsPopulated ++;
#ifdef RHITMobile_RHLoaderDebug
            NSLog(@"Finished internal location update for location %d", parentId);
            NSLog(@"Internal location status: %d/%d locations complete", locationsPopulated, locationsToPopulate);
#endif
        }];
        
    }
    
#ifdef RHITMobile_RHLoaderDebug
    NSLog(@"Waiting for internal location creation to finish");
#endif
    
    while (locationsPopulated < locationsToPopulate) {
        // Spin wait
    }
    
#ifdef RHITMobile_RHLoaderDebug
    NSLog(@"Saving new internal locations");
#endif
    
    // Save the new top level locations
    currentError = nil;
    [localContext save:&currentError];
    
    if (currentError) {
        NSLog(@"Problem saving top level locations: %@", currentError);
        return;
    }
    
#ifdef RHITMobile_RHLoaderDebug
    NSLog(@"Done updating internal locations");
#endif
    
    // TODO: Notify if applicable
}

- (void)registerCallbackForTopLevelLocations:(void (^)(void))callback
{
    // TODO
}

- (void)registerCallbackForLocationWithId:(NSInteger)locationId
                                 callback:(void (^)(void))callback
{
    // TODO
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
        return [possibleDuplicates objectAtIndex:0];
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
