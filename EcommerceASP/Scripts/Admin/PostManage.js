var PostManage = function () {
    $("#modal-update").off('hidden.bs.modal').on('hidden.bs.modal', function (e) {
        //Common.PostManage.SubmitForm();
    });
    $("#modal-update").off('shown.bs.modal').on('shown.bs.modal', function (e) {
        //Common.PostManage.ShowDialog();
    });
    return this.Init();
};

PostManage.prototype = {
    Init: function (options) {
        this.RegisterEvent();
    },
    RegisterEvent: function () {
        this.IsPaging = false;
        var that = this;
        var form = $("#form-search-PostManage");
        $("#table-list-PostManage > tbody > tr i.fa-edit").unbind("click").click(function () {
            var tr = $(this).closest("tr");
            Common.PostManage.ShowDialog(tr.data("id"));
        });
        $("#table-list-PostManage > tbody > tr i.fa-trash").unbind("click").click(function () {
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
                                url: PostManage.Url.Delete,
                                cache: false,
                                dataType: "json",
                                data: { id: tr.data("id") }
                            }, function (result) {
                                if (result.Status) {
                                    Common.HideAlert();
                                    Common.PostManage.SubmitForm();
                                } else {
                                    alert("Xóa không thành công!");
                                }
                            });
                        },
                        Value: "Tiếp tục"
                    },
                }
            }, "Continue");
        });

        $("#btn-update").unbind("click").click(function (e) {
            //set ckEditor value using jQuery
            $('#Content').val(CKEDITOR.instances["Content"].getData());
            Common.PostManage.SubmitFormUpdate(e);
        });
    },
    SubmitForm: function () {
        $("#form-search-PostManage").submit();
    },
    SetPage: function (page) {
        $("#form-search-PostManage").find("input[name='PageCurrent']").val(page);
    },
    Paging: function (page) {
        Common.PostManage.SetPage(page);
        Common.PostManage.IsPaging = true;
        Common.PostManage.SubmitForm();
    },
    SuccessForm: function () {
        Common.ShowLoading(false);
        Common.PostManage.RegisterEvent();
    },
    BeforeSend: function () {
        Common.ShowLoading(true);
        if (Common.PostManage.IsPaging) {
            Common.PostManage.SetPage(1);
        }
    },

    UpdateSuccess: function (res) {
        Common.ShowLoading(false);
        if (res.Status) {
            Common.PostManage.SubmitForm();
            Common.PostManage.HideDialog();
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
        location.reload();
        //$('#modal-update').on('shown.bs.modal', function (e) {
        //    $("#modal-update").modal("hide");
        //})

    },
    ShowDialog: function (id) {
        Common.Ajax({
            type: "POST",
            url: PostManage.Url.FormUpdate,
            cache: false,
            dataType: "html",
            data: { id: id }
        }, function (data) {
            $("#modal-update .modal-body").html(data);
            $("#modal-update .modal-dialog").css("max-width", "80%")
            //$("#modal-update").modal("show");

            //set ckEditor value using jQuery
            /*$('#Infomation').val(CKEDITOR.instances["Infomation"].getData());*/

            Common.PostManage.RegisterEvent();
        });
    },
};
