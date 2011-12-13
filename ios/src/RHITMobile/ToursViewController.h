//
//  ToursViewController.h
//  RHITMobile
//
//  Created by Jimmy Theis on 12/12/11.
//  Copyright (c) 2011 Rose-Hulman Institute of Technology. All rights reserved.
//

#import <UIKit/UIKit.h>
#import "RHTourRequesterDelegate.h"

@class RHTourRequester;

@interface ToursViewController : UIViewController <RHTourRequesterDelegate> {
    @private
    RHTourRequester *currentDirectionsRequest_;
}

- (IBAction)selectTour:(id)sender;

@end
