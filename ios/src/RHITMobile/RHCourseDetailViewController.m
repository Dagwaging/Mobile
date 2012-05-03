//
//  RHCourseDetailViewController.m
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

#import "RHCourseDetailViewController.h"
#import "RHCourse.h"
#import "RHDirectoryRequestsWrapper.h"
#import "RHUser.h"

@interface RHCourseDetailViewController () {
    @private
    BOOL _loaded;
}

@property (nonatomic, strong) IBOutlet UILabel *nameLabel;
@property (nonatomic, strong) IBOutlet UILabel *commentsLabel;
@property (nonatomic, strong) IBOutlet UILabel *creditHoursLabel;
@property (nonatomic, strong) IBOutlet UILabel *instructorLabel;
@property (nonatomic, strong) IBOutlet UILabel *finalLabel;
@property (nonatomic, strong) IBOutlet UILabel *enrolledLabel;
@property (nonatomic, strong) IBOutlet UILabel *maxEnrolledLabel;
@property (nonatomic, strong) IBOutlet UILabel *crnLabel;

@property (nonatomic, strong) RHCourse *course;

@end

@implementation RHCourseDetailViewController

@synthesize sourceCourse = _sourceCourse;
@synthesize nameLabel = _nameLabel;
@synthesize commentsLabel = _commentsLabel;
@synthesize creditHoursLabel = _creditHoursLabel;
@synthesize instructorLabel = _instructorLabel;
@synthesize finalLabel = _finalLabel;
@synthesize enrolledLabel = _enrolledLabel;
@synthesize maxEnrolledLabel = _maxEnrolledLabel;
@synthesize crnLabel = _crnLabel;
@synthesize course = _course;

- (id)initWithStyle:(UITableViewStyle)style
{
    self = [super initWithStyle:style];
    if (self) {
        // Custom initialization
    }
    return self;
}

- (void)viewDidLoad
{
    [super viewDidLoad];
    
    _loaded = NO;
    
    self.navigationItem.title = @"Loading...";
    
    UIActivityIndicatorView *activityIndicator = [[UIActivityIndicatorView alloc] initWithActivityIndicatorStyle:UIActivityIndicatorViewStyleGray];
    self.navigationItem.rightBarButtonItem = [[UIBarButtonItem alloc] initWithCustomView:activityIndicator];
    [activityIndicator startAnimating];
    
    [RHDirectoryRequestsWrapper makeCourseDetailRequestForCourse:self.sourceCourse successBlock:^(RHCourse *course) {
        
        self.course = course;
        
        self.navigationItem.title = course.courseNumber;
        self.navigationItem.rightBarButtonItem = nil;
        
        self.nameLabel.text = course.name;
        self.commentsLabel.text = course.comments;
        self.creditHoursLabel.text = course.credits.description;
        self.instructorLabel.text = course.instructor.fullName;
        self.finalLabel.text = course.finalRoom; // TODO
        self.enrolledLabel.text = course.enrolled.description;
        self.maxEnrolledLabel.text = course.maxEnrolled.description;
        self.crnLabel.text = course.crn.description;
        
        _loaded = YES;
        
        [self.tableView reloadData];
        
    } failureBlock:^(NSError *error) {
        [self.navigationController popViewControllerAnimated:YES];
    }];

    // Uncomment the following line to preserve selection between presentations.
    // self.clearsSelectionOnViewWillAppear = NO;
 
    // Uncomment the following line to display an Edit button in the navigation bar for this view controller.
    // self.navigationItem.rightBarButtonItem = self.editButtonItem;
}

- (void)viewDidUnload
{
    [super viewDidUnload];
    // Release any retained subviews of the main view.
    // e.g. self.myOutlet = nil;
}

- (BOOL)shouldAutorotateToInterfaceOrientation:(UIInterfaceOrientation)interfaceOrientation
{
    return (interfaceOrientation == UIInterfaceOrientationPortrait);
}

#pragma mark - Table view delegate

- (void)tableView:(UITableView *)tableView didSelectRowAtIndexPath:(NSIndexPath *)indexPath
{
    [tableView deselectRowAtIndexPath:indexPath animated:YES];
    
    // Navigation logic may go here. Create and push another view controller.
    /*
     <#DetailViewController#> *detailViewController = [[<#DetailViewController#> alloc] initWithNibName:@"<#Nib name#>" bundle:nil];
     // ...
     // Pass the selected object to the new view controller.
     [self.navigationController pushViewController:detailViewController animated:YES];
     */
}

@end
