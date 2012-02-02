//
//  RHAnnotationView.m
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

#import "RHAnnotationView.h"
#import "RHAnnotationViewDelegate.h"
#import "RHAnnotation.h"
#import "RHMapLabel.h"
#import "RHLocation.h"
#import "RHMapViewController.h"


#pragma mark Private Method Declarations

@interface RHAnnotationView () 

- (void) createText;

@end


#pragma mark -
#pragma mark Implementation

@implementation RHAnnotationView

#pragma mark -
#pragma mark Generic Properties

@synthesize delegate;
@synthesize textView;

#pragma mark -
#pragma mark General Methods

- (id)initWithAnnotation:(id<MKAnnotation>)inAnnotation
         reuseIdentifier:(NSString *)reuseIdentifier {
    self = [super initWithAnnotation:inAnnotation
                     reuseIdentifier:reuseIdentifier];
    
    self.backgroundColor = [UIColor clearColor];
    self.frame = CGRectMake(0, 0, 100, 50);
    
    [self createText];
    
    if (!self.storedAnnotation.visible) {
        self.textView.hidden = YES;
    }
    
    return self;
}


#pragma mark -
#pragma mark Property Methods

- (void)setSelected:(BOOL)inSelected {
    [super setSelected:inSelected];
    if (inSelected) {
        [delegate focusMapViewToAreaAnnotation:self.storedAnnotation
                                      selected:YES];
    } else {
        [delegate performSelector:@selector(clearUnusedOverlays)
                       withObject:nil
                       afterDelay:0.01];
        [delegate performSelector:@selector(clearUnusedOverlays)
                       withObject:nil
                       afterDelay:0.3];
    }
}

- (void)updateAnnotationVisibility {
    if (!self.storedAnnotation.visible) {
        self.textView.hidden = YES;
    } else {
        self.textView.hidden = NO;
    }
}

- (RHAnnotation *)storedAnnotation {
    return (RHAnnotation *) self.annotation;
}

#pragma mark -
#pragma mark Private Methods

- (void) createText {
    textView = [[RHMapLabel alloc] initWithFrame:CGRectMake(2, 2, 96, 26)];
    [textView setNumberOfLines:2];
    [textView setLineBreakMode:UILineBreakModeWordWrap];
    textView.backgroundColor = [UIColor clearColor];
    textView.text = self.storedAnnotation.location.name;
    textView.font = [UIFont fontWithName:@"Arial-BoldMT" size:10];
    textView.textColor = [UIColor whiteColor];
    textView.textAlignment = UITextAlignmentCenter;
    [self addSubview:textView];
}

@end