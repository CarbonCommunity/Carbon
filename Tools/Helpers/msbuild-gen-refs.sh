#!/bin/bash

for FILE in $(find . -type f -name \*.dll | sort); do
cat <<-EOF
    <Reference Include="..\\..\\Rust\\RustDedicated_Data\\Managed\\$(basename ${FILE})">
      <PrivateAssets>all</PrivateAssets>
      <SpecificVersion>false</SpecificVersion>
    </Reference>
EOF
done
