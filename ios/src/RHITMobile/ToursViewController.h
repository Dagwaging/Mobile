//
//  ToursViewController.h
//  RHITMobile
//
//  Created by Jimmy Theis on 12/12/11.
//  Copyright (c) 2011 Rose-Hulman Institute of Technology. All rights reserved.
//

#import <UIKit/UIKit.h>
#import "RHDirectionsRequesterDelegate.h"

@class RHDirectionsRequester;

@interface ToursViewController : UIViewController <RHDirectionsRequesterDelegate> {
    @private
    RHDirectionsRequester *currentDirectionsRequest_;
}

- (IBAction)selectTour:(id)sender;

@end
