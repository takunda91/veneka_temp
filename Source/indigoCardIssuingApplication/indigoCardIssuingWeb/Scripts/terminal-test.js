
function TestPINPad() {
    var step = 0;
    var resp;

    var testpan = $("#txtTestPan").val(); 

    while (step < 3) {       

        switch (step) {
            case 0: resp = TestGetSessionKey(); break;
            case 1: resp = TestLoadParameters(testpan + "=123456789"); break;
            case 2: resp = TestDoPinSelection(testpan + "=123456789", "PINBLOCK"); break;
            default: alert("Step unknown " + step); return false;
        }         

        if (resp.Status !== 0) {
            alert("Failed on step " + step + " cause: " + resp.Exception);
            return false;
        }

        $("#responseDiv").val("Step " + step + ". Response " + resp.Status);
        step++;
    }

    alert("DONE!");
    return true;
}

function TestGetSessionKey() {
    var serial = $("#txtDeviceId").val();

    var obj = {};
    obj.deviceId = serial;

    var response = {};

    $.ajax({
        type: "POST",
        url: "/Terminal/TerminalActions.aspx/GetSessionKey",
        contentType: "application/json; charset=utf-8",
        data: JSON.stringify(obj),
        dataType: "json",
        async: false,
    }).done(function (result) {
        var parsedResult = result.d;
        response.Status = 0;
        response.PinSessionKey = parsedResult;
        response.TrackSessionKey = parsedResult;
    }).fail(function (data) {
        response.Status = 1;
        response.Exception = OnFailure(data);
    });

    return response;
}

function TestLoadParameters(encryptedTrack) {
    var trackObj = {};
    trackObj.deviceId = $("#txtDeviceId").val();
    trackObj.track2 = encryptedTrack;

    var response = {};

    $.ajax({
        type: "POST",
        url: "/Terminal/TerminalActions.aspx/LoadParameters",
        contentType: "application/json; charset=utf-8",
        data: JSON.stringify(trackObj),
        dataType: "json",
        async: false,
    }).done(function (result) {
        var parsedResult = result.d;
        response.Status = 0;
        response.Parm1 = parsedResult[1];
        response.Parm2 = parsedResult[2];
        response.Parm3 = parsedResult[3];
        response.Parm4 = parsedResult[4];
    }).fail(function (data) {
        response.Status = 1;
        response.Exception = OnFailure(data);
    });
    return response;
}

function TestDoPinSelection(encryptedTrack, encryptedPinBlock) {
    var trackObj = {};
    trackObj.deviceId = $("#txtDeviceId").val();
    trackObj.track2 = encryptedTrack;
    trackObj.pinBlock = encryptedPinBlock;

    var response = {};

    var readTrackReq = $.ajax({
        type: "POST",
        url: "/Terminal/TerminalActions.aspx/UpdatePin",
        contentType: "application/json; charset=utf-8",
        data: JSON.stringify(trackObj),
        dataType: "json",
        async: false
    }).done(function (data) {
        response.Status = 0;
    }).fail(function (data) {
        response.Status = 1;
        response.Exception = OnFailure(data);
    });

    return response;
}

//function OnFailure(xhr, textStatus, errorThrown) {
//    ex = jQuery.parseJSON(xhr.responseText);
//    rtnError = 'Unexpected error.';
//    if (xhr.status == 500) {
//        rtnError = ex.Message;
//    }

//    return rtnError;
//}

function TestClosePinApplet() {
    $(document).ready(function () {
        $('#dlgPinPad').puidialog('hide');
        __doPostBack('ClosePinApp', '');
    });
}