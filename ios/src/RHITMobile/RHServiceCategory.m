//
//  RHServiceCategory.m
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

#import "RHServiceCategory.h"
#import "RHServiceItem.h"
#import "RHServiceLink.h"


@implementation RHServiceCategory

@dynamic contents;

- (NSArray *)sortedContents
{
    return [[self.contents allObjects]
            sortedArrayUsingComparator:^NSComparisonResult(id item1, id item2) {
                if ([item1 isKindOfClass:[RHServiceCategory class]]) {
                    if ([item2 isKindOfClass:[RHServiceLink class]]) {
                        return NSOrderedAscending;
                    } else {
                        return [[item1 name] compare:[item2 name]];
                    }
                } else {
                    if ([item1 isKindOfClass:[RHServiceCategory class]]) {
                        return NSOrderedDescending;
                    } else {
                        return [[item1 name] compare:[item2 name]];
                    }
                }
    }];
}

@end
