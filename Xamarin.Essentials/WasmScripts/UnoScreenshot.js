require([`${config.uno_app_base}/html2canvas.min`], c => window.html2canvas = c);

function UnoScreenshot_GetUrlPromise() {
    return new Promise(function (resolve, reject) {
        var canvasResult = html2canvas(document.body);

        canvasResult.then(function (canvas) {

            let jepgPath = UnoScreenshot_CreateBase64Image(canvas, "image/jpeg");
            let pngPath = UnoScreenshot_CreateBase64Image(canvas, "image/png");
            resolve('success: true, Width: ' + canvas.width + ', Height: ' + canvas.height + ', PngPath: ' + pngPath + ", JpegPath: " + jepgPath);
        });

        canvasResult.catch(function (reason) {
            console.log('E.1');
            reject('success: false, reason: ' + reason);
        });

    });
}

function UnoScreenshot_CreateBase64Image(canvas, mimeType) {
    let urlImage = canvas.toDataURL(mimeType);
    let base64Image = urlImage.replace("data:" + mimeType + ";base64,", "");
    let suffix = mimeType.replace("image/", "");
    let path = '/tmp/' + uuidv4() + "." + suffix;
    FS.writeFile(path, base64Image);
    return path;
}

function uuidv4() {
    return 'xxxxxxxx-xxxx-4xxx-yxxx-xxxxxxxxxxxx'.replace(/[xy]/g, function (c) {
        var r = Math.random() * 16 | 0, v = c == 'x' ? r : (r & 0x3 | 0x8);
        return v.toString(16);
    });
}
