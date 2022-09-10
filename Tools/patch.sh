#!/bin/sh

chmod +x ./carbon/tools/NStrip

# Patch Rust's assemblies
./carbon/tools/NStrip -p -cg --keep-resources -n --unity-non-serialized ./RustDedicated_Data/Managed/Assembly-CSharp.dll ./RustDedicated_Data/Managed/Assembly-CSharp.dll