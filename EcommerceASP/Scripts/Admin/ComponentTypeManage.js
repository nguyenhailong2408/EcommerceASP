var ComponentTypeManage = function () {
    $("#modal-update").off('hidden.bs.modal').on('hidden.bs.modal', function (e) {
        //Common.ComponentTypeManage.SubmitForm();
    });
    $("#modal-update").off('shown.bs.modal').on('shown.bs.modal', function (e) {
        //Common.ComponentTypeManage.ShowDialog();
    });
    return this.Init();
};

ComponentTypeManage.prototype = {
    Init: function (options) {
        this.PageCurrent = 1;
        this.RegisterEvent();
    },
    RegisterEvent: function () {
        this.IsPaging = false;
        var that = this;
        var form = $("#form-search-ComponentTypeManage");
        $("#table-list-ComponentTypeManage > tbody > tr i.fa-edit").unbind("click").click(function () {
            var tr = $(this).closest("tr");
            Common.ComponentTypeManage.ShowDialog(tr.data("id"));
        });
        $("#table-list-ComponentTypeManage > tbody > tr i.fa-trash").unbind("click").click(function () {
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
                                url: ComponentTypeManage.Url.Delete,
                                cache: false,
                                dataType: "json",
                                data: { id: tr.data("id") }
                            }, function (result) {
                                if (result.Status) {
                                    Common.HideAlert(function () {
                                        Common.ComponentTypeManage.SubmitForm();
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
        $("#table-list-ComponentTypeManage > tbody > tr img").zoomify();

        $("#btn-update").unbind("click").click(function (e) {
            Common.ComponentTypeManage.SubmitFormUpdate(e);
        });

        $("#file-upload-image").unbind("change").change(function (e) {
            $("#DescriptionImage").val(this.files[0].name);
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
        $("#imgShow").zoomify();

        var form = $("#form-update");

        form.unbind("submit").submit(function (e) {
            e.preventDefault();
            e.stopImmediatePropagation();
            var xhr = new XMLHttpRequest();
            xhr.responseType = "json";
            xhr.open(form[0].method, form[0].action);
            xhr.onreadystatechange = function () {
                if (xhr.readyState == 4 && xhr.status == 200) {
                    Common.ComponentTypeManage.UpdateSuccess(xhr.response);
                }
            };
            Common.ComponentTypeManage.UpdateBeforeSend();
            xhr.send((new FormData(form[0])));
        })


    },
    SubmitForm: function () {
        Common.ComponentTypeManage.SetPage(Common.ComponentTypeManage.PageCurrent);
        $("#form-search-ComponentTypeManage").submit();
    },
    SetPage: function (page) {
        $("#form-search-ComponentTypeManage").find("input[name='PageCurrent']").val(page);
    },
    Paging: function (page) {
        Common.ComponentTypeManage.PageCurrent = page;
        Common.ComponentTypeManage.SetPage(page);
        Common.ComponentTypeManage.IsPaging = true;
        Common.ComponentTypeManage.SubmitForm();
    },
    SuccessForm: function () {
        Common.ShowLoading(false);
        Common.ComponentTypeManage.RegisterEvent();
    },
    BeforeSend: function () {
        Common.ShowLoading(true);
        if (Common.ComponentTypeManage.IsPaging) {
            Common.ComponentTypeManage.SetPage(1);
        }
    },

    UpdateSuccess: function (res) {
        Common.ShowLoading(false);
        if (res.Status) {
            Common.ComponentTypeManage.SubmitForm();
            Common.ComponentTypeManage.HideDialog();
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
        //target = $("#modal-update");
        //target.removeClass("in");
        //$(".modal-backdrop").remove();
        //target.hide()

        $("#modal-update").modal("hide");

    },
    ShowDialog: function (id) {
        Common.Ajax({
            type: "POST",
            url: ComponentTypeManage.Url.FormUpdate,
            cache: false,
            dataType: "html",
            data: { id: id }
        }, function (data) {
            $("#modal-update .modal-body").html(data);

            Common.ComponentTypeManage.RegisterEvent();
        });
    },
};
