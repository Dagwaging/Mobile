//
//  RHSchedule.h
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


/**
 * A wrapper class for RHCourseMeeting objects that assigns each to a specific
 * period. One RHScheduleEntry contains one RHCourseMeeting, mapped to a specific
 * class period As a result, RHCourseMeetings spanning multiple periods will belong
 * to multiple RHScheduleEntry objects.
 */
@class RHCourseMeeting;

@interface RHScheduleEntry : NSObject

/** @name Initializing */

/**
 * Designated initializer. Allows setting of both the entry's class period and
 * wrapped course meeting during initialization.
 *
 * @param period The period for this entry, as defined in the matching property.
 *
 * @param meeting The RHCourseMeeting wrapped by this entry.
 */
- (id)initWithPeriod:(NSNumber *)period meeting:(RHCourseMeeting *)meeting;

/** @name Assigned Data */

/**
 * The class period (0 through 9) that this schedule entry is for.
 */
@property (nonatomic, strong) NSNumber *period;

/** @name Wrapped Data */

/**
 * The course meeting wrapped by this schedule entry. Though this course meeting may
 * span multiple class periods, this schedule entry specifically maps it to one. This value
 * may be `nil`, in which case this entry is serving as a placeholder for a period
 * with no course meetings.
 */
@property (nonatomic, strong) RHCourseMeeting *courseMeeting;

@end


/**
 * A wrapper class for a set of (or even just one) RHCourse object that
 * exposes aggregate information for displaying the data contained within
 * in a list of time slots. A schedule can represent a student's classes for
 * a specific term, a course section's meeting times, or a room's occupancy.
 */
@interface RHSchedule : NSObject

/** @name Initializing */

/**
 * Designated initializer. Allows the schedule's contained courses to be set
 * during initialization.
 *
 * @param courses The array of RHCourse objects that this schedule will wrap.
 */
- (id)initWithCourses:(NSArray *)courses;

/** @name Wrapped Data */

/**
 * An array of RHCourse objects (each populated with its respective
 * RHCourseMeeting objects) to be grouped together as this schedule.
 */
@property (nonatomic, strong) NSArray *courses;

/** @name Accessing Schedule Data */

/**
 * Retrieve the list (ordered by time in forward motion) of RHScheduleEntry objects
 * for a particular day.
 *
 * @param dayIndex The zero-based index of the day being requested, starting with
 *        "Monday" at 0 and ending with "Saturday" at 5.
 *
 * @return An NSArray of the RHScheduleEntry objects found for the given day. If
 *         no entries are found, the array will be empty, but not `nil`. It is also
 *         important to note that if there is overlap in the schedule's contained courses,
 *         multiple schedule entries for the same period may occur.
 */
- (NSArray *)scheduleEntriesForDayIndex:(NSNumber *)dayIndex;

@end
