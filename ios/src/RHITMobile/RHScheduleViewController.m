//
//  RHScheduleViewController.m
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

#import "RHScheduleViewController.h"
#import "RHMeetingDetailViewController.h"
#import "RHSchedule.h"
#import "RHScheduleTableViewController.h"

#define kNumberOfDays 6

#define kDayScheduleViewControllerIdentifier @"DayScheduleViewController"
#define kCourseMeetingSegueIdentifier @"ScheduleToCourseMeetingDetailSegue"


@interface RHScheduleViewController () {
    @private
    BOOL _pageControlUsed;
}

@property (nonatomic, strong) IBOutlet UIScrollView *scrollView;

@property (nonatomic, strong) IBOutlet UIPageControl *pageControl;

@property (nonatomic, strong) NSMutableArray *viewControllers;

- (void)loadScrollViewWithDayIndex:(int)dayIndex;

@end


@implementation RHScheduleViewController

@synthesize schedule = _schedule;
@synthesize scrollView = _scrollView;
@synthesize pageControl = _pageControl;
@synthesize viewControllers = _viewControllers;

static NSString *_dayNames[] = { @"Monday", @"Tuesday", @"Wednesday", @"Thursday", @"Friday", @"Saturday" };

#pragma mark - View lifecycle

- (void)viewDidLoad
{
    [super viewDidLoad];
    
    // Initialize view controllers with null values
    NSMutableArray *controllers = [[NSMutableArray alloc] init];
    for (unsigned i = 0; i < kNumberOfDays; i++)
    {
		[controllers addObject:[NSNull null]];
    }
    self.viewControllers = controllers;
    
    // Configure scroll view
    self.scrollView.contentSize = CGSizeMake(self.scrollView.frame.size.width * kNumberOfDays, self.scrollView.frame.size.height);
    self.scrollView.scrollsToTop = NO;
    
    // Get the current day of the week
    NSDateFormatter* theDateFormatter = [[NSDateFormatter alloc] init];
    [theDateFormatter setFormatterBehavior:NSDateFormatterBehavior10_4];
    [theDateFormatter setDateFormat:@"EEEE"];
    NSString *weekDay =  [theDateFormatter stringFromDate:[NSDate date]];
    
    int dateIndex;
    
    if ([weekDay isEqualToString:@"Tuesday"]) dateIndex = 1;
    else if ([weekDay isEqualToString:@"Wednesday"]) dateIndex = 2;
    else if ([weekDay isEqualToString:@"Thursday"]) dateIndex = 3;
    else if ([weekDay isEqualToString:@"Friday"]) dateIndex = 4;
    else if ([weekDay isEqualToString:@"Saturday"]) dateIndex = 5;
    else dateIndex = 0;
    
    // Configure page control
    self.pageControl.currentPage = dateIndex;
    
    [self changePage:self];
}

- (BOOL)shouldAutorotateToInterfaceOrientation:(UIInterfaceOrientation)interfaceOrientation
{
    return (interfaceOrientation == UIInterfaceOrientationPortrait);
}

- (void)prepareForSegue:(UIStoryboardSegue *)segue sender:(id)sender
{
    if ([segue.identifier isEqualToString:kCourseMeetingSegueIdentifier]) {
        RHMeetingDetailViewController *meetingDetail = segue.destinationViewController;
        meetingDetail.meeting = sender;
    }
}

#pragma mark - IBActions

- (IBAction)changePage:(id)sender
{
    int dayIndex = self.pageControl.currentPage;
	
    [self loadScrollViewWithDayIndex:dayIndex - 1];
    [self loadScrollViewWithDayIndex:dayIndex];
    [self loadScrollViewWithDayIndex:dayIndex + 1];
    
    CGRect frame = self.scrollView.frame;
    frame.origin.x = frame.size.width * dayIndex;
    frame.origin.y = 0;
    [self.scrollView scrollRectToVisible:frame animated:YES];
    
    _pageControlUsed = YES;
    
    self.navigationItem.title = _dayNames[dayIndex];
}

#pragma mark - Scroll view delegate

- (void)scrollViewDidScroll:(UIScrollView *)sender
{
    if (_pageControlUsed)
    {
        return;
    }
	
    // Switch the indicator when more than 50% of the previous/next page is visible
    CGFloat pageWidth = self.scrollView.frame.size.width;
    int dayIndex = floor((self.scrollView.contentOffset.x - pageWidth / 2) / pageWidth) + 1;
    
    if (dayIndex < 0 || dayIndex >= kNumberOfDays) {
        return;
    }
    
    self.pageControl.currentPage = dayIndex;
    
    [self loadScrollViewWithDayIndex:dayIndex - 1];
    [self loadScrollViewWithDayIndex:dayIndex];
    [self loadScrollViewWithDayIndex:dayIndex + 1];
    
    self.navigationItem.title = _dayNames[dayIndex];
}

- (void)scrollViewWillBeginDragging:(UIScrollView *)scrollView
{
    _pageControlUsed = NO;
}

- (void)scrollViewDidEndDecelerating:(UIScrollView *)scrollView
{
    _pageControlUsed = NO;
}

#pragma mark - Private methods

- (void)loadScrollViewWithDayIndex:(int)dayIndex
{
    if (dayIndex < 0) {
        return;
    }

    if (dayIndex >= kNumberOfDays) {
        return;
    }
    
    // Replace an NSNull placeholder if necessary
    RHScheduleTableViewController *controller = [self.viewControllers objectAtIndex:dayIndex];
    if ((NSNull *)controller == [NSNull null])
    {
        //controller = [[RHScheduleTableViewController alloc] init];
        controller = [self.storyboard instantiateViewControllerWithIdentifier:kDayScheduleViewControllerIdentifier];
        [self.viewControllers replaceObjectAtIndex:dayIndex withObject:controller];
    }
    
    // Add to the scroll view
    if (controller.view.superview == nil)
    {
        CGRect frame = self.scrollView.frame;
        frame.origin.x = frame.size.width * dayIndex;
        frame.origin.y = 0;
        controller.view.frame = frame;
        [self.scrollView addSubview:controller.view];
        
        controller.scheduleEntries = [self.schedule scheduleEntriesForDayIndex:[NSNumber numberWithInt:dayIndex]];
        controller.delegate = self;
    }
}

#pragma mark - Schedule table view controller delegate

- (void)didSelectCourseMeeting:(RHCourseMeeting *)meeting
{
    [self performSegueWithIdentifier:kCourseMeetingSegueIdentifier sender:meeting];
}

@end
