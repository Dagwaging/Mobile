//
//  RHDirectionsRequester.h
//  RHITMobile
//
//  Created by Jimmy Theis on 12/11/11.
//  Copyright (c) 2011 Rose-Hulman Institute of Technology. All rights reserved.
//

#import <Foundation/Foundation.h>
#import "RHDirectionsRequesterDelegate.h"

@interface RHDirectionsRequester : NSObject

@property (nonatomic, retain) id<RHDirectionsRequesterDelegate> delegate;

- (id)initWithDelegate:(id<RHDirectionsRequesterDelegate>)delegate;

- (void)requestLocations;

@end
