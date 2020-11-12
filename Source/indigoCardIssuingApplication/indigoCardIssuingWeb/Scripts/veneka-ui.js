/* 
* Veneka IT Scripts
*/



function disableButtons() {
    $(".button").prop('disabled', true);    
}

function enableButtons() {
    $(".button").prop('disabled', false);
}

function timedRefresh(timeoutPeriod) {
    setTimeout("enableButtons();", timeoutPeriod);
}
function isNumberKeyWithPlus(evt) {
    var charCode = (evt.which) ? evt.which : event.keyCode
    if (charCode === 43) {
        return true;
    }
    if (charCode > 31 && (charCode < 48 || charCode > 57)) {
        return false;
    }

    return true;
}

function isNumberKeyWithhyphen(evt) {
    var charCode = (evt.which) ? evt.which : event.keyCode
    if (charCode === 45) {
        return true;
    }
    if (charCode > 31 && (charCode < 48 || charCode > 57)) {
        return false;
    }

    return true;
}
function isNumberKey(evt) {
    var charCode = (evt.which) ? evt.which : event.keyCode
    if (charCode > 31 && (charCode < 48 || charCode > 57)) {
        return false;
    }

    return true;
}

function isNumericValue(e,obj,intsize,deczize) {
    var keycode;

    if (window.event) keycode = window.event.keyCode;
    else if (e) { keycode = e.which; }
    else { return true; }

    var fieldval = (obj.value),
        splitfield = fieldval.split(".");

    if(keycode === 46) {
        return splitfield.length <= 1;
    }

    if (keycode > 31 && (keycode < 48 || keycode > 57)) {
        return false;
    }

    //if (splitfield.length >= 1 && splitfield[0].length <= intsize && splitfield[1].length <= decsize) {
    //    return true;
    //}
    //else if (splitfield.length == 0 && fieldval.length <= intsize) {
    //    return true;
    //}
    return true;
}

//Prevent the backspace key from taking the user back when a textbox is set to read only.
function preventBackspace(e) {
    var evt = e || window.event;
    if (evt) {
        var keyCode = evt.charCode || evt.keyCode;
        if (keyCode === 8) {
            if (evt.preventDefault) {
                evt.preventDefault();
            } else {
                evt.returnValue = false;
            }
        }
    }
}

function htmlOptionsToJson(htmlOptions) {
    var json = '[';

    for (i = 0; i < htmlOptions.length; i++) {
        json += '{"label":"' + htmlOptions[i].label + '","value":"' + htmlOptions[i].value + '"}';
        if (i < (htmlOptions.length - 1)) {
            json += ','
        }
    }

    json += ']'

    return json;
}


function checkTermStatus() {
    
    $(document).ready(function () {
        var hiddenSource = document.getElementById('hdnGuid');
        var guid = hiddenSource.value;

        $.ajax({
            type: "GET",
            url: "/NativeAPI.svc/rest/general/checkstatus/get/" + guid,
            contentType: "application/json; charset=utf-8",
            cache: false,
            dataType: "json",
            async: true
        }).done(function (result) {
            if (result === 0) {                
                checkTimer();
            }
            else if (result === 2 || result === 1) {
                
                ClosePinApplet();
            }
            

            // result -1 means the guid doesnt exist
            // result 0 means its in progress
            // result 1 means its failed
            // result 2 means were done
        })
        .fail(function () {
            console.log("API Call Failed");
        });
    });
}

function checkTimer() {
    clearTimeout(t);
    t = setTimeout(checkTermStatus, 5000);
}



function getPinPadApplet() {
    if(isIE(window.navigator.userAgent))
        return document.getElementById("pinpadapplet-object");
    else
        return document.getElementById("pinpadapplet-embed");
}

function isIE(userAgent) {
    userAgent = userAgent || navigator.userAgent;
    return userAgent.indexOf("MSIE ") > -1 || userAgent.indexOf("Trident/") > -1;
}

//function GetSessionKey() {
//    var serial = getPinPadApplet().GetSerial();

//    var obj = {};
//    obj.deviceId = serial;

//    var response = {};

//    $.ajax({
//        type: "POST",
//        url: "/Terminal/TerminalActions.aspx/GetSessionKey",
//        contentType: "application/json; charset=utf-8",
//        data: JSON.stringify(obj),
//        dataType: "json",
//        async: false,
//    }).done(function (result) {
//        var parsedResult = result.d;
//        response.Status = 0;
//        response.PinSessionKey = parsedResult;
//        response.TrackSessionKey = parsedResult;
//    }).fail(function (data) {
//        response.Status = 1;
//        response.Exception = OnFailure(data);        
//    });

//    return response;
//}

//function LoadParameters(encryptedTrack)
//{
//    var trackObj = {};
//    trackObj.deviceId = getPinPadApplet().GetSerial();
//    trackObj.track2 = encryptedTrack;

//    var response = {};

//    $.ajax({
//        type: "POST",
//        url: "/Terminal/TerminalActions.aspx/LoadParameters",
//        contentType: "application/json; charset=utf-8",
//        data: JSON.stringify(trackObj),
//        dataType: "json",
//        async: false,
//    }).done(function (result) {
//        var parsedResult = result.d;
//        response.Status = 0;
//        response.Parm1 = parsedResult[1];
//        response.Parm2 = parsedResult[2];
//        response.Parm3 = parsedResult[3];
//        response.Parm4 = parsedResult[4];
//    }).fail(function (data) {
//        response.Status = 1;
//        response.Exception = OnFailure(data);        
//    });
//    return response;
//}

//function DoPinSelection(encryptedTrack, encryptedPinBlock)
//{    
//    var serial = getPinPadApplet().GetSerial();

//    var trackObj = {};
//    trackObj.deviceId = serial;
//    trackObj.track2 = encryptedTrack;
//    trackObj.pinBlock = encryptedPinBlock;

//    var response = {};

//    var readTrackReq = $.ajax({
//        type: "POST",
//        url: "/Terminal/TerminalActions.aspx/UpdatePin",
//        contentType: "application/json; charset=utf-8",
//        data: JSON.stringify(trackObj),
//        dataType: "json",
//        async: false
//    }).done(function (data) {
//        response.Status = 0;
//    }).fail(function (data) {
//        response.Status = 1;
//        response.Exception = OnFailure(data);
//    });

//    return response;
//}

function OnFailure(xhr, textStatus, errorThrown) {
    ex = jQuery.parseJSON(xhr.responseText);
    rtnError = 'Unexpected error.';
    if (xhr.status === 500) {        
        rtnError = ex.Message;
    } 

    return rtnError;
}

function showPinApplet(token,operationtype) {
    $(document).ready(function () {
        checkTimer();
        window.location = "IndigoNativeApp://" +operationtype+"," + window.location.protocol + "," + window.location.hostname + "," + window.location.port + "," + token;
    });           
}

function ClosePinApplet() {
    $(document).ready(function () {        
        __doPostBack('ClosePinApp', '');
    });
}

    $(".ErrorPanel").children().change(function() {
        alert( "Handler for .change() called." );
    });

    (function ($, undefined) {

        $.widget("ui.combobox", {

            version: "@VERSION",

            widgetEventPrefix: "combobox",

            uiCombo: null,
            uiInput: null,
            _wasOpen: false,

            _create: function () {

                var self = this,
             select = this.element.hide(),
             input, wrapper;

                input = this.uiInput =
                  $("<input />")
                      .insertAfter(select)
                      .addClass("ui-widget ui-widget-content ui-corner-left ui-combobox-input")
                      .val(select.children(':selected').text())
                      .attr('tabindex', select.attr('tabindex'));

                wrapper = this.uiCombo =
            input.wrap('<span>')
               .parent()
               .addClass('ui-combobox')
               .insertAfter(select);

                input
          .autocomplete({

              delay: 0,
              minLength: 0,

              appendTo: wrapper,
              source: $.proxy(this, "_linkSelectList")

          });

                $("<button>")
            .attr("tabIndex", -1)
            .attr("type", "button")
            .insertAfter(input)
            .button({
                icons: {
                    primary: "ui-icon-triangle-1-s"
                },
                text: false
            })
            .removeClass("ui-corner-all")
            .addClass("ui-corner-right ui-button-icon ui-combobox-button");


                // Our items have HTML tags.  The default rendering uses text()
                // to set the content of the <a> tag.  We need html().
                input.data("ui-autocomplete")._renderItem = function (ul, item) {

                    return $("<li>")
                           .append($("<a>").html(item.label))
                           .appendTo(ul);

                };

                this._on(this._events);

            },


            _linkSelectList: function (request, response) {

                var matcher = new RegExp($.ui.autocomplete.escapeRegex(request.term), 'i');
                response(this.element.children('option').map(function () {
                    var text = $(this).text();

                    if (this.value && (!request.term || matcher.test(text))) {
                        var optionData = {
                            label: text,
                            value: text,
                            option: this
                        };
                        if (request.term) {
                            optionData.label = text.replace(
                           new RegExp(
                              "(?![^&;]+;)(?!<[^<>]*)(" +
                              $.ui.autocomplete.escapeRegex(request.term) +
                              ")(?![^<>]*>)(?![^&;]+;)", "gi"),
                              "<strong>$1</strong>");
                        }
                        return optionData;
                    }
                })
           );
            },

            _events: {

                "autocompletechange input": function (event, ui) {

                    var $el = $(event.currentTarget);
                    var changedOption = ui.item ? ui.item.option : null;
                    if (!ui.item) {

                        var matcher = new RegExp("^" + $.ui.autocomplete.escapeRegex($el.val()) + "$", "i"),
               valid = false,
               matchContains = null,
               iContains = 0,
               iSelectCtr = -1,
               iSelected = -1,
               optContains = null;
                        if (this.options.autofillsinglematch) {
                            matchContains = new RegExp($.ui.autocomplete.escapeRegex($el.val()), "i");
                        }


                        this.element.children("option").each(function () {
                            var t = $(this).text();
                            if (t.match(matcher)) {
                                this.selected = valid = true;
                                return false;
                            }
                            if (matchContains) {
                                // look for items containing the value
                                iSelectCtr++;
                                if (t.match(matchContains)) {
                                    iContains++;
                                    optContains = $(this);
                                    iSelected = iSelectCtr;
                                }
                            }
                        });

                        if (!valid) {
                            // autofill option:  if there is just one match, then select the matched option
                            if (iContains === 1) {
                                changedOption = optContains[0];
                                changedOption.selected = true;
                                var t2 = optContains.text();
                                $el.val(t2);
                                $el.data('ui-autocomplete').term = t2;
                                this.element.prop('selectedIndex', iSelected);
                                console.log("Found single match with '" + t2 + "'");
                            } else {

                                // remove invalid value, as it didn't match anything
                                $el.val('');

                                // Internally, term must change before another search is performed
                                // if the same search is performed again, the menu won't be shown
                                // because the value didn't actually change via a keyboard event
                                $el.data('ui-autocomplete').term = '';

                                this.element.prop('selectedIndex', -1);
                            }
                        }
                    }

                    this._trigger("change", event, {
                        item: changedOption
                    });

                },

                "autocompleteselect input": function (event, ui) {

                    ui.item.option.selected = true;
                    this._trigger("select", event, {
                        item: ui.item.option
                    });

                },

                "autocompleteopen input": function (event, ui) {

                    this.uiCombo.children('.ui-autocomplete')
               .outerWidth(this.uiCombo.outerWidth(true));
                },

                "mousedown .ui-combobox-button": function (event) {
                    this._wasOpen = this.uiInput.autocomplete("widget").is(":visible");
                },

                "click .ui-combobox-button": function (event) {

                    this.uiInput.focus();

                    // close if already visible
                    if (this._wasOpen)
                        return;

                    // pass empty string as value to search for, displaying all results
                    this.uiInput.autocomplete("search", "");

                }

            },

            value: function (newVal) {
                var select = this.element,
             valid = false,
             selected;

                if (!arguments.length) {
                    selected = select.children(":selected");
                    return selected.length > 0 ? selected.val() : null;
                }

                select.prop('selectedIndex', -1);
                select.children('option').each(function () {
                    if (this.value === newVal) {
                        this.selected = valid = true;
                        return false;
                    }
                });

                if (valid) {
                    this.uiInput.val(select.children(':selected').text());
                } else {
                    this.uiInput.val("");
                    this.element.prop('selectedIndex', -1);
                }

            },

            _destroy: function () {
                this.element.show();
                this.uiCombo.replaceWith(this.element);
            },

            widget: function () {
                return this.uiCombo;
            },

            _getCreateEventData: function () {

                return {
                    select: this.element,
                    combo: this.uiCombo,
                    input: this.uiInput
                };
            }

        });


    } (jQuery));
    
