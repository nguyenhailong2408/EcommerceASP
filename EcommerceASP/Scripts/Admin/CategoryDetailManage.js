var CategoryDetailManage = function () {
    $("#modal-update").off('hidden.bs.modal').on('hidden.bs.modal', function (e) {
        //Common.CategoryDetailManage.SubmitForm();
    });
    $("#modal-update").off('shown.bs.modal').on('shown.bs.modal', function (e) {
        //Common.CategoryDetailManage.ShowDialog();
    });
    return this.Init();
};

CategoryDetailManage.prototype = {
    Init: function (options) {
        this.RegisterEvent();
    },
    RegisterEvent: function () {
        this.IsPaging = false;
        var that = this;
        var form = $("#form-search-CategoryDetailManage");
        $("#table-list-CategoryDetailManage > tbody > tr i.fa-edit").unbind("click").click(function () {
            var tr = $(this).closest("tr");
            Common.CategoryDetailManage.ShowDialog(tr.data("id"));
        });
        $("#table-list-CategoryDetailManage > tbody > tr i.fa-trash").unbind("click").click(function () {
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
                                url: CategoryDetailManage.Url.Delete,
                                cache: false,
                                dataType: "json",
                                data: { id: tr.data("id") }
                            }, function (result) {
                                if (result.Status) {
                                    Common.HideAlert();
                                    Common.CategoryDetailManage.SubmitForm();
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
        $("#table-list-CategoryDetailManage > tbody > tr img").zoomify();
        $("#imgShow").zoomify();

        $("#btn-update").unbind("click").click(function (e) {
            Common.CategoryDetailManage.SubmitFormUpdate(e);
        });

        $("#file-upload-image").unbind("change").change(function (e) {
            $("#BannerImage").val(this.files[0].name);
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
                    Common.CategoryDetailManage.UpdateSuccess(xhr.response);
                }
            };
            Common.CategoryDetailManage.UpdateBeforeSend();
            xhr.send((new FormData(form[0])));
        })
    },
    SubmitForm: function () {
        $("#form-search-CategoryDetailManage").submit();
    },
    SetPage: function (page) {
        $("#form-search-CategoryDetailManage").find("input[name='PageCurrent']").val(page);
    },
    Paging: function (page) {
        Common.CategoryDetailManage.SetPage(page);
        Common.CategoryDetailManage.IsPaging = true;
        Common.CategoryDetailManage.SubmitForm();
    },
    SuccessForm: function () {
        Common.ShowLoading(false);
        Common.CategoryDetailManage.RegisterEvent();
    },
    BeforeSend: function () {
        Common.ShowLoading(true);
        if (Common.CategoryDetailManage.IsPaging) {
            Common.CategoryDetailManage.SetPage(1);
        }
    },

    UpdateSuccess: function (res) {
        Common.ShowLoading(false);
        if (res.Status) {
            Common.CategoryDetailManage.SubmitForm();
            Common.CategoryDetailManage.HideDialog();
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
            url: CategoryDetailManage.Url.FormUpdate,
            cache: false,
            dataType: "html",
            data: { id: id }
        }, function (data) {
            $("#modal-update .modal-body").html(data);
            //$("#modal-update .modal-dialog").css("max-width", "80%")
            //$("#modal-update").modal("show");

            //set ckEditor value using jQuery
            /*$('#Infomation').val(CKEDITOR.instances["Infomation"].getData());*/

            Common.CategoryDetailManage.RegisterEvent();
        });
    },

    OnChangeSelectCategory: function (e) {
        Common.CategoryDetailManage.GetCategoryDetail($(e).val())
            .then(function (options) {
                $("#inputCategoryDetail").html(options);
            });
    },
    OnChangeSelectCategorySearch: function (e) {
        Common.CategoryDetailManage.GetCategoryDetail($(e).val())
            .then(function (options) {
                $("#inputSearchCategoryDetail").html(options);
            });
    },
    GetCategoryDetail: function (id) {
        return new Promise(function (resolve, reject) {
            var option = "";
            Common.Ajax({
                type: "POST",
                url: CategoryDetailManage.Url.GetCategoryDetail,
                cache: false,
                dataType: "json",
                data: {
                    CatId: id
                }
            }, function (res) {
                option = '<option value = "0"> -- Không có cha --</option>';
                for (var i = 0; i < res.length; i++) {
                    option += '<option value = "' + res[i].Value + '">' + res[i].Text + '</option>';
                }
                resolve(option);
            }, true);
        });
    },

    OnBlurInputSlug: function (e) {
        Common.CategoryDetailManage.GetPageBySlug($(e).val())
            .then(function (options) {
                $("#PageId").html(options);
            });
    },

    GetPageBySlug: function (slug) {
        return new Promise(function (resolve, reject) {
            var option = "";
            Common.Ajax({
                type: "POST",
                url: CategoryDetailManage.Url.GetPageBySlug,
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