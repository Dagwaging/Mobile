//
//  RHRestHandler.m
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

#import "RHRestHandler.h"
#import "RHRemoteHandlerDelegate.h"
#import "RHLabelNode.h"
#import "RHLocation.h"


@interface RHRestHandler ()

@property (nonatomic, retain) NSManagedObjectContext *context;
@property (nonatomic, retain) NSOperationQueue *operations;

- (void)performFetchAllLocations;

@end


@implementation RHRestHandler

@synthesize delegate;
@synthesize context;
@synthesize operations;

- (id)initWithContext:(NSManagedObjectContext *)inContext
             delegate:(RHRemoteHandlerDelegate *)inDelegate {
    self = [super init];
    
    if (self) {
        self.delegate = inDelegate;
        self.context = inContext;
        self.operations = [[NSOperationQueue new] autorelease];
    }
    
    return self;
}

- (void)fetchAllLocations {
    if (self.delegate == nil) {
        return;
    }
    
    NSInvocationOperation* operation = [NSInvocationOperation alloc];
    operation = [[operation initWithTarget:self
                                  selector:@selector(performFetchAllLocations)
                                    object:nil] autorelease];
    [operations addOperation:operation];
}

#pragma mark -
#pragma mark Private Methods

- (void)performFetchAllLocations {
    
    // Aggregate and pass on locations
}

@end
