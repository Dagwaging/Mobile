//
//  RHWrappedCoordinate.h
//  RHITMobile
//
//  Created by Jimmy Theis on 12/12/11.
//  Copyright (c) 2011 Rose-Hulman Institute of Technology. All rights reserved.
//

#import <Foundation/Foundation.h>
#import <CoreLocation/CoreLocation.h>

@interface RHWrappedCoordinate : NSObject

@property (nonatomic, assign) CLLocationCoordinate2D coordinate;

@property (nonatomic, readonly) CLLocationDegrees latitude;

@property (nonatomic, readonly) CLLocationDegrees longitude;

+ (id)coordinateWithcoordinate2D:(CLLocationCoordinate2D)coordinate;

+ (id)coordinateWithLatitude:(CLLocationDegrees)latitude
                   longitude:(CLLocationDegrees)longitude;

@end
