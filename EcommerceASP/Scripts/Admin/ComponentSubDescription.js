var ComponentSubDescription = function () {
    $("#modal-update").off('hidden.bs.modal').on('hidden.bs.modal', function (e) {
        //Common.ComponentSubDescription.SubmitForm();
    });
    $("#modal-update").off('shown.bs.modal').on('shown.bs.modal', function (e) {
        //Common.ComponentSubDescription.ShowDialog();
    });
    return this.Init();
};

ComponentSubDescription.prototype = {
    Init: function (options) {
        this.RegisterEvent();
    },
    RegisterEvent: function () {
        this.IsPaging = false;
        var that = this;
        var form = $("#form-search-ComponentSubDescription");
        $("#table-list-ComponentSubDescription > tbody > tr i.fa-edit").unbind("click").click(function () {
            var tr = $(this).closest("tr");
            Common.ComponentTypeManage.ShowDialog(tr.data("id"));
        });
        $("#table-list-ComponentSubDescription > tbody > tr i.fa-trash").unbind("click").click(function () {
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

        $("#btn-update").unbind("click").click(function (e) {
            Common.ComponentSubDescription.SubmitFormUpdate(e);
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
        $("#table-list-ComponentSubDescription > tbody > tr img").zoomify();
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
                    Common.ComponentSubDescription.UpdateSuccess(xhr.response);
                }
            };
            Common.ComponentSubDescription.UpdateBeforeSend();
            xhr.send((new FormData(form[0])));
        })


    },
    SubmitForm: function () {
        $("#form-search-ComponentSubDescription").submit();
    },
    SetPage: function (page) {
        $("#form-search-ComponentSubDescription").find("input[name='PageCurrent']").val(page);
    },
    Paging: function (page) {
        Common.ComponentSubDescription.SetPage(page);
        Common.ComponentSubDescription.IsPaging = true;
        Common.ComponentSubDescription.SubmitForm();
    },
    SuccessForm: function () {
        Common.ShowLoading(false);
        Common.ComponentSubDescription.RegisterEvent();
    },
    BeforeSend: function () {
        Common.ShowLoading(true);
        if (Common.ComponentSubDescription.IsPaging) {
            Common.ComponentSubDescription.SetPage(1);
        }
    },

    UpdateSuccess: function (res) {
        Common.ShowLoading(false);
        if (res.Status) {
            Common.ComponentSubDescription.SubmitForm();
            Common.ComponentSubDescription.HideDialog();
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
            url: ComponentSubDescription.Url.FormUpdate,
            cache: false,
            dataType: "html",
            data: { id: id }
        }, function (data) {
            $("#modal-update .modal-body").html(data);

            Common.ComponentSubDescription.RegisterEvent();
        });
    },
};
