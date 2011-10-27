//
//  RHRemoteHandler.h
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

#import <Foundation/Foundation.h>
#import <CoreData/CoreData.h>


@class RHRemoteHandlerDelegate;

/// \ingroup web
/// Wrapper for remote data fetching and posting.
@protocol RHRemoteHandler <NSObject>

/// RHRemoteHandlerDelgate to interact with.
@property (nonatomic, retain) RHRemoteHandlerDelegate *delegate;

/// Init with a managed object context for object creation;
- (id)initWithPersistantStoreCoordinator:(NSPersistentStoreCoordinator *)coordinator
                                delegate:(RHRemoteHandlerDelegate *)delegate;

/// Asynchronously check for new data from the server. If there is new data,
/// appropriate callbacks will be called on the RHRemoteHandlerDelegate.
- (void)checkForLocationUpdates;

/// Asynchronously pull down information for all incomplete locations, as well
/// as all locations enclosed in these locations.
- (void)populateUnderlyingLocations;

@end