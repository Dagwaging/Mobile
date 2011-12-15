#!/usr/bin/env bash

# Determine which branch we're on
if [[ `git branch --contains $GIT_COMMIT` == *master* ]]
then
    BUILD_TYPE="beta"
    BUILD_CLASSIFICATION="official"
    sed -i -e "s/#define kRHBetaBuildType kRHBetaBuildTypeRolling/#define kRHBetaBuildType kRHBetaBuildTypeOfficial/" src/RHITMobile/RHBeta.h
else
    BUILD_TYPE="alpha"
    BUILD_CLASSIFICATION="rolling"
fi

echo "Build type: $BUILD_CLASSIFICATION"
echo "Build version type: $BUILD_TYPE"

# Find the existing version number
VERSION=$(defaults read `pwd`/src/RHITMobile/RHITMobile-Info CFBundleShortVersionString)
echo "Existing version number: $VERSION"

cd src

# Insert generated build number
sed -i -e "s/DEVELOPMENT_BUILD/${VERSION}${BUILD_TYPE}${BUILD_NUMBER}/" \
    RHITMobile/RHITMobile-Info.plist

# Set build number for beta screen
sed -i -e "s/#define kRHBetaBuildNumber -1/#define kRHBetaBuildNumber ${BUILD_NUMBER}/" \
    RHITMobile/RHBeta.h

# Unlock the keychain
security list-keychains -s /Users/hudson/Library/Keychains/login.keychain
security unlock-keychain -p defaultpassword /Users/hudson/Library/Keychains/login.keychain

# Build the project in beta mode
xcodebuild clean build
xcrun -sdk iphoneos PackageApplication "build/Release-iphoneos/RHITMobile.app" \
    -o "/Users/hudson/workspace/workspace/iOS/ios/RHITMobile-beta.ipa" \
    --sign "iPhone Developer: Erik Hayes (UBJCB4G878)" \
    --embed "/Users/hudson/RHITMobileBeta.mobileprovision"

# Build the project in clean mode
sed -i -e "s|#define RHITMobile_RHBeta|//#define RHITMobile_RHBeta|" RHITMobile/RHBeta.h
xcodebuild clean build
xcrun -sdk iphoneos PackageApplication "build/Release-iphoneos/RHITMobile.app" \
    -o "/Users/hudson/workspace/workspace/iOS/ios/RHITMobile-clean.ipa" \
    --sign "iPhone Developer: Erik Hayes (UBJCB4G878)" \
    --embed "/Users/hudson/RHITMobile.mobileprovision"


# Create the application manifest that will allow the app to be side-loaded
sed -i -e "s/BUILD_NUMBER/${BUILD_NUMBER}/g" app-manifest.plist.in
sed -i -e "s/VERSION/${VERSION}${BUILD_TYPE}${BUILD_NUMBER}/g" app-manifest.plist.in
sed -i -e "s/IPA_NAME/RHITMobile-beta.ipa/g" app-manifest.plist.in
cp app-manifest.plist.in ../app-manifest-beta.plist
sed -i -e "s/RHITMobile-beta/RHITMobile-clean.ipa/g" app-manifest.plist.in
cp app-manifest.plist.in ../app-manifest-clean.plist

# Create download page
sed -i -e "s/BUILD_NUMBER/${BUILD_NUMBER}/g" download.html.in
sed -i -e "s/VERSION/${VERSION}${BUILD_TYPE}${BUILD_NUMBER}/g" download.html.in
mv download.html.in ../download.html

# Create upgrade page
sed -i -e "s/BUILD_NUMBER/${BUILD_NUMBER}/g" upgrade.html.in
sed -i -e "s/VERSION/${VERSION}${BUILD_TYPE}${BUILD_NUMBER}/g" upgrade.html.in
mv upgrade.html.in ../upgrade.html

# Build Doxygen documentation
cd ../../
sed -i -e "s/DEVELOPMENT_BUILD/v${VERSION}${BUILD_TYPE}${BUILD_NUMBER}/" ios/Doxyfile
/Applications/Doxygen.app/Contents/Resources/doxygen ios/Doxyfile

# Register build
VIEW_URL=`echo "from urllib import quote_plus; print quote_plus('$BUILD_URL')" | python`
#DOWNLOAD_URL="itms-services%3A%2F%2F%3Faction%3Ddownload-manifest%26url%3D${VIEW_URL}artifact%2Fios%2Fapp-manifest-beta.plist"
DOWNLOAD_URL="http://mobile.csse.rose-hulman.edu/hudson/job/iOS/$BUILD_NUMBER/artifact/ios/upgrade.html"
curl -d "platform=ios&buildNumber=${BUILD_NUMBER}&buildType=${BUILD_CLASSIFICATION}&publishingKey=413eb1c0e664012ed160123139181105&viewURL=${VIEW_URL}&downloadURL=${DOWNLOAD_URL}" http://rhitmobilebeta-test.heroku.com/build/publish
curl -d "platform=ios&buildNumber=${BUILD_NUMBER}&buildType=${BUILD_CLASSIFICATION}&publishingKey=ed2beb20e663012ef18512313b032c8e&viewURL=${VIEW_URL}&downloadURL=${DOWNLOAD_URL}" http://rhitmobilebeta.heroku.com/build/publish
