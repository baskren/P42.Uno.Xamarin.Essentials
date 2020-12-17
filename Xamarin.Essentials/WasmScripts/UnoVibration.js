function UnoVibration_IsSupported() {
    return navigator.vibrate || navigator.webkitVibrate || navigator.mozVibrate || navigator.msVibrate;
}

function UnoVibration_Vibrate(milliseconds) {
    navigator.vibrate(milliseconds);
}

function UnoVibration_Cancel() {
    navigator.vibrate(0);
}