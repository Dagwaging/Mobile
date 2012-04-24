//
//  RHDataVersionsLoader.m
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

#import "RHDataVersionsLoader.h"
#import "RHConstants.h"
#import "RHLoaderRequestsWrapper.h"
#import "RHLocationsLoader.h"


#define kFileName @"CachedDataVersions.plist"
#define kLocationsVersionKey @"LocationsVersion"
#define kCampusServicesVesionkey @"ServicesVersion"
#define kTourTagsVersionKey @"TagsVersion"


@interface RHDataVersionsLoader() {
@private
    NSString *_plistPath;
    NSDictionary *_versionsDict;
    BOOL _currentlyUpdating;
}

- (void)saveData;

- (void)loadData;

@end


@implementation RHDataVersionsLoader

static RHDataVersionsLoader * _instance;

+ (void)initialize
{
    static BOOL initialized = NO;
    if(!initialized)
    {
        initialized = YES;
        _instance = [[RHDataVersionsLoader alloc] init];
    }
}

+ (id)instance
{
    return _instance;
}

- (id)init
{
    self = [super init];
    
    if (self) {
        _currentlyUpdating = NO;
        
        NSArray *paths = NSSearchPathForDirectoriesInDomains(NSDocumentDirectory, NSUserDomainMask, YES);
        NSString *documentsDirectory = [paths objectAtIndex:0];
        
        _plistPath = [documentsDirectory stringByAppendingPathComponent:kFileName];
        
        [self loadData];
    }
    
    return self;
}

- (BOOL)currentlyUpdating
{
    return _currentlyUpdating;
}

- (void)checkForNewVersions
{
    // Only operate on background thread
    if ([NSThread isMainThread]) {
        [self performSelectorInBackground:@selector(checkForNewVersions) withObject:nil];
        return;
    }
    
    NSError *currentError = nil;
    
    NSDictionary *jsonResponse = [RHLoaderRequestsWrapper makeSynchronousDataVersionsRequestWithError:&currentError];
    
    if (currentError) {
        NSLog(@"Problem loading current data versions: %@", currentError);
    }
    
    _currentlyUpdating = YES;    
    
    // Old (cached) versions
    NSNumber *oldLocationsVersion = [_versionsDict objectForKey:kLocationsVersionKey];
    NSNumber *oldCampusServicesVersion = [_versionsDict objectForKey:kCampusServicesVesionkey];
    NSNumber *oldTourTagsVersion = [_versionsDict objectForKey:kTourTagsVersionKey];
    
    // New (server) versions
    NSNumber *newLocationsVersion = [jsonResponse objectForKey:kLocationsVersionKey];
    NSNumber *newCampusServicesVersion = [jsonResponse objectForKey:kCampusServicesVesionkey];
    NSNumber *newTourTagsVersion = [jsonResponse objectForKey:kTourTagsVersionKey];
    
    BOOL upToDate = YES;
    
    // Check for location update
    if (oldLocationsVersion.doubleValue < newLocationsVersion.doubleValue) {
        NSLog(@"Locations update required (%f => %f)", oldLocationsVersion.doubleValue, newLocationsVersion.doubleValue);
        upToDate = NO;
        
        [RHLocationsLoader.instance updateLocations:oldLocationsVersion];
    }
    
    // Check for campus services update
    if (oldCampusServicesVersion.doubleValue < newCampusServicesVersion.doubleValue) {
        NSLog(@"Campus services update required (%f => %f)", oldCampusServicesVersion.doubleValue, newCampusServicesVersion.doubleValue);
        upToDate = NO;
        
        // TODO
    }
    
    // Check for tour tags update
    if (oldTourTagsVersion.doubleValue < newTourTagsVersion.doubleValue) {
        NSLog(@"Tour tags update required (%f => %f)", oldTourTagsVersion.doubleValue, newTourTagsVersion.doubleValue);
        upToDate = NO;
        
        // TODO
    }
    
    if (upToDate) {
        NSLog(@"All cached data up to date");
    }
    
    _currentlyUpdating = NO;
}

- (void)setLocationsVersion:(NSNumber *)locationsVersion
{
    [_versionsDict setValue:locationsVersion forKey:kLocationsVersionKey];
    [self saveData];
    [self loadData];
    
    NSLog(@"Locations updated to version %@", locationsVersion);
}

- (void)setCampusServicesVersion:(NSNumber *)campusServicesVersion
{
    [_versionsDict setValue:campusServicesVersion forKey:kCampusServicesVesionkey];
    [self saveData];
    [self loadData];
    
    NSLog(@"Campus services updated to version %@", campusServicesVersion);
}

- (void)setTourTagsVersion:(NSNumber *)tourTagsVersion
{
    [_versionsDict setValue:tourTagsVersion forKey:kTourTagsVersionKey];
    [self saveData];
    [self loadData];
    
    NSLog(@"Tour tags updated to version %@", tourTagsVersion);
}

- (void)saveData
{
    @synchronized (self) {
        [_versionsDict writeToFile:_plistPath atomically:YES];
    }
}

- (void)loadData
{
    @synchronized (self) {
        _versionsDict = [NSDictionary dictionaryWithContentsOfFile:_plistPath];
        
        if (_versionsDict == nil) {
#ifdef RHITMobile_RHLoaderDebug
            NSLog(@"No version data stored yet. Creating");
#endif
            _versionsDict = [[NSMutableDictionary alloc] init];
        }
    }
}

@end
