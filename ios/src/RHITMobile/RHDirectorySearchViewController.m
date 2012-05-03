//
//  RHDirectorySearchViewController.m
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

#import "RHDirectorySearchViewController.h"
#import "RHAuthenticationLoader.h"
#import "RHCourse.h"
#import "RHCourseDetailViewController.h"
#import "RHDirectoryRequestsWrapper.h"
#import "RHDirectorySearchResult.h"
#import "RHFacultyDetailViewController.h"
#import "RHStudentDetailViewController.h"
#import "RHUser.h"

#define kStudentCell @"DirectoryStudentResultCell"
#define kFacultyCell @"DirectoryFacultyResultCell"
#define kCourseCell @"DirectoryCourseResultCell"
#define kNoResultsCell @"DirectoryNoResultsCell"

#define kStudentSegueIdentifier @"DirectorySearchToStudentDetailSegue"
#define kFacultySegueIdentifier @"DirectorySearchToFacultyDetailSegue"
#define kCourseSegueIdentifier @"DirectorySearchToCourseDetailSegue"

@interface RHDirectorySearchViewController () {
@private
    BOOL _hasSearched;
    BOOL _searching;
}

@property (nonatomic, strong) IBOutlet UITableView *tableView;

@property (nonatomic, strong) NSArray *results;

- (void)markSearching;

- (void)markNotSearching;

@end

@implementation RHDirectorySearchViewController

@synthesize tableView = _tableView;
@synthesize results = _results;

- (id)initWithNibName:(NSString *)nibNameOrNil bundle:(NSBundle *)nibBundleOrNil
{
    self = [super initWithNibName:nibNameOrNil bundle:nibBundleOrNil];
    if (self) {
        // Custom initialization
    }
    return self;
}

- (void)viewDidLoad
{
    [super viewDidLoad];
	// Do any additional setup after loading the view.
    
    _searching = NO;
    _hasSearched = NO;
}

- (void)viewDidUnload
{
    [super viewDidUnload];
    // Release any retained subviews of the main view.
}

- (BOOL)shouldAutorotateToInterfaceOrientation:(UIInterfaceOrientation)interfaceOrientation
{
    return (interfaceOrientation == UIInterfaceOrientationPortrait);
}

- (void)prepareForSegue:(UIStoryboardSegue *)segue sender:(id)sender
{
    if ([segue.identifier isEqualToString:kStudentSegueIdentifier]) {
        RHStudentDetailViewController *studentDetail = segue.destinationViewController;
        studentDetail.user = [self.results objectAtIndex:[[self.tableView indexPathForCell:sender] row]];
    } else if ([segue.identifier isEqualToString:kFacultySegueIdentifier]) {
        RHFacultyDetailViewController *facultyDetail = segue.destinationViewController;
        facultyDetail.user = [self.results objectAtIndex:[[self.tableView indexPathForCell:sender] row]];
    } else if ([segue.identifier isEqualToString:kCourseSegueIdentifier]) {
        RHCourseDetailViewController *courseDetail = segue.destinationViewController;
        courseDetail.sourceCourse = [self.results objectAtIndex:[[self.tableView indexPathForCell:sender] row]];
    }
}

- (NSArray *)results
{
    if (_results == nil) {
        _results = [NSArray array];
    }
    
    return _results;
}

#pragma mark - Table View Delegate Methods

- (void)tableView:(UITableView *)tableView didSelectRowAtIndexPath:(NSIndexPath *)indexPath
{
    [tableView deselectRowAtIndexPath:indexPath animated:YES];
}

#pragma mark - Table View Data Source Methods

- (NSInteger)tableView:(UITableView *)tableView numberOfRowsInSection:(NSInteger)section {
    return self.results.count == 0 && !_searching && _hasSearched ? 1 : self.results.count;
}

- (UITableViewCell *)tableView:(UITableView *)tableView cellForRowAtIndexPath:(NSIndexPath *)indexPath
{
    if (self.results.count == 0) {
        return [tableView dequeueReusableCellWithIdentifier:kNoResultsCell];
    }
    
    UITableViewCell *cell;
    
    NSObject<RHDirectorySearchResult> *result = [self.results objectAtIndex:indexPath.row];
    
    if ([result isKindOfClass:[RHUser class]] && [(RHUser *)result type] == RHUserTypeStudent) {
        cell = [tableView dequeueReusableCellWithIdentifier:kStudentCell];
    } else if ([result isKindOfClass:[RHUser class]]){
        cell = [tableView dequeueReusableCellWithIdentifier:kFacultyCell];
    } else {
        cell = [tableView dequeueReusableCellWithIdentifier:kCourseCell];
    }
    
    cell.textLabel.text = result.title;
    cell.detailTextLabel.text = result.subtitle;
    
    return cell;
}

#pragma mark - Search Bar Delegate Methods

- (void)searchBarCancelButtonClicked:(UISearchBar *)searchBar
{
    [searchBar resignFirstResponder];
}

- (void)searchBarSearchButtonClicked:(UISearchBar *)searchBar
{
    [searchBar resignFirstResponder];
    
    [self markSearching];
    
    int oldResultsSize = self.results.count;
    
    // Clear underlying results
    self.results = [NSArray array];
    
    if (oldResultsSize == 0 && _hasSearched) {
        // Animate deletion of "no results" cell
        [self.tableView deleteRowsAtIndexPaths:[NSArray arrayWithObject:[NSIndexPath indexPathForRow:0 inSection:0]] withRowAnimation:UITableViewRowAnimationAutomatic];
    } else if (oldResultsSize > 0) {
        // Animate deletion of old rows
        NSMutableArray *allIndexPaths = [NSMutableArray array];
        
        for (int i = 0; i < oldResultsSize; i ++) {
            [allIndexPaths addObject:[NSIndexPath indexPathForRow:i inSection:0]];
        }
        
        [self.tableView deleteRowsAtIndexPaths:allIndexPaths withRowAnimation:UITableViewRowAnimationAutomatic];
    }
    
    _hasSearched = YES;
    
    if (searchBar.selectedScopeButtonIndex == 0) {
        // People
        [RHDirectoryRequestsWrapper makeUserSearchRequestForSearchTerm:searchBar.text successBlock:^(NSArray *results) {
            
            // Update underlying results
            self.results = results;
            
            [self markNotSearching];
            
            if (results.count > 0) {
                // Animate insertion of new rows
                NSMutableArray *newIndexPaths = [NSMutableArray array];
                
                for (int i = 0; i < results.count; i ++) {
                    [newIndexPaths addObject:[NSIndexPath indexPathForRow:i inSection:0]];
                }
                
                [self.tableView insertRowsAtIndexPaths:newIndexPaths withRowAnimation:UITableViewRowAnimationAutomatic];
            } else {
                // Animate insertion of "no results" cell
                [self.tableView insertRowsAtIndexPaths:[NSArray arrayWithObject:[NSIndexPath indexPathForRow:0 inSection:0]] withRowAnimation:UITableViewRowAnimationAutomatic];
            }
            
        } failureBlock:^(NSError *error) {
            [self markNotSearching];
            NSLog(@"Error: %@", error);
        }];
        
    } else if (searchBar.selectedScopeButtonIndex == 1) {
        // Courses
        
        [RHDirectoryRequestsWrapper makeCourseSearchRequestForSearchTerm:searchBar.text term:[RHAuthenticationLoader.instance currentTerm] successBlock:^(NSArray *results) {
            
            // Update underlying results
            self.results = results;
            
            [self markNotSearching];
            
            if (results.count > 0) {
                // Animate insertion of new rows
                NSMutableArray *newIndexPaths = [NSMutableArray array];
                
                for (int i = 0; i < results.count; i ++) {
                    [newIndexPaths addObject:[NSIndexPath indexPathForRow:i inSection:0]];
                }
                
                [self.tableView insertRowsAtIndexPaths:newIndexPaths withRowAnimation:UITableViewRowAnimationAutomatic];
            } else {
                // Animate insertion of "no results" cell
                [self.tableView insertRowsAtIndexPaths:[NSArray arrayWithObject:[NSIndexPath indexPathForRow:0 inSection:0]] withRowAnimation:UITableViewRowAnimationAutomatic];
            }
            
        } failureBlock:^(NSError *error) {
            [self markNotSearching];
            NSLog(@"Error: %@", error);
        }];
    }
}

#pragma mark - Private Methods

- (void)markSearching
{
    _searching = YES;
    
    self.navigationItem.title = @"Searching";
    
    UIActivityIndicatorView *activityIndicator = [[UIActivityIndicatorView alloc] initWithActivityIndicatorStyle:UIActivityIndicatorViewStyleGray];
    self.navigationItem.rightBarButtonItem = [[UIBarButtonItem alloc] initWithCustomView:activityIndicator];
    [activityIndicator startAnimating];
}

- (void)markNotSearching
{
    _searching = NO;
    
    self.navigationItem.rightBarButtonItem = nil;
    self.navigationItem.title = @"Directory Search";
}

@end
