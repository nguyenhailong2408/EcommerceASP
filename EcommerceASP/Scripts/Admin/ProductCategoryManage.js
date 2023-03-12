var ProductCategoryManage = function () {
    $("#modal-update").off('hidden.bs.modal').on('hidden.bs.modal', function (e) {
        //Common.ProductCategoryManage.SubmitForm();
    });
    $("#modal-update").off('shown.bs.modal').on('shown.bs.modal', function (e) {
        //Common.ProductCategoryManage.ShowDialog();
    });
    return this.Init();
};

ProductCategoryManage.prototype = {
    Init: function (options) {
        this.RegisterEvent();
    },
    RegisterEvent: function () {
        this.IsPaging = false;
        var that = this;
        var form = $("#form-search-ProductCategoryManage");
        $("#table-list-ProductCategoryManage > tbody > tr i.fa-edit").unbind("click").click(function () {
            var tr = $(this).closest("tr");
            Common.ProductCategoryManage.ShowDialog(tr.data("id"));
        });
        $("#table-list-ProductCategoryManage > tbody > tr i.fa-trash").unbind("click").click(function () {
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
                                url: ProductCategoryManage.Url.Delete,
                                cache: false,
                                dataType: "json",
                                data: { id: tr.data("id") }
                            }, function (result) {
                                if (result.Status) {
                                    Common.HideAlert();
                                    Common.ProductCategoryManage.SubmitForm();
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
            Common.ProductCategoryManage.SubmitFormUpdate(e);
        });
    },
    SubmitForm: function () {
        $("#form-search-ProductCategoryManage").submit();
    },
    SetPage: function (page) {
        $("#form-search-ProductCategoryManage").find("input[name='PageCurrent']").val(page);
    },
    Paging: function (page) {
        Common.ProductCategoryManage.SetPage(page);
        Common.ProductCategoryManage.IsPaging = true;
        Common.ProductCategoryManage.SubmitForm();
    },
    SuccessForm: function () {
        Common.ShowLoading(false);
        Common.ProductCategoryManage.RegisterEvent();
    },
    BeforeSend: function () {
        Common.ShowLoading(true);
        if (Common.ProductCategoryManage.IsPaging) {
            Common.ProductCategoryManage.SetPage(1);
        }
    },

    UpdateSuccess: function (res) {
        Common.ShowLoading(false);
        if (res.Status) {
            Common.ProductCategoryManage.SubmitForm();
            Common.ProductCategoryManage.HideDialog();
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
            url: ProductCategoryManage.Url.FormUpdate,
            cache: false,
            dataType: "html",
            data: { id: id }
        }, function (data) {
            $("#modal-update .modal-body").html(data);
            //$("#modal-update .modal-dialog").css("max-width", "80%")
            //$("#modal-update").modal("show");

            //set ckEditor value using jQuery
            /*$('#Infomation').val(CKEDITOR.instances["Infomation"].getData());*/

            Common.ProductCategoryManage.RegisterEvent();
        });
    },
    OnBlurInputSlug: function (e) {
        Common.ProductCategoryManage.GetPageBySlug($(e).val())
            .then(function (options) {
                $("#PageId").html(options);
            });
    },

    GetPageBySlug: function (slug) {
        return new Promise(function (resolve, reject) {
            var option = "";
            Common.Ajax({
                type: "POST",
                url: ProductCategoryManage.Url.GetPageBySlug,
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