#!/usr/bin/env bash
# Usage (Git Bash): ./build.sh
# Creates the mod zip in build/. Only reads files from the game installation.
set -euo pipefail

# Always run from this script's folder.
cd "$(dirname "$0")"

# Tools and dependencies. Set VS_GAME to use a different game installation.
game_folder="${VS_GAME:-$(cygpath -u "$APPDATA")/Vintagestory}"
compiler="/c/Program Files/Microsoft Visual Studio/2022/Community/MSBuild/Current/Bin/Roslyn/csc.exe"
zip_tool="/c/Windows/System32/tar.exe"

# Pick the newest installed .NET 10 runtime.
runtime_folder=$(
  ls -d "/c/Program Files/dotnet/shared/Microsoft.NETCore.App/10."* |
    sort -V |
    tail -1
)

# Read the mod version for the zip filename.
version=$(sed -n 's/.*"version": *"\([^"]*\)".*/\1/p' modinfo.json)
zip_name="healerscraft_${version}.zip"

# 1. Clear old output and prepare the files that will go into the zip.
rm -rf build
mkdir -p build/stage

# 2. List the DLLs the C# compiler needs, one reference per line.
# The compiler expects Windows paths, with quotes to allow spaces.
write_reference() {
  local dll_path="$1"
  printf -- '-r:"%s"\n' "$(cygpath -w "$dll_path")"
}

{
  for dll in "$runtime_folder"/System*.dll "$runtime_folder"/netstandard.dll "$runtime_folder"/mscorlib.dll; do
    # Native DLLs cannot be used as C# references.
    if [[ "$dll" == *Native* ]]; then
      continue
    fi
    write_reference "$dll"
  done

  game_libraries=(
    "VintagestoryAPI.dll"
    "Lib/Newtonsoft.Json.dll"
    "Lib/protobuf-net.dll"
  )
  for library in "${game_libraries[@]}"; do
    if [[ -f "$game_folder/$library" ]]; then
      write_reference "$game_folder/$library"
    fi
  done
} > build/refs.rsp

# 3. Compile the mod. Keep the options on separate lines for readability.
compiler_options=(
  -nologo
  -noconfig
  -nostdlib
  -target:library
  -optimize+
  -debug:portable
  -langversion:latest
  -nowarn:1701,1702
  -out:build/stage/HealersCraft.dll
  @build/refs.rsp
)

if ! "$compiler" "${compiler_options[@]}" src/*.cs; then
  echo
  echo "BUILD FAILED: fix the C# errors above (file(line,column): error ...). No zip was made."
  exit 1
fi

# 4. Add the mod metadata, assets, and optional icon.
cp modinfo.json build/stage/
cp -r assets build/stage/
if [[ -f modicon.png ]]; then
  cp modicon.png build/stage/
fi

# 5. Zip the staged files. A subshell keeps our working folder unchanged.
(
  cd build/stage
  "$zip_tool" -a -cf "../$zip_name" *
)
echo "Built build/$zip_name"
