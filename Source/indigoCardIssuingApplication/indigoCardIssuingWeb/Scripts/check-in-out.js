/* Get the screen ready */
$(document).ready(function () {
    var $PaginationPanel = undefined;
    var $hasChanges = undefined;
    var $sourceCaption = undefined;
    var $targetCaption = undefined;

    $hasChanges = $('#hdnHasChanges');

    if ($hasChanges.val() === "true") {
        $('#btnUpdate').show();
    }
    else {
        $('#btnUpdate').hide();
    }

    $sourceCaption = $('#hdnsourcecaption');
    $targetCaption = $('#hdntargetcaption');

    $('#cardCheckInOut').puipicklist({
        sourceCaption: $sourceCaption.val(),
        targetCaption: $targetCaption.val(),
        sourceData: CheckedInCards,
        targetData: CheckedOutCards,
        filter: true,
        filterMatchMode: 'contains',
        effectSpeed: 1,
        transfer: function (event, ui) {
            $hasChanges.val(true);
            $("#btnUpdate").show();
            $("#btnCheckedInReport").hide();
            $("#btnCheckedOutReport").hide();
        }
    });

    //$("#btnConfirm").click(function () {
    //    var hiddenSource = $('#sourceSel');
    //    hiddenSource.value = htmlOptionsToJson($('#source option'))

    //    var hiddenTarget = $('#targetSel');
    //    hiddenTarget.value = htmlOptionsToJson($('#target option'))
    //});

    $('#ddlIssuer').puidropdown({
        effectSpeed: 1,
        change: function () {
            $("#btnUpdate").hide();
            __doPostBack('ddlIssuer', '')
        }
    });

    $('#ddlBranch').puidropdown({
        effectSpeed: 1,
        change: function () {
            $("#btnUpdate").hide();
            __doPostBack('ddlBranch', '')
        }
    });

    $('#ddlProduct').puidropdown({
        effectSpeed: 1,
        change: function () {
            $("#btnUpdate").hide();
            __doPostBack('ddlProduct', '')
        }
    });

    $('#ddlOperator').puidropdown({
        effectSpeed: 1,
        change: function () {
            $("#btnUpdate").hide();
            __doPostBack('ddlOperator', '')
        }
    });

    $('input[type="text"]').puiinputtext();
  
    $('#tblSummary').puidatatable({
        caption: 'Local Datasource',
        paginator: {
            rows: 25
        },
        columns: [
                { field: 'card_id', headerText: 'card_id', sortable: true },
                { field: 'card_number', headerText: 'card_number', sortable: true },
                { field: 'branch_card_statuses_name', headerText: 'branch_card_statuses_name', sortable: true },
                { field: 'branch_card_statuses_id', headerText: 'branch_card_statuses_id', sortable: true }
        ],
        datasource: summaryResults,
        selectionMode: 'single'
    });

    
});


/*Functions for the screen*/
function EditScreen() {
    $("#btnConfirm").hide();
    $("#btnCancel").hide();

    $("#btnUpdate").show();
    $("#pnlPage").show();
    $("#btnLoadCards").show();

    $('#ddlIssuer').puidropdown('enable');
    $('#ddlBranch').puidropdown('enable');
    $('#ddlProduct').puidropdown('enable');
    $('#ddlOperator').puidropdown('enable');
    $('input[type="text"]').puiinputtext('enable');

    $('#cardCheckInOut').puipicklist('enable');
    $('#lblInfoMessage').html("");

    var $hasChanges = undefined;
    $hasChanges = $('#hdnHasChanges');    
    $hasChanges.val(true);

    return false;
}

function ConfirmScreen() {

    $("#btnConfirm").show();
    $("#btnCancel").show();

    $("#btnUpdate").hide();
    $("#btnLoadCards").hide();
    $("#pnlPage").hide();
    $("#btnCheckedInReport").hide();
    $("#btnCheckedOutReport").hide();

    $('#ddlIssuer').puidropdown('disable');
    $('#ddlBranch').puidropdown('disable');
    $('#ddlProduct').puidropdown('disable');
    $('#ddlOperator').puidropdown('disable');
    $('input[type="text"]').puiinputtext('disable');

    $('#cardCheckInOut').puipicklist('disable');

    var confirmMsg = $('#hdnConfirmMsg');
    $('#lblInfoMessage').html(confirmMsg.val());

    var $hasChanges = undefined;
    $hasChanges = $('#hdnHasChanges');
    $hasChanges.val(false);

    return false;
}


function saveCheckedOutCards(button) {
    var $hiddenSource = undefined;
    $hiddenSource = $('#sourceSel');
    $hiddenSource.val(htmlOptionsToJson($('#source option')));

    var $hiddenTarget = undefined;
    $hiddenTarget = $('#targetSel');
    $hiddenTarget.val(htmlOptionsToJson($('#target option')));

    return true;
}


function htmlOptionsToJson(htmlOptions) {
    var json = '';
    var log = '';

    htmlOptions.each(function (i) {        
        log += this + ":" + $(this).val() + ',';
        json += '{"label":"' + $(this).text() + '","value":"' + $(this).val() + '"},';
    });

    var $logging = undefined;
    $logging = $('#hdnlog');
    $logging.val(log + " --> " + json);
    

    return '[' + json.slice(0, -1) + ']';
}