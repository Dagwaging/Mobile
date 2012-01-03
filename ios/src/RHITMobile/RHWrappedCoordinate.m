//
//  RHWrappedCoordinate.m
//  RHITMobile
//
//  Created by Jimmy Theis on 12/12/11.
//  Copyright (c) 2011 Rose-Hulman Institute of Technology. All rights reserved.
//

#import "RHWrappedCoordinate.h"

@implementation RHWrappedCoordinate

@synthesize coordinate;

+ (id)coordinateWithcoordinate2D:(CLLocationCoordinate2D)inCoordinate {
    RHWrappedCoordinate *result = [[[RHWrappedCoordinate alloc] init] autorelease];
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
