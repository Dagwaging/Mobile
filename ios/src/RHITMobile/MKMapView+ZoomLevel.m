//
//  MKMapView+ZoomLevel.m
//  RHIT Mobile Campus Directory
//
//  Copyright 2011 Rose-Hulman Institute of Technology
//
//  Based on code provided publicly by Troy Brant without licensing terms:
//  http://troybrant.net/blog/2010/01/set-the-zoom-level-of-an-mkmapview/
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

#import "MKMapView+ZoomLevel.h"


#define MERCATOR_OFFSET 268435456
#define MERCATOR_RADIUS 85445659.44705395

@implementation MKMapView (ZoomLevel)

#pragma mark - Map Conversion Methods

- (double)longitudeToPixelSpaceX:(double)longitude {
    return round(MERCATOR_OFFSET + MERCATOR_RADIUS * longitude * M_PI / 180.0);
}

- (double)latitudeToPixelSpaceY:(double)latitude {
    return round(MERCATOR_OFFSET - MERCATOR_RADIUS *
                 logf((1 + sinf(latitude * M_PI / 180.0)) / 
                      (1 - sinf(latitude * M_PI / 180.0))) / 2.0);
}

- (double)pixelSpaceXToLongitude:(double)pixelX {
    return ((round(pixelX) - MERCATOR_OFFSET) / MERCATOR_RADIUS) * 180.0 / M_PI;
}

- (double)pixelSpaceYToLatitude:(double)pixelY {
    return (M_PI / 2.0 - 2.0 * atan(exp((round(pixelY) - MERCATOR_OFFSET) / 
                                        MERCATOR_RADIUS))) * 180.0 / M_PI;
}

#pragma mark - Helper Methods

- (MKCoordinateSpan)coordinateSpanWithMapView:(MKMapView *)mapView
                             centerCoordinate:(CLLocationCoordinate2D)center
                                 andZoomLevel:(NSUInteger)zoomLevel {
    // convert center coordiate to pixel space
    double centerPixelX = [self longitudeToPixelSpaceX:center.longitude];
    double centerPixelY = [self latitudeToPixelSpaceY:center.latitude];
    
    // determine the scale value from the zoom level
    NSInteger zoomExponent = 20 - zoomLevel;
    double zoomScale = pow(2, zoomExponent);
    
    // scale the map’s size in pixel space
    CGSize mapSizeInPixels = mapView.bounds.size;
    double scaledMapWidth = mapSizeInPixels.width * zoomScale;
    double scaledMapHeight = mapSizeInPixels.height * zoomScale;
    
    // figure out the position of the top-left pixel
    double topLeftPixelX = centerPixelX - (scaledMapWidth / 2);
    double topLeftPixelY = centerPixelY - (scaledMapHeight / 2);
    
    // find delta between left and right longitudes
    CLLocationDegrees minLng = [self pixelSpaceXToLongitude:topLeftPixelX];
    CLLocationDegrees maxLng = [self pixelSpaceXToLongitude:topLeftPixelX +
                                scaledMapWidth];
    CLLocationDegrees longitudeDelta = maxLng - minLng;
    
    // find delta between top and bottom latitudes
    CLLocationDegrees minLat = [self pixelSpaceYToLatitude:topLeftPixelY];
    CLLocationDegrees maxLat = [self pixelSpaceYToLatitude:topLeftPixelY +
                                scaledMapHeight];
    CLLocationDegrees latitudeDelta = -1 * (maxLat - minLat);
    
    // create and return the lat/lng span
    MKCoordinateSpan span = MKCoordinateSpanMake(latitudeDelta, longitudeDelta);
    return span;
}

- (NSUInteger)zoomLevelWithMapView:(MKMapView *)mapView
                              span:(MKCoordinateSpan)span {
    // A reverse-engineering of the existing zoom level calculation
    
    CLLocationDegrees longitudeDelta = span.longitudeDelta;
    
    CLLocationDegrees maxLng = mapView.centerCoordinate.longitude + longitudeDelta / 2;
    CLLocationDegrees minLng = mapView.centerCoordinate.longitude - longitudeDelta / 2;
    
    double scaledMapWidth = [self longitudeToPixelSpaceX:maxLng] -
        [self longitudeToPixelSpaceX:minLng];
    
    CGSize mapSizeInPixels = mapView.bounds.size;
    
    double zoomScale = scaledMapWidth / mapSizeInPixels.width;
    
    return (NSUInteger) 20 - log2(zoomScale);
}

#pragma mark - Public Methods

- (void)setCenterCoordinate:(CLLocationCoordinate2D)centerCoordinate
                  zoomLevel:(NSUInteger)zoomLevel
                   animated:(BOOL)animated {
    // clamp large numbers to 28
    zoomLevel = MIN(zoomLevel, 28);
    
    // use the zoom level to compute the region
    MKCoordinateSpan span = [self coordinateSpanWithMapView:self
                                           centerCoordinate:centerCoordinate
                                               andZoomLevel:zoomLevel];
    MKCoordinateRegion region = MKCoordinateRegionMake(centerCoordinate, span);
    
    // set the region like normal
    [self setRegion:region animated:animated];
}

#pragma mark - Property Methods

- (NSUInteger)zoomLevel {
    return [self zoomLevelWithMapView:self span:self.region.span];
}

@end