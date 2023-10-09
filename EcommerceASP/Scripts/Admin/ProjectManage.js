this.PageCurrent = 1;
var ProjectManage = function () {
    $("#modal-update").off('hidden.bs.modal').on('hidden.bs.modal', function (e) {
        //Common.ProjectManage.SubmitForm();
    });
    $("#modal-update").off('shown.bs.modal').on('shown.bs.modal', function (e) {
        //Common.ProjectManage.ShowDialog();
    });
    return this.Init();
};

ProjectManage.prototype = {
    Init: function (options) {
        this.RegisterEvent();
    },
    RegisterEvent: function () {
        this.IsPaging = false;
        var that = this;
        var form = $("#form-search-ProjectManage");
        $("#table-list-ProjectManage > tbody > tr .btn-success").unbind("click").click(function () {
            var tr = $(this).closest("tr");
            Common.ProjectManage.ShowDialog(tr.data("id"));
        });
        $("#table-list-ProjectManage > tbody > tr .btn-danger").unbind("click").click(function () {
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
                                url: ProjectManage.Url.Delete,
                                cache: false,
                                dataType: "json",
                                data: { id: tr.data("id") }
                            }, function (result) {
                                if (result.Status) {
                                    Common.HideAlert();
                                    Common.ProjectManage.SubmitForm();
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
            Common.ProjectManage.SubmitFormUpdate(e);
        });

        $("#imgShow").zoomify();

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

        var form = $("#form-update");
        
        form.find("#file-upload-multiple").unbind("change").change(function () {
            for (var i = 0; i < this.files.length; i++) {
                var file = this.files[i];
                var fileType = file["type"];
                if (fileType.search('image') < 0) {
                    $("#file-upload-multiple").val(null);
                    Common.ShowAlert("Thông báo", "File upload phải là file hình!");
                    return false;
                }
            }
            //else {
            //    formUploadMultipleFile.find("#MultipleFile").val(this.files[0].name);
            //}
        });

        form.unbind("submit").submit(function (e) {
            e.preventDefault();
            e.stopImmediatePropagation();
            var xhr = new XMLHttpRequest();
            xhr.responseType = "json";
            xhr.open(form[0].method, form[0].action);
            xhr.onreadystatechange = function () {
                if (xhr.readyState == 4 && xhr.status == 200) {
                    Common.ProjectManage.UpdateSuccess(xhr.response);
                }
            };
            Common.ProjectManage.UpdateBeforeSend();
            xhr.send((new FormData(form[0])));
        })
    },

    SubmitForm: function () {
        //Common.ComponentTypeManage.SetPage(PageCurrent);
        $("#form-search-ProjectManage").submit();
    },
    SetPage: function (page) {
        $("#form-search-ProjectManage").find("input[name='PageCurrent']").val(page);
    },
    Paging: function (page) {
        PageCurrent = page;
        Common.ProjectManage.SetPage(page);
        Common.ProjectManage.IsPaging = true;
        Common.ProjectManage.SubmitForm();
    },
    SuccessForm: function () {
        Common.ShowLoading(false);
        Common.ProjectManage.RegisterEvent();
    },
    BeforeSend: function () {
        Common.ShowLoading(true);
        if (Common.ProjectManage.IsPaging) {
            Common.ProjectManage.SetPage(1);
        }
    },

    UpdateSuccess: function (res) {
        Common.ShowLoading(false);
        if (res.Status) {
            Common.ProjectManage.SubmitForm();
            Common.ProjectManage.HideDialog();
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
        //target.hide();
        //location.reload();
        $("#modal-update").modal("hide");

    },
    ShowDialog: function (id) {
        Common.Ajax({
            type: "POST",
            url: ProjectManage.Url.FormUpdate,
            cache: false,
            dataType: "html",
            data: { id: id }
        }, function (data) {
            $("#modal-update .modal-body").html(data);
            $("#modal-update .modal-dialog").css("max-width", "80%")
            //$("#modal-update").modal("show");

            //set ckEditor value using jQuery
            /*$('#Infomation').val(CKEDITOR.instances["Infomation"].getData());*/

            Common.ProjectManage.RegisterEvent();
        });
    },
};
