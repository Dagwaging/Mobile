//
//  RHPinAnnotationView.h
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

#import <MapKit/MapKit.h>


@class MapViewController;


/// \ingroup map
/// MKPinAnnotationView subclass capable of notifying the rest of the app of
/// when it is selected and deselected. This is specifically useful for
/// maintaining the correct set of annotations and overlays on the campus
/// MKMapView.
@interface RHPinAnnotationView : MKPinAnnotationView

/// The MapViewController responsible for this RHPinAnnotationView. This object
/// will be the one receiving updates about the pin's selected status.
@property (nonatomic, retain) MapViewController *mapViewController;

@end
