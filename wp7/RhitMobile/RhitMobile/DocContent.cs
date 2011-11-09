//Main page documetation for automatic doxygen generation.


// Doxygen group definitions for the project

/// \defgroup pages WP7 Pages
/// All of the page that can be navigated to within the app.
/// 
/// \defgroup services Major Services
/// This includes static and singleton classes used by many other classes to
/// gather/store information.
/// 
/// \defgroup objects Object Models
/// Various object models.
/// 
/// \defgroup tile_sources Tile Sources
/// Map tile source classes. This includes cutom overlays.


// Doxygen main page documentation

/// \mainpage RHIT Mobile WP7 Developer Documentation
///
/// \section intro_sec Introduction
///
/// Welcome to the WP7 developer documetation for the RHIT Mobile project.
/// The purpose of this documentation is to provide an easily digestable
/// overview of how the project is structured, specifically for future teams
/// who may be assigned maintenance of this work, project advisors, or even
/// curious developers.
///
/// \section where_to_start Where To Start
///
/// For help getting a build environment set up, refer to README.md that can 
/// be found <a href="https://github.com/RHIT/Mobile/tree/master/wp7">here</a>
/// , the top-level wp7 directory of the repository. This documentation assumes
/// that you have a working development environment for the project.
///
/// \section overview Architecture Overview
/// 
/// Each view of the app has both a xaml and C# file associated with it.
/// All of these files can be found in the following group: \ref pages.
/// Very little information is stored in the actual view itself.
/// They each utilize one of the classes found in the \ref services group.
/// \ref objects conatins all of the object models.
/// \ref tile_source contains all of the available map tile sources and overlays.
/// 
/// - The views handle page navigation and user events.
/// - Anytime one of the views need the map, manipulate it, or get information from it they will use the \ref RhitMap singleton class.
/// This way the map stays up-to-date at all times.
///  - RhitMap.cs keeps track of the current properties of the map as well as the possible tile sources avaible.
/// - If any of the views need any other information, like location data for instance, they will use the DataCollector singleton class.
///  - The DataCollector will try to get the requested data from the DataStorage singleton class (i.e. local storage) first.
///  If DataStorage has the data, then it is returned. Otherwise, it uses RequestBuilder and GeoService to make a request to the server.
///  - DataCollector will also check for updates from the server on the app's initialization.
/// - All views listen to at least one ServiceEvent handler to handle updated data.
///  - All server responses are handled by ResponseHandler, parsed, and made into a ServiceEvent.
///  - Data from the server responses are parsed uses the classes in DataContracts, which are then usually converted into RhitLocation objects.
/// 
/// \section doc_maintenance_sec Maintaining This Documentation
///
/// For developers working on this project after its initial creation, please
/// document any code you add or change inline, using the existing files as
/// an example. For administrative-level documentation, including this index
/// page, make changes to DocContent.cs (this documentation won't be visible
/// in the file if you view it with the built-in Doxygen source viewer).