#!/usr/bin/env bash

# Determine which branch we're on
if [[ `git branch --contains $GIT_COMMIT` == *master* ]]
then
    BUILD_TYPE="beta"
    BUILD_CLASSIFICATION="official"
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

# Unlock the keychain
security list-keychains -s /Users/hudson/Library/Keychains/login.keychain
security unlock-keychain -p defaultpassword /Users/hudson/Library/Keychains/login.keychain

# Build the project
xcodebuild clean build
xcrun -sdk iphoneos PackageApplication "build/Release-iphoneos/RHITMobile.app" \
    -o "/Users/hudson/workspace/workspace/iOS/ios/RHITMobile.ipa" \
    --sign "iPhone Developer: Erik Hayes (UBJCB4G878)" \
    --embed "/Users/hudson/RHITMobileBeta.mobileprovision"

# Create the application manifest that will allow the app to be side-loaded
sed -i -e "s/BUILD_NUMBER/${BUILD_NUMBER}/g" app-manifest.plist.in
sed -i -e "s/VERSION/${VERSION}${BUILD_TYPE}${BUILD_NUMBER}/g" app-manifest.plist.in
sed -i -e "s/IPA_NAME/RHITMobile.ipa/g" app-manifest.plist.in
cp app-manifest.plist.in ../app-manifest.plist

# Create download page
sed -i -e "s/BUILD_NUMBER/${BUILD_NUMBER}/g" download.html.in
sed -i -e "s/VERSION/${VERSION}${BUILD_TYPE}${BUILD_NUMBER}/g" download.html.in
mv download.html.in ../download.html

# Create upgrade page
sed -i -e "s/BUILD_NUMBER/${BUILD_NUMBER}/g" upgrade.html.in
sed -i -e "s/VERSION/${VERSION}${BUILD_TYPE}${BUILD_NUMBER}/g" upgrade.html.in
mv upgrade.html.in ../upgrade.html

# Build Doxygen documentation
#cd ../../
#sed -i -e "s/DEVELOPMENT_BUILD/v${VERSION}${BUILD_TYPE}${BUILD_NUMBER}/" ios/Doxyfile
#/Applications/Doxygen.app/Contents/Resources/doxygen ios/Doxyfile

# Generate docs
/usr/local/bin/appledoc --project-name "Rose-Hulman Mobile" \
                        --project-company "Rose-Hulman Institute of Technology" \
	                    --company-id edu.rosehulman \
	                    --ignore .m \
                        --output ../doc \
	                    --keep-intermediate-files \
	                    --exit-threshold 2 \
	                    ./RHITMobile
