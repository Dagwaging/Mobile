//
//  RHWrappedCoordinate.m
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

#import "RHWrappedCoordinate.h"

@implementation RHWrappedCoordinate

@synthesize coordinate;

+ (id)coordinateWithcoordinate2D:(CLLocationCoordinate2D)inCoordinate {
    RHWrappedCoordinate *result = [[RHWrappedCoordinate alloc] init];
    result.coordinate = inCoordinate;
    return result;
}

+ (id)coordinateWithLatitude:(CLLocationDegrees)latitude longitude:(CLLocationDegrees)longitude {
    return [RHWrappedCoordinate coordinateWithcoordinate2D:CLLocationCoordinate2DMake(latitude, longitude)];
}

- (CLLocationDegrees)latitude {
    return self.coordinate.latitude;
}

- (CLLocationDegrees)longitude {
    return self.coordinate.longitude;
}

@end
