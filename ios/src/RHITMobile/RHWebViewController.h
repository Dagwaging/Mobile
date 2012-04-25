//
//  RHWebViewController.h
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


/// A view controller used to frame a web page. Meant to be pushed into a 
/// UINavigationViewController, this view controller provides an "Open in Safari"
/// button in the navigation item, which switches the user to the full blown Safari
/// app with the current web page.
@interface RHWebViewController : UIViewController

/// The IBOutlet for this view controller's web view.
@property (nonatomic, strong) IBOutlet UIWebView *webView;

/// The NSURL to frame in this view controller's web view. This propery needs to be set
/// before a segue orrurs.
@property (nonatomic, strong) NSURL *url;

/// The IBAction caused by tapping the "Open in Safari" button in the navigation item.
- (IBAction)openInSafari;

@end
