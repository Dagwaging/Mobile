//
//  RHDirectionsRequester.h
//  RHITMobile
//
//  Created by Jimmy Theis on 12/11/11.
//  Copyright (c) 2011 Rose-Hulman Institute of Technology. All rights reserved.
//

#import <Foundation/Foundation.h>
#import "RHTourRequesterDelegate.h"

@interface RHTourRequester : NSObject

@property (nonatomic, retain) id<RHTourRequesterDelegate> delegate;

- (id)initWithDelegate:(id<RHTourRequesterDelegate>)delegate;

- (void)requestLocations;

@end
