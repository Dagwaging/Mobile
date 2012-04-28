//
//  RHDepartureLocationSelectionViewControllerViewController.h
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

#import <CoreLocation/CoreLocation.h>

#import "RHLocationSearchViewController.h"


@class RHLocation;


@interface RHDepartureLocationCell : UITableViewCell

@property (nonatomic, strong) RHLocation *location;

@property (nonatomic, strong) IBOutlet UILabel *locationNameLabel;

@property (nonatomic, strong) IBOutlet UILabel *locationDetailLabel;

@end


@interface RHDepartureLocationSelectionViewControllerViewController : RHLocationSearchViewController <CLLocationManagerDelegate>

@property (nonatomic, assign) void(^locationChosenBlock)(RHLocation *);

@property (nonatomic, assign) void(^gpsChosenBlock)(CLLocation *);

@end
