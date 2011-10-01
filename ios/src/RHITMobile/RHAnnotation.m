//
//  RHAnnotation.m
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

#import "RHAnnotation.h"
#import "RHLabelNode.h"
#import "RHLocation.h"


@implementation RHAnnotation

@synthesize coordinate;
@synthesize annotationType;
@synthesize location;

- (RHAnnotation *)initWithLocation:(RHLocation *)inLocation
                    annotationType:(RHAnnotationType)inAnnotationType {
    self = [super init];
    
    if (self) {
        [self setLocation:inLocation];
        [self setAnnotationType:inAnnotationType];
    }
    
    return self;
    
}

- (CLLocationCoordinate2D)coordinate {
    return [[[self location] labelLocation] coordinate];
}

- (NSString *)title {
    return [[self location] name];
}

- (NSString *)subtitle {
    return self.location.quickDescription;
}

@end