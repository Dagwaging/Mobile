#!/usr/bin/env bash

# Determine which branch we're working from
BRANCH=$(git status | awk '$3 ~ /branch/ {print $4}')

# Determine whether this is an alpha or beta build
if [ "$BRANCH" = "master" ]
then
    BUILD_TYPE="beta"
    BUILD_CLASSIFICATION="official"
else
    BUILD_TYPE="alpha"
    BUILD_CLASSIFICATION="rolling"
fi

# Bad awk and sed foo to extract the version number
VERSION=$(awk '$1 ~ /CFBundleShortVersion/ {version_next = "YES"}; \
          version_next ~ /YES/  && $1 ~ /string/ {print $1; \
          version_next = "NO"}' \
          src/RHITMobile/RHITMobile-Info.plist | \
          sed 's|<string>\(.*\)</string>|\1|g')

cd src

# Insert generated build number
sed -i -e "s/DEVELOPMENT_BUILD/${BUILD_TYPE}${BUILD_NUMBER}/" RHITMobile/RHITMobile-Info.plist

# Build the project
security list-keychains -s /Users/hudson/Library/Keychains/login.keychain
security unlock-keychain -p defaultpassword /Users/hudson/Library/Keychains/login.keychain
xcodebuild clean build
xcrun -sdk iphoneos PackageApplication "build/Release-iphoneos/RHITMobile.app" \
    -o "/Users/hudson/workspace/workspace/iOS/ios/RHITMobile.ipa" \
    --sign "iPhone Developer: Erik Hayes (UBJCB4G878)" \
    --embed "/Users/hudson/TestPhoneOnly.mobileprovision"

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
curl -d "platform=ios&buildNumber=${BUILD_NUMBER}&buildType=${BUILD_CLASSIFICATION}&publishingKey=ba3a8c60e57c012eda781231391ebf76&viewURL=${VIEW_URL}&downloadURL=${DOWNLOAD_URL}" http://rhitmobilebeta-test.heroku.com/build/publish
