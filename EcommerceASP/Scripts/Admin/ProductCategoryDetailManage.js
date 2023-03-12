var ProductCategoryDetailManage = function () {
    $("#modal-update").off('hidden.bs.modal').on('hidden.bs.modal', function (e) {
        //Common.ProductCategoryDetailManage.SubmitForm();
    });
    $("#modal-update").off('shown.bs.modal').on('shown.bs.modal', function (e) {
        //Common.ProductCategoryDetailManage.ShowDialog();
    });
    return this.Init();
};

ProductCategoryDetailManage.prototype = {
    Init: function (options) {
        this.RegisterEvent();
    },
    RegisterEvent: function () {
        this.IsPaging = false;
        var that = this;
        var form = $("#form-search-ProductCategoryDetailManage");
        $("#table-list-ProductCategoryDetailManage > tbody > tr i.fa-edit").unbind("click").click(function () {
            var tr = $(this).closest("tr");
            Common.ProductCategoryDetailManage.ShowDialog(tr.data("id"));
        });
        $("#table-list-ProductCategoryDetailManage > tbody > tr i.fa-trash").unbind("click").click(function () {
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
                                url: ProductCategoryDetailManage.Url.Delete,
                                cache: false,
                                dataType: "json",
                                data: { id: tr.data("id") }
                            }, function (result) {
                                if (result.Status) {
                                    Common.HideAlert();
                                    Common.ProductCategoryDetailManage.SubmitForm();
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
            Common.ProductCategoryDetailManage.SubmitFormUpdate(e);
        });
    },
    SubmitForm: function () {
        $("#form-search-ProductCategoryDetailManage").submit();
    },
    SetPage: function (page) {
        $("#form-search-ProductCategoryDetailManage").find("input[name='PageCurrent']").val(page);
    },
    Paging: function (page) {
        Common.ProductCategoryDetailManage.SetPage(page);
        Common.ProductCategoryDetailManage.IsPaging = true;
        Common.ProductCategoryDetailManage.SubmitForm();
    },
    SuccessForm: function () {
        Common.ShowLoading(false);
        Common.ProductCategoryDetailManage.RegisterEvent();
    },
    BeforeSend: function () {
        Common.ShowLoading(true);
        if (Common.ProductCategoryDetailManage.IsPaging) {
            Common.ProductCategoryDetailManage.SetPage(1);
        }
    },

    UpdateSuccess: function (res) {
        Common.ShowLoading(false);
        if (res.Status) {
            Common.ProductCategoryDetailManage.SubmitForm();
            Common.ProductCategoryDetailManage.HideDialog();
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
            url: ProductCategoryDetailManage.Url.FormUpdate,
            cache: false,
            dataType: "html",
            data: { id: id }
        }, function (data) {
            $("#modal-update .modal-body").html(data);

            Common.ProductCategoryDetailManage.RegisterEvent();
        });
    },

    OnChangeSelectProductCategory: function (e) {
        Common.ProductCategoryDetailManage.GetProductCategoryDetail($(e).val())
            .then(function (options) {
                $("#inputProductCategoryDetail").html(options);
            });
    },
    OnChangeSelectProductCategorySearch: function (e) {
        Common.ProductCategoryDetailManage.GetProductCategoryDetail($(e).val())
            .then(function (options) {
                $("#inputSearchProductCategoryDetail").html(options);
            });
    },
    GetProductCategoryDetail: function (id) {
        return new Promise(function (resolve, reject) {
            var option = "";
            Common.Ajax({
                type: "POST",
                url: ProductCategoryDetailManage.Url.GetProductCategoryDetail,
                cache: false,
                dataType: "json",
                data: {
                    productCatId: id
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
        Common.ProductCategoryDetailManage.GetPageBySlug($(e).val())
            .then(function (options) {
                $("#PageId").html(options);
            });
    },

    GetPageBySlug: function (slug) {
        return new Promise(function (resolve, reject) {
            var option = "";
            Common.Ajax({
                type: "POST",
                url: ProductCategoryDetailManage.Url.GetPageBySlug,
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