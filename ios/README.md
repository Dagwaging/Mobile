# RHIT Mobile (iOS Edition)

This directory contains the iOS portion of the Rose-Hulman Mobile Campus
directory project. This project was built using **Xcode 4.3.2**, targeting **iOS 5.1**.
If future work is done on this project, it will probably be necessary to
update the structure and format of the project to newer target and IDE
versions. This is absolutely acceptable and encouraged.

## Dependencies

This portion of the project is built entirely using the iOS 4.3 standard
library, except for one additional dependency:
[TouchJSON](https://github.com/TouchCode/TouchJSON). TouchJSON is used to parse
and interpret the JSON data that the server component of this project provides,
because, as of iOS 4.3, there is no JSON handling present in the standard
library. If, at a future data, JSON functionality is added to the standard
library, it is encouraged that this project be updated to no longer have this
dependency.

TouchJSON builds from source as part of this project, and as such is included
as a submodule in this repository. If this project is built outright after a
clean clone of the repository, it will likely fail, being unable to find the
TouchJSON source files. This is because the submodule needs to be initialized.
To do this, run the following commands from the root of the repository (the
directory above this one):

    $ git submodule init
    $ git submodule update

## Building the Project

After satisfying the above dependency, building the project is as simple as
opening `RHITMobile.xcodeproj` under the `src` directory with Xcode and using
the IDE's `Build` and `Run` actions. This will allow the project to be built
for the iPhone Simulator. Building and running this project on a physical
device will require a developer to have an Apple Developer Account and to
attach his credentials to the project inside Xcode.

## Building the Documentation

Documentation specific to developers is included inline in the source code in
[Doxygen](http://www.stack.nl/~dimitri/doxygen/) format. To build a copy of
this documentation, first
[install Doxygen](http://www.stack.nl/~dimitri/doxygen/download.html#latestsrc),
then run the following command from the root of the repository (the directory
above this one):

    $ doxygen ios/Doxyfile

This will generate a set of HTML documentation under the `doc` directory within
this folder (full path will be `ios/doc`). Open the `index.html` file in this
new directory with your favorite web browser to get started with the developer
documentation. 

## Build Considerations

This portion of the project is designed to be able to be built by anyone with
an iOS development environment. This is absolutely the case. However, some
elements of the project have also been specifically set up to work well with an
automated build system or continuous integration server. These elements are
noted here for future developers to customize.

1. The "build number" of this project is always set to `DEVELOPMENT_BUILD`,
   with the intention of a build system replacing this string with a generated
   build number.

2. The Doxygen documentation is also marked as `DEVELOPMENT_BUILD`, with the
   intention of similar build system functionality being in place.
