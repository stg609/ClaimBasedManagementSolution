var cbms_dialog_onbegin = function () {
    var dialog = $(this);
    dialog.find(".modal-dialog").prepend('<div class="overlay"><div class="container" style="height:100%;width:100%"><div class="row row-center" ><div class="col-xs-12"><i class="fa fa-spinner fa-spin" style="font-size: 40px;"></i></div></div></div></div>');
};
var cbms_dialog_onComplete = function () {
    var dialog = $(this);
    dialog.find(".modal-dialog").find(".overlay").remove();
    dialog.find(".modal").modal("toggle");
};

var cbms_dialog_onSuccess = function (context) {
    window.location.href = context;
};

var cbms_dialog_onFailed = function (context) {
    showAndDismissAlert("danger", context.responseText);
};

(function (JQ) {
    var spinnerHtml = "<center><i class='fa fa-spinner fa-spin' style='font-size: 24px'></i></center>";
    var showLoading = function () {
        var modal = $(this);
        var form = modal.closest("form");

        form.find(".modal-body").html(spinnerHtml);
    };

    $(document).on('shown.bs.modal', '.modal', function () {
        $(this).find('[autofocus]').focus();
    });

    JQ.extend({
        'createConfirmationDialog': function (id, action, title, txtOkBtn, requestAntiForgeryToken, formContentFunction) {
            var html = '<form action="' + action + '" autocomplete="off" data-ajax-begin="cbms_dialog_onbegin" data-ajax-complete="cbms_dialog_onComplete"' +
                'data-ajax-failure="cbms_dialog_onFailed" data-ajax-success="cbms_dialog_onSuccess" data-ajax="true" data-ajax-method="POST" >' +
                '<div class="modal fade" id="' + id + '" role="dialog">' +
                '<div class="modal-dialog">' +
                '<div class="modal-content">' +
                '<div class="modal-header">' +
                '<button type="button" class="close" data-dismiss="modal">&times;</button>' +
                '<h4>' + title + '</h4>' +
                '</div>' +
                '<div class="modal-body"></div>' +
                '<div class="modal-footer">' +
                '<input type="hidden" name="__RequestVerificationToken" value="' + requestAntiForgeryToken + '" />' +
                '<button type="submit" class="btn btn-primary">' + txtOkBtn + '</button> <button class="btn btn-default" data-dismiss="modal">Cancel</button>' +
                '</div>' +
                '</div>' +
                '</div>' +
                '</div>' +
                '</form>';
            var $modalForm = $(html);
            $(document).find("body").append($modalForm);
            $modalForm.find(".modal").on('show.bs.modal', showLoading);
            $modalForm.find(".modal").on('show.bs.modal', formContentFunction);
        }
    })
})(jQuery)