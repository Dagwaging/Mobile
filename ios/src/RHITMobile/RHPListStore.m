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
#define kRHCurrentDataVersionKey @"CurrentDataVersion"


@interface RHPListStore ()

@property (nonatomic, readonly) NSDictionary *data;

- (void)saveData;

@end


@implementation RHPListStore

- (NSString *)currentDataVersion {
    return [self.data valueForKey:kRHCurrentDataVersionKey];
}

- (void)setCurrentDataVersion:(NSString *)inCurrentDataVersion {
    [self.data setValue:inCurrentDataVersion forKey:kRHCurrentDataVersionKey];
    [self saveData];
}

#pragma mark -
#pragma mark Private Methods

- (NSDictionary *)data {
    NSData *plistData;  
    NSString *error;  
    NSPropertyListFormat format;  
    id plist;  
    
    NSString *localizedPath = [[NSBundle mainBundle]
                               pathForResource:kRHPListStoreFile
                               ofType:@"plist"];  
    plistData = [NSData dataWithContentsOfFile:localizedPath];   
    
    plist = [NSPropertyListSerialization
             propertyListFromData:plistData
             mutabilityOption:NSPropertyListImmutable
             format:&format
             errorDescription:&error]; 
    
    return (NSDictionary *)plist;  
}

- (void)saveData {
    NSData *xmlData;  
    NSString *error;   
    
    NSString *localizedPath = [[NSBundle mainBundle]
                               pathForResource:kRHPListStoreFile
                               ofType:@"plist"];  
    
    xmlData = [NSPropertyListSerialization
               dataFromPropertyList:self.data
               format:NSPropertyListXMLFormat_v1_0
               errorDescription:&error];
    
    if (xmlData) {  
        [xmlData writeToFile:localizedPath atomically:YES];  
    }
}

@end
