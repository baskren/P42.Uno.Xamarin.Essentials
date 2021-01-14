function UnoOrientation_IsAvailable(){
    return window.DeviceOrientationEvent;
}

function UnoOrientation_RequestPermission() {
    
    // IMPORTANT NOTE:
    // Requires the use of Feature-Policy or Permissions-Policy in the HTTP response header
    // https://developer.mozilla.org/en-US/docs/Web/HTTP/Headers/Feature-Policy
    // https://developer.mozilla.org/en-US/docs/Web/HTTP/Feature_Policy
    return new Promise((resolve, reject) => {
        /*
        if (navigator.permissions && navigator.permissions.query) {
            await Promise.all([navigator.permissions.query({ name: "accelerometer" }),
                                navigator.permissions.query({ name: "magnetometer" }),
                                navigator.permissions.query({ name: "gyroscope" })])
                .then(results => {
                    if (results.every(result => result.state === "granted")) {
                        resolve("Granted");
                    } else {
                        resolve("Denied");
                    }
                });
        }
        else {
            resolve("Disabled");
        }
        */
        resolve("not implemented");
    });
}

var UnoOrientation_Sensor = null;
let UnoOrientation_DataUpdate = Module.mono_bind_static_method("[P42.Uno.Xamarin.Essentials] Xamarin.Essentials.OrientationSensor:DataUpdated");

function UnoOrientation_Start(frequency) {
    const options = { frequency: frequency, referenceFrame: 'device' };
    UnoOrientation_Sensor = new AbsoluteOrientationSensor(options);
    UnoOrientation_Sensor.addEventListener('reading', () => {
        UnoOrientation_DataUpdate(UnoOrientation_Sensor.quaternion);
    });
    UnoOrientation_Sensor.addEventListener('error', error => {
        if (event.error.name == 'NotReadableError') {
            console.log("Sensor is not available.");
        }
    });
    UnoOrientation_Sensor.start();
}

function UnoOrientation_Stop() {
    UnoOrientation_Sensor.stop();
}



function UnoLegacyOrientation_RequestPermission() {
    if (DeviceMotionEvent && typeof DeviceMotionEvent.requestPermission === "function") {
        return DeviceMotionEvent.requestPermission();
    }
 }

let UnoLegacyOrientation_IsRunning = false;
var UnoLegacyOrientation_IntervalHandler = null;

function UnoLegacyOrientation_Start(period) {
    if (window.DeviceOrientationEvent && !UnoLegacyOrientation_IsRunning) {
        UnoLegacyOrientation_IsRunning = true;
        window.addEventListener("deviceorientationabsolute", UnoLegacyOrientation_UpodateOrientation);
        UnoLegacyOrientation_IntervalHandler = window.setInterval(UnoLegacyOrientation_PushOrientation, period);
    }
}

function UnoLegacyOrientation_Stop() {
    if (UnoLegacyOrientation_IsRunning) {
        UnoLegacyOrientation_IsRunning = false;
        window.clearInterval(UnoLegacyOrientation_IntervalHandler);
        window.removeEventListener("deviceorientationabsolute", UnoLegacyOrientation_UpodateOrientation);
    }
}

var UnoLegacyOrientation_Absolute = true;
var UnoLegacyOrientation_Alpha = 0.0;
var UnoLegacyOrientation_Beta = 0.0;
var UnoLegacyOrientation_Gamma = 0.0;

function UnoLegacyOrientation_UpodateOrientation(event) {
    UnoLegacyOrientation_Absolute = event.absolute;
    UnoLegacyOrientation_Alpha = event.alpha;
    UnoLegacyOrientation_Beta = event.beta;
    UnoLegacyOrientation_Gamma = event.gamma;
}


let UnoLegacyOrientation_DataUpdate = Module.mono_bind_static_method("[P42.Uno.Xamarin.Essentials] Xamarin.Essentials.OrientationSensor:LegacyDataUpdated");

function UnoLegacyOrientation_PushOrientation() {
    UnoLegacyOrientation_DataUpdate({
        z: UnoLegacyOrientation_Alpha,
        x: UnoLegacyOrientation_Beta,
        y: UnoLegacyOrientation_Gamma,
        absolute: UnoLegacyOrientation_Absolute
    });
}