//
//  RHCampusServicesViewController.h
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

#import <UIKit/UIKit.h>

#import "RHLoader.h"


/// A view controller that displays a listing of RHServiceItem (RHServiceCategory
/// and RHServiceLink) objects in a table. Selecting a category table cell will
/// cause a "drill-down" into another RHCampusServicesViewController, while selecting
/// a link cell will segue to an RHWebViewController.
@interface RHCampusServicesViewController : UITableViewController <RHLoaderDelegate>

/// An NSArray of RHServiceItem objects, pre-sorted, to be displayed by this view controller.
/// This property will be automatically populated in the root campus services view controller,
/// but later instances of this view controller will need to have it set before segue.
@property (nonatomic, strong) NSArray *serviceItems;

@end
