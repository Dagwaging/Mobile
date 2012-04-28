//
//  RHLocationSearchRequest.m
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

#import "RHLocationSearchRequest.h"
#import "RHAppDelegate.h"
#import "RHJSONRequest.h"
#import "RHLocation.h"


#define kAllLocationsPath @"/locations/names"
#define kDepartableLocationsPath @"/locations/names/departable"

#define kSearchTermURLKey @"s"

#define kLocationsKey @"Names"
#define kServerIdentifierKey @"Id"


@implementation RHLocationSearchRequest

+ (void)makeLocationSearchRequestWithSearchTerm:(NSString *)searchTerm
                              limitToDepartable:(BOOL)limitToDepartable
                                   successBlock:(void (^)(NSArray *))successBlock
                                   failureBlock:(void (^)(NSError *))failureBlock
{
    searchTerm = [searchTerm stringByReplacingOccurrencesOfString:@" " withString:@"+"];
    
    NSDictionary *urlArgs = [NSDictionary dictionaryWithObject:searchTerm forKey:kSearchTermURLKey];
    
    [RHJSONRequest makeRequestWithPath:limitToDepartable ? kDepartableLocationsPath : kAllLocationsPath
                               urlArgs:urlArgs
                          successBlock:^(NSDictionary *jsonDict) {
                              
                              NSManagedObjectContext *managedObjectContext = [(RHAppDelegate *)[[UIApplication sharedApplication] delegate] managedObjectContext];
                              
                              NSArray *locations = [jsonDict objectForKey:kLocationsKey];
                              NSMutableArray *result = [NSMutableArray arrayWithCapacity:locations.count];
                              
                              for (NSDictionary *locationDict in locations) {
                                  NSFetchRequest *fetchRequest = [NSFetchRequest fetchRequestWithEntityName:kRHLocationEntityName];
                                  
                                  fetchRequest.predicate = [NSPredicate predicateWithFormat:@"serverIdentifier == %@", [locationDict objectForKey:kServerIdentifierKey]];
                                  
                                  NSArray *fetchResult = [managedObjectContext executeFetchRequest:fetchRequest error:nil];
                                  
                                  if (fetchResult.count > 0) {
                                      [result addObject:[fetchResult objectAtIndex:0]];
                                  }
                              }
     
                              successBlock(result);
                          }
                          failureBlock:^(NSError *error) {
                              failureBlock(error);
                          }];
}

@end
