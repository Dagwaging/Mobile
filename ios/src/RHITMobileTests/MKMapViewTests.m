//
//  MKMapViewTests.m
//  RHITMobile
//
//  Created by Jimmy Theis on 10/4/11.
//  Copyright 2011 Rose-Hulman Institute of Technology. All rights reserved.
//

#import "MKMapViewTests.h"

@implementation MKMapViewTests

#if USE_APPLICATION_UNIT_TEST     // all code under test is in the iPhone Application

- (void)testAppDelegate {
    
    id yourApplicationDelegate = [[UIApplication sharedApplication] delegate];
    STAssertNotNil(yourApplicationDelegate, @"UIApplication failed to find the AppDelegate");
    
}

#else                           // all code under test must be linked into the Unit Test bundle

- (void)testMath {
    
    STAssertTrue((1+1)==2, @"Compiler isn't feeling well today :-(" );
    
}

#endif

@end
