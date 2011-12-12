//
//  RHDirectionsRequesterDelegate.h
//  RHITMobile
//
//  Created by Jimmy Theis on 12/11/11.
//  Copyright (c) 2011 Rose-Hulman Institute of Technology. All rights reserved.
//

#import <Foundation/Foundation.h>

@protocol RHDirectionsRequesterDelegate <NSObject>

- (void)didFinishLoadingDirections:(NSArray *)directions;

@end
