//
//  RHAnnotationView.m
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

#import "RHAnnotationView.h"
#import "RHAnnotationViewDelegate.h"
#import "RHAnnotation.h"
#import "RHMapLabel.h"
#import "RHLocation.h"


@interface RHAnnotationView () 

- (void) createText;

@end

@implementation RHAnnotationView

@synthesize delegate;
@synthesize textView;
@synthesize storedAnnotation;

- (id)initWithAnnotation:(id<MKAnnotation>)inAnnotation
         reuseIdentifier:(NSString *)reuseIdentifier {
    self = [super initWithAnnotation:inAnnotation
                     reuseIdentifier:reuseIdentifier];
    
    self.storedAnnotation = (RHAnnotation *)inAnnotation;
    self.backgroundColor = [UIColor clearColor];
    
    switch (storedAnnotation.annotationType) {
            
        case RHAnnotationTypeText:
            self.frame = CGRectMake(0, 0, 100, 50);

            [self createText];
            break;
            
        case RHAnnotationTypePolygon:
            /// \todo Add polygon rendering
            break;
            
        case RHAnnotationTypeTextAndPolygon:
            /// \todo Add polygon rendering
            self.frame = CGRectMake(0, 0, 100, 30);
            [self createText];
            break;
    }
    
    return self;
}

- (void) createText {
    textView = [[RHMapLabel alloc] initWithFrame:CGRectMake(2, 2, 96, 26)];
    [textView setNumberOfLines:2];
    [textView setLineBreakMode:UILineBreakModeWordWrap];
    textView.backgroundColor = [UIColor clearColor];
    textView.text = storedAnnotation.location.name;
    textView.font = [UIFont fontWithName:@"Arial-BoldMT" size:10];
    textView.textColor = [UIColor whiteColor];
    textView.textAlignment = UITextAlignmentCenter;
    [self addSubview:textView];
}

- (void)setSelected:(BOOL)inSelected {
    if (inSelected) {
        [delegate focusMapViewToLocation:[[self storedAnnotation] location]];
    }
    [super setSelected:inSelected];
}

- (void) dealloc {
    [textView release];
    [storedAnnotation release];
    [super dealloc];
}

@end