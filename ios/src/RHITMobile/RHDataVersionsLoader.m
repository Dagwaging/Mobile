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
    _currentlyUpdating = YES;
    
    [RHLoaderRequestsWrapper makeDataVersionsRequestWithSuccessBlock:^(NSDictionary *jsonDict) {
        
        NSNumber *oldLocationsVersion = [_versionsDict objectForKey:kLocationsVersionKey];
        NSNumber *oldCampusServicesVersion = [_versionsDict objectForKey:kCampusServicesVesionkey];
        NSNumber *oldTourTagsVersion = [_versionsDict objectForKey:kTourTagsVersionKey];
        
        NSNumber *newLocationsVersion = [jsonDict objectForKey:kLocationsVersionKey];
        NSNumber *newCampusServicesVersion = [jsonDict objectForKey:kCampusServicesVesionkey];
        NSNumber *newTourTagsVersion = [jsonDict objectForKey:kTourTagsVersionKey];
        
        BOOL upToDate = YES;
        
        if (oldLocationsVersion.doubleValue < newLocationsVersion.doubleValue) {
            NSLog(@"Locations update required");
            upToDate = NO;
            
            [RHLocationsLoader.instance updateLocations:oldLocationsVersion.doubleValue];
        }
        
        if (oldCampusServicesVersion.doubleValue < newCampusServicesVersion.doubleValue) {
            NSLog(@"Campus services update required");
            upToDate = NO;
            
            // TODO
        }
        
        if (oldTourTagsVersion.doubleValue < newTourTagsVersion.doubleValue) {
            NSLog(@"Tour tags update required");
            upToDate = NO;
            
            // TODO
        }
        
        if (upToDate) {
            NSLog(@"All cached data up to date");
        }
        
        _currentlyUpdating = NO;
        
    } failureBlock:^(NSError *error) {
        
        NSLog(@"Error checking for new versions: %@", error);
        
        _currentlyUpdating = NO;
        
    }];
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
    [_versionsDict writeToFile:_plistPath atomically:YES];
}

- (void)loadData
{
    _versionsDict = [NSDictionary dictionaryWithContentsOfFile:_plistPath];
}

@end
