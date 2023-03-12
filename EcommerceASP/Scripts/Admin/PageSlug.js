var PageSlug = function () {
    $("#modal-update").off('hidden.bs.modal').on('hidden.bs.modal', function (e) {
        //Common.PageSlug.SubmitForm();
    });
    $("#modal-update").off('shown.bs.modal').on('shown.bs.modal', function (e) {
        //Common.PageSlug.ShowDialog();
    });
    return this.Init();
};

PageSlug.prototype = {
    Init: function (options) {
        this.RegisterEvent();
    },
    RegisterEvent: function () {
        this.IsPaging = false;
        var that = this;
        var form = $("#form-search-PageSlug");
        $("#table-list-PageSlug > tbody > tr i.fa-edit").unbind("click").click(function () {
            var tr = $(this).closest("tr");
            Common.PageSlug.ShowDialog(tr.data("id"));
        });
        $("#table-list-PageSlug > tbody > tr i.fa-trash").unbind("click").click(function () {
            var tr = $(this).closest("tr");
            Common.ShowAlert("Thông báo", "Bạn có chắc chắn muốn xóa không?", {
                Close: {
                    Display: true,
                    OnClick: () => { Common.HideAlert(); }
                },
                Items: {
                    Continue: {
                        Name: "Continue",
                        OnClick: function (target) {
                            Common.Ajax({
                                type: "POST",
                                url: PageSlug.Url.Delete,
                                cache: false,
                                dataType: "json",
                                data: { id: tr.data("id") }
                            }, function (result) {
                                if (result == 1) {
                                    Common.HideAlert(function () {
                                        Common.PageSlug.SubmitForm();
                                    });
                                } else {
                                    Common.ShowAlert("Thông báo", "Xóa không thành công !");
                                }
                            });
                        },
                        Value: "Tiếp tục"
                    },
                }
            }, "Continue");
        });
        /*$("#table-list-PageSlug > tbody > tr img").zoomify();*/

        $("#btn-update").unbind("click").click(function () {
            Common.PageSlug.SubmitFormUpdate();
        });
    },
    SubmitForm: function () {
        $("#form-search-PageSlug").submit();
    },
    SetPage: function (page) {
        $("#form-search-PageSlug").find("input[name='PageCurrent']").val(page);
    },
    Paging: function (page) {
        Common.PageSlug.SetPage(page);
        Common.PageSlug.IsPaging = true;
        Common.PageSlug.SubmitForm();
    },
    SuccessForm: function () {
        Common.ShowLoading(false);
        Common.PageSlug.RegisterEvent();
    },
    BeforeSend: function () {
        Common.ShowLoading(true);
        if (Common.PageSlug.IsPaging) {
            Common.PageSlug.SetPage(1);
        }
    },

    UpdateSuccess: function (res) {
        Common.ShowLoading(false);
        if (res.Status) {
            Common.PageSlug.SubmitForm();
            Common.PageSlug.HideDialog();
            alert(res.Message);
            //Common.ShowAlert("Thông báo", res.Message, {
            //    Close: {
            //        Display: true,
            //        OnClick: () => { Common.HideAlert(); }
            //    },
            //});
        }
        else {
            alert(res.Message);
        }

    },
    UpdateBeforeSend: function () {
        Common.ShowLoading(true);
    },

    SubmitFormUpdate: function () {
        $("#form-update").submit();
    },
    HideDialog: function () {
        target = $("#modal-update");
        target.removeClass("in");
        $(".modal-backdrop").remove();
        target.hide();
    },
    ShowDialog: function (id) {
        Common.Ajax({
            type: "POST",
            url: PageSlug.Url.FormUpdate,
            cache: false,
            dataType: "html",
            data: { id: id }
        }, function (data) {
            $("#modal-update .modal-body").html(data);
            //$("#modal-update").modal("show");
            Common.PageSlug.RegisterEvent();
        });
    },

    OnChangeSelectPage: function (e) {
        Common.PageSlug.GetActionController($(e).val())
            .then(function (res) {
                $("#Action").val(res.Data.Action);
                $("#Controller").val(res.Data.Controller);
            });
    },
    GetActionController: function (id) {
        return new Promise(function (resolve, reject) {
            var option = "";
            if (!Common.Empty(id)) {
                Common.Ajax({
                    type: "POST",
                    url: PageSlug.Url.GetActionController,
                    cache: false,
                    dataType: "json",
                    data: { pageId: id }
                }, function (res) {
                    resolve(res);
                }, true);
            } else {
                resolve(res);
            }
        });
    },
};
