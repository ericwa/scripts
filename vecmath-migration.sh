#!/bin/bash

d=$(basename "$PWD")

if [ "$d" != "TrenchBroom" ]; then
    echo "only run this as a git filter in a TrenchBroom git checkout, it will delete files"
    exit
fi

rm -fr /tmp/tbmigrate
mkdir /tmp/tbmigrate

# set up the desired folder hiereachy
mkdir /tmp/tbmigrate/test
mkdir /tmp/tbmigrate/test/src
mkdir /tmp/tbmigrate/include
mkdir /tmp/tbmigrate/include/vecmath

# copy stuff from pwd to /tmp/tbmigrate
cp LICENSE /tmp/tbmigrate

#old hiereachy
cp test/src/*_test.cpp /tmp/tbmigrate/test/src
cp vecmath/include/vecmath/*.h /tmp/tbmigrate/include/vecmath

#new hierarchy
cp lib/vecmath/test/src/*.cpp /tmp/tbmigrate/test/src
cp lib/vecmath/include/vecmath/*.h /tmp/tbmigrate/include/vecmath

# erase everything current dir and copy /tmp/tbmigrate over us
find . -maxdepth 1 ! -name '.git' -exec rm -r {} \;

# copy everything back
GLOBIGNORE=".:.."
cp -r /tmp/tbmigrate/* .

rm -fr /tmp/tbmigrate