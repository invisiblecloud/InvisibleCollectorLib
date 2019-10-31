# helper script for building docs with bash
set -e

if [ "$#" = "0" ]; then
    echo -e "${RED}Re-run with:${NC}
    install - download docfx binary
    build - build html docs (needs 'mono' to be installed)
    clean - clean cache and /docs folder"
    exit 1
fi

DOCFX_VERSION="v2.46"

option=$1
if [ "$option" = "install" ]; then
    curl -L https://github.com/dotnet/docfx/releases/download/${DOCFX_VERSION}/docfx.zip > docfx.zip
    unzip ./docfx.zip -d docfx
elif [ "$option" = "build" ]; then
    mono docfx/docfx.exe docfx.json
elif [ "$option" = "serve" ]; then
    mono docfx/docfx.exe docfx.json --serve
elif [ "$option" = "clean" ]; then
    rm -rf ./obj ../docs
fi