//
//  RHBeta.h
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

// Comment out this line to build the app in non-beta mode.
#define RHITMobile_RHBeta


#ifdef RHITMobile_RHBeta

// Definitions of build types
#define kRHBetaBuildTypeRolling 0
#define kRHBetaBuildTypeOfficial 1

// Build number
#define kRHBetaBuildNumber -1

// Build type
#define kRHBetaBuildType kRHBetaBuildTypeRolling

#endif
