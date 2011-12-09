//Main page documetation for automatic doxygen generation.


// Doxygen group definitions for the project

/// \defgroup util Utility Classes
/// The classes in the server used for administrative or utilitary purposes
/// 
/// \defgroup requests Request Parsers
/// The classes in the server used for listening for and parsing apart incoming requests
/// 
/// \defgroup objects Response Handlers
/// The classes in the server used for responding to requests


// Doxygen main page documentation

/// \mainpage RHIT Mobile Server Developer Documentation
///
/// \section intro_sec Introduction
///
/// Welcome to the server developer documetation for the RHIT Mobile project.
/// The purpose of this documentation is to provide an easily digestable
/// overview of how the project is structured, specifically for future teams
/// who may be assigned maintenance of this work, project advisors, or even
/// curious developers.
///
/// \section where_to_start Where To Start
///
/// For help getting a build environment set up, refer to README.md that can 
/// be found <a href="https://github.com/RHIT/Mobile/tree/master/server">here</a>
/// , the top-level server directory of the repository. This documentation assumes
/// that you have a working development environment for the project.
///
/// \section overview Architecture Overview
/// 
/// - The Program class is the system's initializer, that sets up the Hash objects and starts the HTTP listener.
/// - The ThreadManager class handles all asynchronous calls throughout the system.  It manages these calls so that only a single thread is running for actual computation.  Any call to an asynchronous operation (waiting for a HTTP request, querying the database, etc.) or any call to a method that is asynchronous must go through the ThreadManager to ensure that single-thread operations.
/// - The WebController class listens for incoming requests, and spins off a new thread to handle the request.  The request is sent through several PathHandler objects to parse the request.
/// - The JsonObjects file contains the objects to be used as responses.  The final result is either a serialized JSON object, or an HTTP response code.
/// 
/// 
/// \section doc_maintenance_sec Maintaining This Documentation
///
/// For developers working on this project after its initial creation, please
/// document any code you add or change inline, using the existing files as
/// an example. For administrative-level documentation, including this index
/// page, make changes to DocContent.cs (this documentation won't be visible
/// in the file if you view it with the built-in Doxygen source viewer).