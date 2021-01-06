function UnoShare_ShareFiles(title, nodeFilePaths) {
    const extractFilename = (path) => {
        const pathArray = path.split("/");
        const lastIndex = pathArray.length - 1;
        return pathArray[lastIndex];
    };

    const files = [];
    for (i = 0; i < nodeFilePaths.length; i++) {
        var file = new File(FS.readFile(nodeFilePaths[i]), extractFilename(nodeFilePaths[i]));
    }

    var share = { files: files, title: title };

    return navigator.share(share);
}

function UnoShare_CanShareFiles(nodeFilePaths) {
    const extractFilename = (path) => {
        const pathArray = path.split("/");
        const lastIndex = pathArray.length - 1;
        return pathArray[lastIndex];
    };

    const files = [];
    for (i = 0; i < nodeFilePaths.length; i++) {
        var file = new File(FS.readFile(nodeFilePaths[i]), extractFilename(nodeFilePaths[i]));
    }

    var share = { files: files };

    var result = navigator.canShare(share);
    console.log('can share: ' + result);
    return result;
}