//
//  RHTourRequester.m
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

#import "RHTourRequester.h"
#import "RHLocation.h"
#import "RHPathStep.h"
#import "RHTourTag.h"
#import "RHWrappedCoordinate.h"
#import "RHWebRequestMaker.h"

#define kTourPath @"/tours"
#define kOnCampusPath @"/oncampus"
#define kOffCampusPath @"/offcampus"
#define kFromLocationPath @"/fromloc/%d"
#define kTagPath @"/%d"
#define kDurationArgs @"&length=%d"


@interface RHTourRequester ()

@property (atomic, assign) RHTourRequestType requestType;
@property (atomic, strong) NSArray *tagIDs;
@property (atomic, strong) NSNumber *duration;
@property (atomic, strong) NSManagedObjectID *locationID;

- (void)requestTourWithTagIDs;

@end


@implementation RHTourRequester

@synthesize requestType = requestType_;
@synthesize tagIDs = tagIDs_;
@synthesize duration = duration_;
@synthesize locationID = locationID_;

- (void)requestTourWithTags:(NSArray *)tags
              startLocation:(RHLocation *)location
                       type:(RHTourRequestType)requestType
                   duration:(NSNumber *)duration {
    NSMutableArray *ids = [NSMutableArray arrayWithCapacity:tags.count];
    
    for (RHTourTag *tag in tags) {
        [ids addObject:tag.objectID];
    }
    
    self.tagIDs = ids;
    self.requestType = requestType;
    self.duration = duration;
    
    [self performSelectorInBackground:@selector(requestTourWithTagIDs) withObject:nil];
}

- (void)requestTourWithTagIDs {
    NSManagedObjectContext *localContext = [[NSManagedObjectContext alloc] init];
    localContext.persistentStoreCoordinator = self.persistantStoreCoordinator;
    
    NSString *path = kTourPath;
    
    if (self.requestType == RHTourRequestTypeOnCampus) {
        path = [path stringByAppendingString:kOnCampusPath];
        
        //RHLocation *startLocation = (RHLocation *) [localContext objectWithID:self.locationID];
        //path = [path stringByAppendingFormat:kFromLocationPath, startLocation.serverIdentifier.intValue];
        path = [path stringByAppendingFormat:kFromLocationPath, 112];
    } else {
        path = [path stringByAppendingString:kOffCampusPath];
    }
    
    for (NSManagedObjectID *tagID in self.tagIDs) {
        NSLog(@"%@", tagID);
        RHTourTag *tag = (RHTourTag *)[localContext objectWithID:tagID];
        path = [path stringByAppendingFormat:kTagPath, tag.serverIdentifier.intValue];
    }
    
    //NSString *urlArgs = [NSString stringWithFormat:kDurationArgs, self.duration.intValue];
    
    [self sendDelegatePathFromJSONResponse:[RHWebRequestMaker JSONGetRequestWithPath:path URLargs:@"?wait=true"]];
}

@end

