//
//  RHDataVersionManager.m
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

#import "RHDataVersionManager.h"
#import "RHDataVersionRequester.h"
#import "RHPListStore.h"

#define kServerLocationsVersionKey @"LocationsVersion"
#define kServerServicesVersionKey @"ServicesVersion"
#define kServerTagsVersionKey @"TagsVersion"


@interface RHDataVersionManager ()

@property (atomic, strong) RHPListStore *pListStore;

@property (atomic, strong) NSDictionary *serverVersions;

@end


@implementation RHDataVersionManager

@synthesize pListStore = pListStore_;
@synthesize serverVersions = serverVersions_;

static RHDataVersionManager *instance_;

+ (RHDataVersionManager *)instance {
    return instance_;
}

+ (void)initialize {
    static BOOL initialized = NO;
    if(!initialized) {
        initialized = YES;
        instance_ = [[RHDataVersionManager alloc] init];
    }
}

- (id)init {
    self = [super init];
    
    if (self) {
        self.pListStore = [[RHPListStore alloc] init];
    }
    
    return self;
}

- (NSNumber *)serverLocationsVersion {
    if (self.serverVersions == nil) {
        self.serverVersions = [[[RHDataVersionRequester alloc] init] currentDataVersions];
    }
    
    return [self.serverVersions objectForKey:kServerLocationsVersionKey];
}

- (NSNumber *)serverServicesVersion {
    if (self.serverVersions == nil) {
        self.serverVersions = [[[RHDataVersionRequester alloc] init] currentDataVersions];
    }
    
    return [self.serverVersions objectForKey:kServerServicesVersionKey];
}

- (NSNumber *)serverTagsVersion {
    if (self.serverVersions == nil) {
        self.serverVersions = [[[RHDataVersionRequester alloc] init] currentDataVersions];
    }
    
    return [self.serverVersions objectForKey:kServerTagsVersionKey];
}

- (NSNumber *)localLocationsVersion {
    return self.pListStore.currentMapDataVersion;
}

- (void)setLocalLocationsVersion:(NSNumber *)localLocationsVersion {
    self.pListStore.currentMapDataVersion = localLocationsVersion;
}

- (NSNumber *)localServicesVersion {
    return self.pListStore.currentServicesDataVersion;
}

- (void)setLocalServicesVersion:(NSNumber *)localServicesVersion {
    self.pListStore.currentServicesDataVersion = localServicesVersion;
}

- (NSNumber *)localTagsVersion {
    return self.pListStore.currentTagsDataVersion;
}

- (void)setLocalTagsVersion:(NSNumber *)localTagsVersion {
    self.pListStore.currentTagsDataVersion = localTagsVersion;
}

- (BOOL)needsLocationsUpdate {
    return self.serverLocationsVersion.doubleValue > self.localLocationsVersion.doubleValue;
}

- (BOOL)needsServicesUpdate {
    return self.serverServicesVersion.doubleValue > self.localServicesVersion.doubleValue;
}

- (BOOL)needsTagsUpdate {
    return self.serverTagsVersion.doubleValue > self.localTagsVersion.doubleValue;
}

- (void)upgradeLocationsVersion {
    self.localLocationsVersion = self.serverLocationsVersion;
}

- (void)upgradeServicesVersion {
    self.localServicesVersion = self.serverServicesVersion;
}

- (void)upgradeTagsVersion {
    self.localTagsVersion = self.serverTagsVersion;
}

@end
