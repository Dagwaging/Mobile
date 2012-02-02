//
//  RHSearchViewController.h
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

#import <UIKit/UIKit.h>

#define kRHSearchViewControllerNibName @"RHSearchViewController"

@class RHRestHandler;

typedef enum RHSearchViewControllerType_ {
    RHSearchViewControllerTypeLocation = 0,
    RHSearchViewControllerTypePeople
} RHSearchViewControllerType;

/// \ingroup views
/// View controller for the search portion of the application.
@interface RHSearchViewController : UIViewController <UISearchBarDelegate, UITableViewDelegate, UITableViewDataSource> {
}

@property (nonatomic, assign) RHSearchViewControllerType searchType;

@property (nonatomic, assign) BOOL searchInitiated;

@property (atomic, strong) NSString *currentAutocompleteTerm;

@property (unsafe_unretained, atomic, readonly) NSDictionary *autocompleteData;

@property (atomic, strong) NSMutableArray *searchResults;

@property (nonatomic, strong) IBOutlet UISearchBar *searchBar;

@property (nonatomic, strong) IBOutlet UITableView *tableView;

@property (nonatomic, strong) RHRestHandler *remoteHandler;

@property (nonatomic, strong) NSManagedObjectContext *context;

- (void)tryAutocomplete:(NSString *)searchTerm;

- (void)didFindSearchResults:(NSArray *)searchResults;

- (id)objectFromResult:(id)result;

@end