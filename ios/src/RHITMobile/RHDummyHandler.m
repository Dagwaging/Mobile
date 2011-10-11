//
//  RHDummyHandler.m
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

#import "RHDummyHandler.h"
#import "RHRemoteHandlerDelegate.h"
#import "RHLabelNode.h"
#import "RHLocation.h"


@interface RHDummyHandler ()

@property (nonatomic, retain) NSManagedObjectContext *context;
@property (nonatomic, retain) NSOperationQueue *operations;

- (void)performFetchAllLocations;

- (void)performCheckForNewLocations;

@end


@implementation RHDummyHandler

@synthesize delegate;
@synthesize context;
@synthesize operations;

- (RHDummyHandler *)initWithContext:(NSManagedObjectContext *)inContext
                           delegate:(RHRemoteHandlerDelegate *)inDelegate {
    self = [super init];
    
    if (self) {
        self.delegate = inDelegate;
        self.context = inContext;
        self.operations = [[NSOperationQueue new] autorelease];
        
    }
    
    return self;
}

- (void)checkForLocationUpdates {
    if (self.delegate == nil) {
        return;
    }
    
    NSInvocationOperation* operation = [NSInvocationOperation alloc];
    operation = [[operation initWithTarget:self
                                 selector:@selector(performFetchAllLocations)
                                   object:nil] autorelease];
    [operations addOperation:operation];
}

- (void)checkForNewLocations {
    NSInvocationOperation* operation = [NSInvocationOperation alloc];
    operation = [[operation initWithTarget:self
                                  selector:@selector(performCheckForNewLocations)
                                    object:nil] autorelease];
    [operations addOperation:operation];
}

#pragma mark -
#pragma mark Private Methods

- (void)performFetchAllLocations {
    // Hatfield Hall
    RHLabelNode *hatfieldLabel = (RHLabelNode *)[RHLabelNode
                                                 fromContext:self.context];
    hatfieldLabel.latitude = [NSNumber numberWithDouble:39.482117];
    hatfieldLabel.longitude = [NSNumber numberWithDouble:-87.322254];
    RHLocation *hatfield = (RHLocation *)[RHLocation
                                          fromContext:self.context];
    hatfield.name = @"Hatfield Hall";
    hatfield.labelLocation = hatfieldLabel;
    
    // Moench
    RHLabelNode *moenchLabel = (RHLabelNode *)[RHLabelNode
                                               fromContext:self.context];
    moenchLabel.latitude = [NSNumber numberWithDouble:39.483367];
    moenchLabel.longitude = [NSNumber numberWithDouble:-87.323745];
    RHLocation *moench = (RHLocation *)[RHLocation
                                        fromContext:self.context];
    moench.name = @"Moench Hall";
    moench.labelLocation = moenchLabel;
    moench.quickDescription = @"Ahhh  super long description. Ahhhh!!!!!!!";
    
    // Crapo
    RHLabelNode *crapoLabel = (RHLabelNode *)[RHLabelNode
                                              fromContext:self.context];
    crapoLabel.latitude = [NSNumber numberWithDouble:39.483690];
    crapoLabel.longitude = [NSNumber numberWithDouble:-87.324443];
    RHLocation *crapo = (RHLocation *)[RHLocation
                                       fromContext:self.context];
    crapo.name = @"Crapo Hall";
    crapo.labelLocation = crapoLabel;
    crapo.quickDescription = @"This is a hall. For school.";
    
    // Olin
    RHLabelNode *olinLabel = (RHLabelNode *)[RHLabelNode
                                             fromContext:self.context];
    olinLabel.latitude = [NSNumber numberWithDouble:39.482763];
    olinLabel.longitude = [NSNumber numberWithDouble:-87.324647];
    RHLocation *olin = (RHLocation *)[RHLocation
                                      fromContext:self.context];
    olin.name = @"Olin Hall";
    olin.labelLocation = olinLabel;
    olin.quickDescription = @"This is a hall. For school.";
    
    // Aggregate and pass on locations
    NSSet *locations = [[NSArray alloc] initWithObjects:hatfield, moench, crapo, olin, nil];
    //[delegate didFetchAllLocations:locations];
    [locations release];
}

-(void)performCheckForNewLocations {
    // TODO
}

@end
