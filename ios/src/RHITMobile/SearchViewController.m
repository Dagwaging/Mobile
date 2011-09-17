//
//  SearchViewController.m
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

#import "SearchViewController.h"

@implementation SearchViewController

@synthesize searchBar;

- (void)viewDidLoad {
    [super viewDidLoad];
}


- (BOOL) shouldAutorotateToInterfaceOrientation:(UIInterfaceOrientation)interfaceOrientation {
    return (interfaceOrientation == UIInterfaceOrientationPortrait);
}

- (void) didReceiveMemoryWarning {
    [super didReceiveMemoryWarning];
    
    // Release any cached data, images, etc. that aren't in use.
}

- (void) viewDidUnload {
    [super viewDidUnload];
    self.searchBar = nil;
}

#pragma mark -
#pragma mark UISearchBarDelegate Methods

- (void)searchBarCancelButtonClicked:(UISearchBar *)inSearchBar {
    [inSearchBar resignFirstResponder];
}

- (void)searchBarSearchButtonClicked:(UISearchBar *)inSearchBar {
    [inSearchBar resignFirstResponder];
    UIAlertView *alert = [[UIAlertView alloc] initWithTitle:@"Sorry..."
                                                    message:@"Searching isn't ready yet. We'll let you know when it is."
                                                   delegate:nil
                                          cancelButtonTitle:@"OK"
                                          otherButtonTitles:nil];
    [alert show];
    [alert release];
}

@end
