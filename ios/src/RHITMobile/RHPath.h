//
//  RHPath.h
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

#import <Foundation/Foundation.h>


@interface RHPath : NSObject

@property (nonatomic, strong) NSNumber *distance;

@property (nonatomic, strong) NSNumber *stairsUp;

@property (nonatomic, strong) NSNumber *stairsDown;

@property (nonatomic, strong) NSArray *steps;

@property (nonatomic, assign) BOOL turnByTurn;

@end
