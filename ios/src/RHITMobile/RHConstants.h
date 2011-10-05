//
//  RHConstants.h
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

// Doxygen group definitions for the project

/// \defgroup map Map Functionality
/// Classes, categories, and protocols specific to displaying and annotating
/// MapKit MKMapViews. Most (if not all) of this code stands independent from
/// the rest of the application, only being referenced by the relevant view
/// controller (MapViewController).
/// \defgroup web Web/HTTP Functionality
/// Classes, categories, and protocols specific to sending to and receiving data
/// from the server. This modules includes abstractions for both the data
/// provider and subscriber delegate functionality.
/// \defgroup views Application-Level Views
/// The classes and view controllers that tie everything together and make this
/// an actual applicaction.
/// \defgroup model Model Layer
/// Model representation and data storage functionality. Model objects are
/// stored using Core Data, but are not made aware of their managed object
/// context, except at the class level for factory-style object creation.

// Doxygen main page documentation

/// \mainpage RHIT Mobile iOS Developer Documentation
///
/// \section intro_sec Introduction
///
/// Welcome to the iOS developer documetation for the RHIT Mobile project.
/// The purpose of this documentation is to provide an easily digestable
/// overview of how the project is structured, specifically for future teams
/// who may be assigned maintenance of this work, project advisors, or even
/// curious developers.
///
/// \section where_to_start Where To Start
///
/// For help getting a build environment set up, refer to README.md in the
/// top-level ios directory of the repository. This documentation assumes
/// that you have a working development environment for the project.
///
/// \section get_familiar_sec How To Get Familiar
///
/// Your best bet at getting familiar with the structure of this project is
/// to start at the modules page and explore the class structure from there.
/// To help you get started, here's a quick overview of the various modules:
/// - \ref model - Start here. This is where all of the domain-level objects
/// exist. These classes also include hooks into Core Data for persistance.
/// - \ref web - Functionality for fetching and sending objects to and from
/// the server. This might be a good next read.
/// - \ref map - Map-specific data. Don't worry about this stuff unless you
/// need to work with the map.
/// - \ref views - Once you're familiar with the various compontents of the
/// project, this module will show you how it's all tied together.
///
/// \section doc_maintenance_sec Maintaining This Documentation
///
/// For developers working on this project after its initial creation, please
/// document any code you add or change inline, using the existing files as
/// an example. For administrative-level documentation, including this index
/// page, make changes to RHConstants.h (this documentation won't be visible
/// in the file if you view it with the built-in Doxygen source viewer).

#ifndef RHITMobile_RHConstants_h
#define RHITMobile_RHConstants_h

#define RH_CAMPUS_CENTER_LATITUDE 39.483011
#define RH_CAMPUS_CENTER_LONGITUDE -87.325623

#define RH_INITIAL_ZOOM_LEVEL 15

#endif