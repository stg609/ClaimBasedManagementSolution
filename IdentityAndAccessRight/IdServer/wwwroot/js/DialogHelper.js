var createConfirmationDialog = function (id, action, title, txtOkBtn, formContentFunction) {
    var html = '<form action="' + action + '" autocomplete="off" data-ajax-begin="onBegin" data-ajax-complete="onComplete"' +
        'data-ajax-failure="onFailed" data-ajax-success="onSuccess" data-ajax="true" data-ajax-method="POST" >' +
        '<div class="modal fade" id="' + id + '" role="dialog">' +
        '<div class="modal-dialog">' +
        '<div class="modal-content">' +
        '<div class="modal-header">' +
        '<button type="button" class="close" data-dismiss="modal">&times;</button>' +
        '<h4>' + title + '</h4>' +
        '</div>' +
        '<div class="modal-body"></div>' +
        '<div class="modal-footer">' +
        '<button type="submit" class="btn btn-primary">' + txtOkBtn + '</button> <button class="btn btn-default" data-dismiss="modal">Cancel</button>' +
        '</div>' +
        '</div>' +
        '</div>' +
        '</div>' +
        '</form>';
    var $modalForm = $(html);
    $(document).find("body").append($modalForm);
    $modalForm.find(".modal").on('show.bs.modal', formContentFunction);
}

var onBegin = function () {
    var dialog = $(this);
    dialog.find(".modal-dialog").prepend('<div class="overlay"><div class="container" style="height:100%;width:100%"><div class="row row-center" ><div class="col-xs-12"><i class="fa fa-spinner fa-spin" style="font-size: 40px;"></i></div></div></div></div>');
};

var onComplete = function () {
    var dialog = $(this);
    dialog.find(".modal-dialog").find(".overlay").remove();
    dialog.find(".modal").modal("toggle");
};

var onSuccess = function (context) {
    window.location.href = context;
};

var onFailed = function (context) {
    showAndDismissAlert("danger", context.responseText);
};

$(document).on('shown.bs.modal', '.modal', function () {
    $(this).find('[autofocus]').focus();
});