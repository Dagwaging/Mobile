//
//  RHTourRequesterDelegate.h
//  RHITMobile
//
//  Created by Jimmy Theis on 12/13/11.
//  Copyright (c) 2011 Rose-Hulman Institute of Technology. All rights reserved.
//

#import <Foundation/Foundation.h>

@protocol RHTourRequesterDelegate <NSObject>

- (void)didFinishLoadingTour:(NSArray *)directions;

@end
