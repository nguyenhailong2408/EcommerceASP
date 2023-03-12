var PostDetailManage = function () {
    $("#modal-update").off('hidden.bs.modal').on('hidden.bs.modal', function (e) {
        //Common.PostDetailManage.SubmitForm();
    });
    $("#modal-update").off('shown.bs.modal').on('shown.bs.modal', function (e) {
        //Common.PostDetailManage.ShowDialog();
    });
    return this.Init();
};

PostDetailManage.prototype = {
    Init: function (options) {
        this.RegisterEvent();
    },
    RegisterEvent: function () {
        this.IsPaging = false;
        var that = this;
        var form = $("#form-search-PostDetailManage");
        $("#table-list-PostDetailManage > tbody > tr i.fa-edit").unbind("click").click(function () {
            var tr = $(this).closest("tr");
            Common.PostDetailManage.ShowDialog(tr.data("id"));
        });
        $("#table-list-PostDetailManage > tbody > tr i.fa-trash").unbind("click").click(function () {
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
                                url: PostDetailManage.Url.Delete,
                                cache: false,
                                dataType: "json",
                                data: { id: tr.data("id") }
                            }, function (result) {
                                if (result.Status) {
                                    Common.HideAlert();
                                    Common.PostDetailManage.SubmitForm();
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
        $("#table-list-PostDetailManage > tbody > tr img").zoomify();

        $("#btn-update").unbind("click").click(function (e) {
            //set ckEditor value using jQuery
            $('#Content').val(CKEDITOR.instances["Content"].getData());
            $('#Description').val(CKEDITOR.instances["Description"].getData());
            Common.PostDetailManage.SubmitFormUpdate(e);
        });

        $("#file-upload-image").unbind("change").change(function (e) {
            $("#ThumbnailImage").val(this.files[0].name);
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
                    Common.PostDetailManage.UpdateSuccess(xhr.response);
                }
            };
            Common.PostDetailManage.UpdateBeforeSend();
            xhr.send((new FormData(form[0])));
        })
    },
    SubmitForm: function () {
        $("#form-search-PostDetailManage").submit();
    },
    SetPage: function (page) {
        $("#form-search-PostDetailManage").find("input[name='PageCurrent']").val(page);
    },
    Paging: function (page) {
        Common.PostDetailManage.SetPage(page);
        Common.PostDetailManage.IsPaging = true;
        Common.PostDetailManage.SubmitForm();
    },
    SuccessForm: function () {
        Common.ShowLoading(false);
        Common.PostDetailManage.RegisterEvent();
    },
    BeforeSend: function () {
        Common.ShowLoading(true);
        if (Common.PostDetailManage.IsPaging) {
            Common.PostDetailManage.SetPage(1);
        }
    },

    UpdateSuccess: function (res) {
        Common.ShowLoading(false);
        if (res.Status) {
            Common.PostDetailManage.SubmitForm();
            Common.PostDetailManage.HideDialog();
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
            url: PostDetailManage.Url.FormUpdate,
            cache: false,
            dataType: "html",
            data: { id: id }
        }, function (data) {
            $("#modal-update .modal-body").html(data);
            $("#modal-update .modal-dialog").css("max-width", "80%");
            //$("#modal-update").modal("show");

            //set ckEditor value using jQuery
            /*$('#Infomation').val(CKEDITOR.instances["Infomation"].getData());*/

            Common.PostDetailManage.RegisterEvent();
        });
    },
};