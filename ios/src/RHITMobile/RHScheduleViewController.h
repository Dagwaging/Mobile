//
//  RHScheduleViewController.h
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

/**
 * The view controller that wraps the scrollable day schedule tables,
 * providing a scroll view, a page control, and a navigation bar button item
 * for changing the current term (when applicable). The code in this class is
 * based heavily on Apple's **PageControl** sample (available at
 * <https://developer.apple.com/library/ios/samplecode/PageControl>)
 */
@interface RHScheduleViewController : UIViewController <UIScrollViewDelegate>

@end
