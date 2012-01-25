//
//  PersonDetailViewController.m
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

#import "PersonDetailViewController.h"

#define kCellReuseIdentifier @"PersonDetailCell"
#define kScheduleCellReuseIdentifier @"ScheduleCell"
#define kScheduleTitle @"Schedule"
#define kPhoneTitle @"Phone"
#define kEmailTitle @"Email"
#define kMailboxTitle @"Mailbox"
#define kLocationTitle @"Location"
#define kWebPageTitle @"Web Page"

@implementation PersonDetailViewController

@synthesize sections;
@synthesize displayValues;

- (id)initWithNibName:(NSString *)nibNameOrNil
               bundle:(NSBundle *)nibBundleOrNil {
    self = [super initWithNibName:nibNameOrNil bundle:nibBundleOrNil];
    if (self) {
        self.sections = [NSArray arrayWithObjects:kScheduleTitle, kPhoneTitle, kEmailTitle, kMailboxTitle, kLocationTitle, kWebPageTitle, nil];
        NSMutableDictionary *values = [NSMutableDictionary dictionaryWithCapacity:self.sections.count];
        [values setObject:@"(317) 782-5424" forKey:kPhoneTitle];
        [values setObject:@"theisje@rose-hulman.edu" forKey:kEmailTitle];
        [values setObject:@"CM 1943" forKey:kMailboxTitle];
        [values setObject:@"F217" forKey:kLocationTitle];
        [values setObject:@"http://www.rose-hulman.edu/~theisje" forKey:kWebPageTitle];
        self.displayValues = values;
    }
    return self;
}

- (void)didReceiveMemoryWarning {
    // Releases the view if it doesn't have a superview.
    [super didReceiveMemoryWarning];
    
    // Release any cached data, images, etc that aren't in use.
}

#pragma mark - View lifecycle

- (void)viewDidLoad {
    [super viewDidLoad];
    // Do any additional setup after loading the view from its nib.
}

- (void)viewDidUnload {
    [super viewDidUnload];
    // Release any retained subviews of the main view.
    // e.g. self.myOutlet = nil;
}

- (BOOL)shouldAutorotateToInterfaceOrientation:(UIInterfaceOrientation)interfaceOrientation {
    // Return YES for supported orientations
    return (interfaceOrientation == UIInterfaceOrientationPortrait);
}

- (NSString *)title {
    return @"Jimmy Theis";
}

#pragma mark - UITableViewDelegate Methods

- (CGFloat)tableView:(UITableView *)tableView heightForHeaderInSection:(NSInteger)section {
    if (section == 0) {
        return 100;
    }
    
    return 0;
}

- (UIView *)tableView:(UITableView *)tableView viewForHeaderInSection:(NSInteger)section {
    if (section == 0) {
        UIView *wrappingView = [[UIView alloc] initWithFrame:CGRectMake(0, 0, 320, 150)];
        
        UIView *portrait = [[UIView alloc] initWithFrame:CGRectMake(20, 20, 60, 60)];
        portrait.backgroundColor = [UIColor grayColor];
        [wrappingView addSubview:portrait];
        
        UILabel *nameLabel = [[UILabel alloc] initWithFrame:CGRectMake(100, 25, 200, 20)];
        nameLabel.backgroundColor = [UIColor clearColor];
        nameLabel.font = [UIFont boldSystemFontOfSize:18];
        nameLabel.text = @"Jimmy Theis";
        [wrappingView addSubview:nameLabel];
        
        UILabel *positionLabel = [[UILabel alloc] initWithFrame:CGRectMake(100, 55, 200, 20)];
        positionLabel.backgroundColor = [UIColor clearColor];
        positionLabel.font = [UIFont systemFontOfSize:14];
        positionLabel.text = @"Student (Senior SE)";
        [wrappingView addSubview:positionLabel];
        
        return wrappingView;
    }
    
    return nil;
}

- (void)tableView:(UITableView *)tableView
didSelectRowAtIndexPath:(NSIndexPath *)indexPath {
    [tableView deselectRowAtIndexPath:indexPath animated:YES];
}

#pragma mark - UITableViewDataSource Methods

- (NSInteger)numberOfSectionsInTableView:(UITableView *)tableView {
    return self.sections.count;
}

- (UITableViewCell *)tableView:(UITableView *)tableView
         cellForRowAtIndexPath:(NSIndexPath *)indexPath {
    UITableViewCell *cell;
    
    if (indexPath.section == 0) {
        cell = [tableView dequeueReusableCellWithIdentifier:kScheduleCellReuseIdentifier];
        
        if (cell == nil) {
            cell = [[UITableViewCell alloc] initWithStyle:UITableViewCellStyleValue1 reuseIdentifier:kScheduleCellReuseIdentifier];
            cell.accessoryType = UITableViewCellAccessoryDisclosureIndicator;
        }
        
        cell.textLabel.text = @"Go to Schedule";
        
        return cell;
    }
    
    cell = [tableView dequeueReusableCellWithIdentifier:kCellReuseIdentifier];
    
    if (cell == nil) {
        cell = [[UITableViewCell alloc] initWithStyle:UITableViewCellStyleValue2
                                      reuseIdentifier:kCellReuseIdentifier];
        cell.detailTextLabel.adjustsFontSizeToFitWidth = YES;
    }
    
    NSString *sectionTitle = [self.sections objectAtIndex:indexPath.section];
    cell.textLabel.text = sectionTitle;
    cell.detailTextLabel.text = [self.displayValues objectForKey:sectionTitle];
    
    return cell;
}

- (NSInteger)tableView:(UITableView *)tableView
 numberOfRowsInSection:(NSInteger)section {
    return 1;
}

@end
