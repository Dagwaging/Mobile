//
//  RHPListStore.m
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

#import "RHPListStore.h"


#define kRHPListStoreFile @"RHITMobileValues"
#define kCurrentMapDataVersionKey @"CurrentMapDataVersion"
#define kCurrentServiceDataVersionKey @"CurrentServiceDataVersion"


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

- (NSString *)currentMapDataVersion {
    return [[self.data valueForKey:kCurrentMapDataVersionKey] description];
}

- (void)setCurrentMapDataVersion:(NSString *)inCurrentDataVersion {
    NSMutableDictionary *data = self.data;
    [data setValue:inCurrentDataVersion forKey:kCurrentMapDataVersionKey];
    self.data = data;
}

- (NSString *)currentServicesDataVersion {
    return [[self.data valueForKey:kCurrentServiceDataVersionKey] description];
}

- (void)setCurrentServicesDataVersion:(NSString *)currentServicesDataVersion {
    NSMutableDictionary *data = self.data;
    [data setValue:currentServicesDataVersion forKey:kCurrentServiceDataVersionKey];
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
