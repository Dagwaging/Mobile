//
//  RHServiceCategory.h
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

#import <Foundation/Foundation.h>
#import <CoreData/CoreData.h>
#import "RHServiceItem.h"

#define kRHServiceCategoryEntityName @"ServiceCategory"


@class RHServiceItem;

@interface RHServiceCategory : RHServiceItem

@property (nonatomic, strong) NSSet *contents;

@end

@interface RHServiceCategory (CoreDataGeneratedAccessors)

- (void)addContentsObject:(RHServiceItem *)value;
- (void)removeContentsObject:(RHServiceItem *)value;
- (void)addContents:(NSSet *)values;
- (void)removeContents:(NSSet *)values;
@end
