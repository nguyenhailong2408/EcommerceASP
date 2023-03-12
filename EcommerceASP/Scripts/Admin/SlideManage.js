var SlideManage = function () {
    $("#modal-update").off('hidden.bs.modal').on('hidden.bs.modal', function (e) {
        //Common.SlideManage.SubmitForm();
    });
    $("#modal-update").off('shown.bs.modal').on('shown.bs.modal', function (e) {
        //Common.SlideManage.ShowDialog();
    });
    return this.Init();
};

SlideManage.prototype = {
    Init: function (options) {
        this.RegisterEvent();
    },
    RegisterEvent: function () {
        this.IsPaging = false;
        var that = this;
        var form = $("#form-search-SlideManage");
        $("#table-list-SlideManage > tbody > tr i.fa-edit").unbind("click").click(function () {
            var tr = $(this).closest("tr");
            Common.SlideManage.ShowDialog(tr.data("id"));
        });
        $("#table-list-SlideManage > tbody > tr i.fa-trash").unbind("click").click(function () {
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
                                url: SlideManage.Url.Delete,
                                cache: false,
                                dataType: "json",
                                data: { id: tr.data("id") }
                            }, function (result) {
                                if (result.Status) {
                                    Common.HideAlert(function () {
                                        Common.SlideManage.SubmitForm();
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
        $("#table-list-SlideManage > tbody > tr img").zoomify();

        $("#btn-update").unbind("click").click(function (e) {
            Common.SlideManage.SubmitFormUpdate(e);
        });

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
                    Common.SlideManage.UpdateSuccess(xhr.response);
                }
            };
            Common.SlideManage.UpdateBeforeSend();
            xhr.send((new FormData(form[0])));
        })


    },
    SubmitForm: function () {
        $("#form-search-SlideManage").submit();
    },
    SetPage: function (page) {
        $("#form-search-SlideManage").find("input[name='PageCurrent']").val(page);
    },
    Paging: function (page) {
        Common.SlideManage.SetPage(page);
        Common.SlideManage.IsPaging = true;
        Common.SlideManage.SubmitForm();
    },
    SuccessForm: function () {
        Common.ShowLoading(false);
        Common.SlideManage.RegisterEvent();
    },
    BeforeSend: function () {
        Common.ShowLoading(true);
        if (Common.SlideManage.IsPaging) {
            Common.SlideManage.SetPage(1);
        }
    },

    UpdateSuccess: function (res) {
        Common.ShowLoading(false);
        if (res.Status) {
            Common.SlideManage.SubmitForm();
            Common.SlideManage.HideDialog();
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
            url: SlideManage.Url.FormUpdate,
            cache: false,
            dataType: "html",
            data: { id: id }
        }, function (data) {
            $("#modal-update .modal-body").html(data);

            Common.SlideManage.RegisterEvent();
        });
    },

    OnBlurInputSlug: function (e) {
        Common.SlideManage.GetPageBySlug($(e).val())
            .then(function (options) {
                $("#PageId").html(options);
            });
    },

    GetPageBySlug: function (slug) {
        return new Promise(function (resolve, reject) {
            var option = "";
            Common.Ajax({
                type: "POST",
                url: SlideManage.Url.GetPageBySlug,
                cache: false,
                dataType: "json",
                data: {
                    strSlug: slug
                }
            }, function (res) {
                option = '<option value = "0"> -- Không tìm thấy trang --</option>';
                for (var i = 0; i < res.length; i++) {
                    option += '<option value = "' + res[i].Value + '" selected="' + (res[i].Selected ? 'selected' : '') + '">' + res[i].Text + '</option>';
                }
                resolve(option);
            }, true);
        });
    },
};
