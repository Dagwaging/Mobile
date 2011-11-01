#!/usr/bin/env bash

xcrun -sdk iphoneos PackageApplication "build/Release-iphoneos/RHITMobile.app" \
    -o "/Users/hudson/workspace/workspace/iOS/ios/RHITMobile.ipa" \
    --sign "iPhone Developer: Erik Hayes (UBJCB4G878)" \
    --embed "/Users/hudson/RHITMobileBeta.mobileprovision"

# Create the application manifest that will allow the app to be side-loaded
sed -i -e "s/BUILD_NUMBER/${BUILD_NUMBER}/g" app-manifest.plist.in
sed -i -e "s/VERSION/${VERSION}${BUILD_TYPE}${BUILD_NUMBER}/g" app-manifest.plist.in
sed -i -e "s/IPA_NAME/RHITMobile.ipa/g" app-manifest.plist.in
mv app-manifest.plist.in ../app-manifest.plist

# Create download page
sed -i -e "s/BUILD_NUMBER/${BUILD_NUMBER}/g" download.html.in
sed -i -e "s/VERSION/${VERSION}${BUILD_TYPE}${BUILD_NUMBER}/g" download.html.in
mv download.html.in ../download.html

# Build Doxygen documentation
cd ../../
sed -i -e "s/DEVELOPMENT_BUILD/v${VERSION}${BUILD_TYPE}${BUILD_NUMBER}/" ios/Doxyfile
/Applications/Doxygen.app/Contents/Resources/doxygen ios/Doxyfile

# Register build
VIEW_URL=`echo "from urllib import quote_plus; print quote_plus('$BUILD_URL')" | python`
DOWNLOAD_URL="itms-services%3A%2F%2F%3Faction%3Ddownload-manifest%26url%3D${VIEW_URL}artifact%2Fios%2Fapp-manifest.plist"
curl -d "platform=ios&buildNumber=${BUILD_NUMBER}&buildType=${BUILD_CLASSIFICATION}&publishingKey=413eb1c0e664012ed160123139181105&viewURL=${VIEW_URL}&downloadURL=${DOWNLOAD_URL}" http://rhitmobilebeta-test.heroku.com/build/publish
curl -d "platform=ios&buildNumber=${BUILD_NUMBER}&buildType=${BUILD_CLASSIFICATION}&publishingKey=ed2beb20e663012ef18512313b032c8e&viewURL=${VIEW_URL}&downloadURL=${DOWNLOAD_URL}" http://rhitmobilebeta.heroku.com/build/publish
