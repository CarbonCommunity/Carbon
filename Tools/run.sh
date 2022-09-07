#!/bin/sh
# Doorstop running script
#
# This script is used to run a Unity game with Doorstop enabled.
# Running a game with Doorstop allows to execute arbitary .NET assemblies before Unity is initialized.
#
# Usage: Configure the script below and simply run this script when you want to run your game modded.

doorstop_libname="doorstop.so"
doorstop_dir=$PWD
case "$(uname -s)" in
    Linux*)  export LD_LIBRARY_PATH=${doorstop_dir}:${LD_LIBRARY_PATH};
             export LD_PRELOAD=$doorstop_libname;;
    Darwin*) export DYLD_LIBRARY_PATH=${doorstop_dir}:${DYLD_LIBRARY_PATH};
             export DYLD_INSERT_LIBRARIES=$doorstop_libname;;
    *) echo "Invalid OS"; exit;;
esac

# Configuration options (EDIT THESE):

# Whether or not to enable Doorstop. Valid values: TRUE or FALSE
export DOORSTOP_ENABLE=TRUE;

# What .NET assembly to execute. Valid value is a path to a .NET DLL that mono can execute.
export DOORSTOP_INVOKE_DLL_PATH=RustDedicated_Data/Managed/Carbon.Doorstop.dll;

# If enabled, this will prioritize assembly resolving from the given directory
# export DOORSTOP_CORLIB_OVERRIDE_PATH=""

# Specify the name of the game's executable here!
./RustDedicated