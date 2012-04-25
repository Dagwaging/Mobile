//
//  RHQuickListViewController.m
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

#import "RHQuickListViewController.h"
#import "RHMapViewController.h"
#import "RHAnnotation.h"
#import "RHLocation.h"

@interface RHQuickListViewController() {
@private
    NSUInteger currentSelection_;
}
@end

@implementation RHQuickListViewController

@synthesize mapViewController;
@synthesize pickerView;
@synthesize tableView;

// TODO: This class needs to update when an update occurs

- (id)initWithNibName:(NSString *)nibNameOrNil bundle:(NSBundle *)nibBundleOrNil
{
    self = [super initWithNibName:nibNameOrNil bundle:nibBundleOrNil];
    if (self) {
        currentSelection_ = 0;
    }
    return self;
}

- (void)didReceiveMemoryWarning
{
    // Releases the view if it doesn't have a superview.
    [super didReceiveMemoryWarning];
    
    // Release any cached data, images, etc that aren't in use.
}

#pragma mark - View lifecycle

- (void)viewDidLoad
{
    [super viewDidLoad];
    // Do any additional setup after loading the view from its nib.
}

- (void)viewDidUnload
{
    [super viewDidUnload];
    // Release any retained subviews of the main view.
    // e.g. self.myOutlet = nil;
}

- (BOOL)shouldAutorotateToInterfaceOrientation:(UIInterfaceOrientation)interfaceOrientation
{
    // Return YES for supported orientations
    return (interfaceOrientation == UIInterfaceOrientationPortrait);
}

- (IBAction)goToLocation:(id)sender {
    RHAnnotation *annotation = (RHAnnotation *)
        [self.mapViewController.quickListAnnotations
         objectAtIndex:currentSelection_];
    [self.mapViewController focusMapViewToAnnotation:annotation];
    [self dismissModalViewControllerAnimated:YES];
}

- (IBAction)cancel:(id)sender {
    [self dismissModalViewControllerAnimated:YES];
}

- (NSArray *)quickListAnnotations {
    return self.mapViewController.quickListAnnotations;
}

- (RHAnnotation *)currentAnnotation {
    return [self.quickListAnnotations objectAtIndex:currentSelection_];
}

- (NSInteger)numberOfComponentsInPickerView:(UIPickerView *)pickerView {
    return 1;
}

- (NSInteger)pickerView:(UIPickerView *)pickerView
numberOfRowsInComponent:(NSInteger)component {
    return self.mapViewController.quickListAnnotations.count;
}

- (NSString *)pickerView:(UIPickerView *)pickerView
             titleForRow:(NSInteger)row
            forComponent:(NSInteger)component {
    return ((RHAnnotation *)[self.mapViewController.quickListAnnotations
                             objectAtIndex:row]).location.name;
}

- (void)pickerView:(UIPickerView *)pickerView
      didSelectRow:(NSInteger)row
       inComponent:(NSInteger)component {
    currentSelection_ = row;
    [self.tableView reloadData];
}

- (NSInteger)tableView:(UITableView *)tableView
 numberOfRowsInSection:(NSInteger)section {
    return 2;
}

- (UITableViewCell *)tableView:(UITableView *)inTableView
         cellForRowAtIndexPath:(NSIndexPath *)indexPath {
    static NSString *cellIdentifier = @"RHCell";
    UITableViewCell *cell = [tableView
                             dequeueReusableCellWithIdentifier:cellIdentifier];
    if (cell == nil) {
        cell = [[UITableViewCell alloc]
                initWithStyle:UITableViewCellStyleDefault
                reuseIdentifier:cellIdentifier];
    }
    
    if (indexPath.row == 0) {
        cell.textLabel.text = self.currentAnnotation.location.name;
    } else {
        cell.textLabel.numberOfLines = 4;
        cell.textLabel.font = [UIFont systemFontOfSize:[UIFont systemFontSize]];
        cell.textLabel.text = self.currentAnnotation.location.quickDescription;
    }
    
    cell.selectionStyle = UITableViewCellSelectionStyleNone;
    
    return cell;
}

- (CGFloat)tableView:(UITableView *)tableView heightForRowAtIndexPath:(NSIndexPath *)indexPath {
    if (indexPath.row == 1) {
        CGSize maximumLabelSize = CGSizeMake(290, 9999);
        
        CGSize expectedLabelSize = [self.currentAnnotation.location.quickDescription 
                                    sizeWithFont:[UIFont systemFontOfSize:UIFont.systemFontSize]
                                    constrainedToSize:maximumLabelSize 
                                    lineBreakMode:UILineBreakModeTailTruncation]; 
        
        return MIN(expectedLabelSize.height + 20, 90);
    }
    
    return 44;
}

@end
