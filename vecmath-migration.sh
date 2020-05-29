#!/bin/bash

#used with:
# git checkout -b vecmath
# git filter-branch --tree-filter ~/dev/scripts/vecmath-migration.sh --prune-empty vecmath

#to get rid of empty merge commits:
# git rebase --root HEAD 

# to graft repos:
# git replace --graft 2538c7a6f83b1611e3b4e2a225bc554f0e921557 vecmath_temp
#                     ^ oldest commit in the "new" history,    ^ newest commit in the "old" history

# safety check
if [ "$PWD" != "/home/ericwa/TrenchBroom/.git-rewrite/t" ]; then
    echo "only run this as a git filter in a TrenchBroom git checkout, it will delete files. pwd is $PWD"
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