//
//  MKMapView+ZoomLevel.h
//  RHIT Mobile Campus Directory
//
//  Copyright 2011 Rose-Hulman Institute of Technology
//
//  This code provided publicly by Troy Brant without licensing terms:
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

#import <MapKit/MapKit.h>


/// \ingroup map
/// MapKit MKMapView category that includes setting a canonical zoom level.
@interface MKMapView(ZoomLevel)

/// Retrievable zoom level that is calculated based on the MapView's current
/// region.
@property (nonatomic, readonly) NSUInteger zoomLevel;

/// Set the MapView's visible region with a more traditional Maps API
/// mechanism.
/// \param centerCoordinate The latitude/longitude point which will be centered
/// in the MapView.
/// \param zoomLevel The canonical zoom level to set the MapView to. This will
/// affect the region that is visible in the MapView. Value-wise, this should
/// be less than 20.
/// \param animated Whether or not to animate the transition to this new region.
- (void)setCenterCoordinate:(CLLocationCoordinate2D)centerCoordinate
                  zoomLevel:(NSUInteger)zoomLevel
                   animated:(BOOL)animated;

@end