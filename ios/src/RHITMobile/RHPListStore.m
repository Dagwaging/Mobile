//
//  RHPListStore.m
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

#import "RHPListStore.h"


#define kRHPListStoreFile @"RHITMobileValues"
#define kCurrentMapDataVersionKey @"CurrentMapDataVersion"
#define kCurrentServiceDataVersionKey @"CurrentServiceDataVersion"
#define kCurrentTagsDataVersionKey @"CurrentTagsDataVersion"


@interface RHPListStore ()

@property (nonatomic, strong) NSString *path;
@property (nonatomic, strong) NSMutableDictionary *data;

@end


@implementation RHPListStore

@synthesize path;

- (id)init {
    self = [super init];
    
    NSError *error;
    NSArray *paths = NSSearchPathForDirectoriesInDomains(NSDocumentDirectory,
                                                         NSUserDomainMask,
                                                         YES);
    NSString *documentsDirectory = [paths objectAtIndex:0];
    self.path = [documentsDirectory
                 stringByAppendingPathComponent:@"RHITMobileValues.plist"];
        
    NSFileManager *fileManager = [NSFileManager defaultManager];
        
    if (![fileManager fileExistsAtPath:self.path]) {
        NSString *bundle = [[NSBundle mainBundle]
                            pathForResource:kRHPListStoreFile
                            ofType:@"plist"];
            
        [fileManager copyItemAtPath:bundle toPath:self.path error:&error];
    }

    return self;
}

#pragma mark - Property Methods

- (NSNumber *)currentMapDataVersion {
    return [self.data valueForKey:kCurrentMapDataVersionKey];
}

- (void)setCurrentMapDataVersion:(NSNumber *)inCurrentDataVersion {
    NSMutableDictionary *data = self.data;
    [data setValue:inCurrentDataVersion forKey:kCurrentMapDataVersionKey];
    self.data = data;
}

- (NSNumber *)currentServicesDataVersion {
    return [self.data valueForKey:kCurrentServiceDataVersionKey];
}

- (void)setCurrentServicesDataVersion:(NSNumber *)currentServicesDataVersion {
    NSMutableDictionary *data = self.data;
    [data setValue:currentServicesDataVersion forKey:kCurrentServiceDataVersionKey];
    self.data = data;
}

- (NSNumber *)currentTagsDataVersion {
    return [self.data valueForKey:kCurrentTagsDataVersionKey];
}

- (void)setCurrentTagsDataVersion:(NSNumber *)currentTagsDataVersion {
    NSMutableDictionary *data = self.data;
    [data setValue:currentTagsDataVersion forKey:kCurrentTagsDataVersionKey];
    self.data = data;
}

#pragma mark - Private Property Methods

- (NSMutableDictionary *)data {
    return [NSMutableDictionary
            dictionaryWithDictionary:[NSDictionary dictionaryWithContentsOfFile:self.path]];
}

- (void)setData:(NSDictionary *)inData {
    [inData writeToFile:path atomically:YES];
}

@end
