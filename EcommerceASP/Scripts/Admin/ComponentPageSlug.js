var ComponentPageSlug = function () {
    $("#modal-update").off('hidden.bs.modal').on('hidden.bs.modal', function (e) {
        //Common.ComponentPageSlug.SubmitForm();
    });
    $("#modal-update").off('shown.bs.modal').on('shown.bs.modal', function (e) {
        //Common.ComponentPageSlug.ShowDialog();
    });
    return this.Init();
};

ComponentPageSlug.prototype = {
    Init: function (options) {
        this.RegisterEvent();
    },
    RegisterEvent: function () {
        this.IsPaging = false;
        var that = this;
        var form = $("#form-search-ComponentPageSlug");
        $("#table-list-ComponentPageSlug > tbody > tr i.fa-edit").unbind("click").click(function () {
            var tr = $(this).closest("tr");
            Common.ComponentPageSlug.ShowDialog(tr.data("id"));
        });
        $("#table-list-ComponentPageSlug > tbody > tr i.fa-trash").unbind("click").click(function () {
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
                                url: ComponentPageSlug.Url.Delete,
                                cache: false,
                                dataType: "json",
                                data: { id: tr.data("id") }
                            }, function (result) {
                                if (result.Status) {
                                    Common.HideAlert(function () {
                                        Common.ComponentPageSlug.SubmitForm();
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

        $("#btn-update").unbind("click").click(function (e) {
            Common.ComponentPageSlug.SubmitFormUpdate(e);
        });

        $("#btn-updateSubDescription").unbind("click").click(function (e) {
            $('#Description').val(CKEDITOR.instances["Content"].getData());
            Common.ComponentPageSlug.SubmitFormUpdateSubDescription(e);
        });

        $("#table-list-ComponentPageSlug > tbody > tr img").zoomify();
        $("#imgShow").zoomify();
        $("#file-upload-image").unbind("change").change(function (e) {
            $("#Image").val(this.files[0].name);
            this.files.item(0).type;
            if (window.FileReader) {
                var reader = new window.FileReader();
                reader.onload = function (e) {

                    $("#imgShow").attr('src', e.target.result);
                };
                reader.readAsDataURL(this.files[0]);
            } else {
                return;
            }

        });
        
        var form = $("#form-update");

        form.unbind("submit").submit(function (e) {
            e.preventDefault();
            e.stopImmediatePropagation();
            var xhr = new XMLHttpRequest();
            xhr.responseType = "json";
            xhr.open(form[0].method, form[0].action);
            xhr.onreadystatechange = function () {
                if (xhr.readyState == 4 && xhr.status == 200) {
                    Common.ComponentPageSlug.UpdateSuccess(xhr.response);
                }
            };
            Common.ComponentPageSlug.UpdateBeforeSend();
            xhr.send((new FormData(form[0])));
        })


    },

    SubmitForm: function () {
        $("#form-search-ComponentPageSlug").submit();
    },

    SetPage: function (page) {
        $("#form-search-ComponentPageSlug").find("input[name='PageCurrent']").val(page);
    },

    Paging: function (page) {
        Common.ComponentPageSlug.SetPage(page);
        Common.ComponentPageSlug.IsPaging = true;
        Common.ComponentPageSlug.SubmitForm();
    },

    SuccessForm: function () {
        Common.ShowLoading(false);
        Common.ComponentPageSlug.RegisterEvent();
    },

    BeforeSend: function () {
        Common.ShowLoading(true);
        if (Common.ComponentPageSlug.IsPaging) {
            Common.ComponentPageSlug.SetPage(1);
        }
    },

    UpdateSuccess: function (res) {
        Common.ShowLoading(false);
        if (res.Status) {
            Common.ComponentPageSlug.SubmitForm();
            Common.ComponentPageSlug.HideDialog();
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
        //$(window).scrollTop(0);
        location.reload();
        //$('#modal-update').on('shown.bs.modal', function (e) {
        //    $("#modal-update").modal("hide");
        //})

    },

    ShowDialog: function (id) {
        Common.Ajax({
            type: "POST",
            url: ComponentPageSlug.Url.FormUpdate,
            cache: false,
            dataType: "html",
            data: { id: id }
        }, function (data) {
            $("#modal-update .modal-body").html(data);
            $("#modal-update .modal-dialog").css("max-width", "60%")
            Common.ComponentPageSlug.RegisterEvent();
        });
    },

    SubmitFormUpdateSubDescription: function () {
        $("#form-updateSubDescription").submit();
    },

    ShowDialogSubDescription: function (id) {
        Common.Ajax({
            type: "POST",
            url: ComponentPageSlug.Url.FormUpdateSubDescription,
            cache: false,
            dataType: "html",
            data: { id: id }
        }, function (data) {
            $("#modal-updateSubDescription .modal-body").html(data);
            $("#modal-updateSubDescription .modal-dialog").css("max-width", "80%")
            Common.ComponentPageSlug.RegisterEvent();
        });
    },
};
