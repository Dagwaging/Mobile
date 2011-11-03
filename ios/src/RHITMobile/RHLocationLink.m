//
//  RHLocationLink.m
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

#import "RHLocationLink.h"
#import "RHLocation.h"


@implementation RHLocationLink

@dynamic name;
@dynamic url;
@dynamic owner;

+ (RHLocationLink *)linkFromContext:(NSManagedObjectContext *)context {
    RHLocationLink *link = [NSEntityDescription insertNewObjectForEntityForName:kRHLocationLinkCoreDataModelIdentifier inManagedObjectContext:context];
    return link;
}

@end