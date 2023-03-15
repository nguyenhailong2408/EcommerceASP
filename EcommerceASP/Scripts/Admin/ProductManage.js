var ProductManage = function () {
    $("#modal-update").off('hidden.bs.modal').on('hidden.bs.modal', function (e) {
        //Common.ProductManage.SubmitForm();
    });
    $("#modal-update").off('shown.bs.modal').on('shown.bs.modal', function (e) {
        //Common.ProductManage.ShowDialog();
    });
    return this.Init();
};

ProductManage.prototype = {
    Init: function (options) {
        this.RegisterEvent();
    },
    RegisterEvent: function () {
        this.IsPaging = false;
        var that = this;
        var form = $("#form-search-ProductManage");
        $("#table-list-ProductManage > tbody > tr i.fa-edit").unbind("click").click(function () {
            var tr = $(this).closest("tr");
            Common.ProductManage.ShowDialog(tr.data("id"));
        });
        $("#table-list-ProductManage > tbody > tr i.fa-trash").unbind("click").click(function () {
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
                                url: ProductManage.Url.Delete,
                                cache: false,
                                dataType: "json",
                                data: { id: tr.data("id") }
                            }, function (result) {
                                if (result.Status) {
                                    Common.HideAlert(function () {
                                        Common.ProductManage.SubmitForm();
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
        $("#table-list-ProductManage > tbody > tr img").zoomify();

        $("#btn-update").unbind("click").click(function (e) {
            //set ckEditor value using jQuery
            $('#Infomation').val(CKEDITOR.instances["Infomation"].getData());
            $('#Description').val(CKEDITOR.instances["Description"].getData());
            Common.ProductManage.SubmitFormUpdate(e);
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
                    Common.ProductManage.UpdateSuccess(xhr.response);
                }
            };
            Common.ProductManage.UpdateBeforeSend();
            xhr.send((new FormData(form[0])));
        })


    },
    SubmitForm: function () {
        $("#form-search-ProductManage").submit();
    },
    SetPage: function (page) {
        $("#form-search-ProductManage").find("input[name='PageCurrent']").val(page);
    },
    Paging: function (page) {
        Common.ProductManage.SetPage(page);
        Common.ProductManage.IsPaging = true;
        Common.ProductManage.SubmitForm();
    },
    SuccessForm: function () {
        Common.ShowLoading(false);
        Common.ProductManage.RegisterEvent();
    },
    BeforeSend: function () {
        Common.ShowLoading(true);
        if (Common.ProductManage.IsPaging) {
            Common.ProductManage.SetPage(1);
        }
    },

    UpdateSuccess: function (res) {
        Common.ShowLoading(false);
        if (res.Status) {
            Common.ProductManage.SubmitForm();
            Common.ProductManage.HideDialog();
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
            url: ProductManage.Url.FormUpdate,
            cache: false,
            dataType: "html",
            data: { id: id }
        }, function (data) {
            $("#modal-update .modal-body").html(data);
            $("#modal-update .modal-dialog").css("max-width", "80%")
            //$("#modal-update").modal("show");

            //set ckEditor value using jQuery
            $('#Infomation').val(CKEDITOR.instances["Infomation"].getData());
            $('#Description').val(CKEDITOR.instances["Description"].getData());

            Common.ProductManage.RegisterEvent();
        });
    },

    OnBlurInputSlug: function (e) {
        Common.ProductManage.CheckExistSlug($(e).val())
            .then(function (res) {
                if ($(e).val() == '' || !$(e).val()) {
                    $("#text-alert").removeClass('text-success');
                    $("#text-alert").addClass('text-danger');
                    $("#text-alert").text(`Vui lòng nhập đường dẫn!`)
                    return;
                }

                if (!!res[0]) {
                    $("#text-alert").removeClass('text-success');
                    $("#text-alert").addClass('text-danger');
                    $("#text-alert").text(`Đường dẫn đã tồn tại ở trang: [${res[0].PageId} - ${res[0].PageName}]. Vui lòng nhập đường dẫn khác!`)
                }
                else {
                    $("#text-alert").addClass('text-success');
                    $("#text-alert").removeClass('text-danger');
                    $("#text-alert").text(`Đường dẫn hợp lệ!`)
                }
            });
    },

    CheckExistSlug: function (slug) {
        return new Promise(function (resolve, reject) {
            var option = "";
            Common.Ajax({
                type: "POST",
                url: ProductManage.Url.CheckExistSlug,
                cache: false,
                dataType: "json",
                data: {
                    strSlug: slug
                }
            }, function (res) {

                resolve(res);
            }, true);
        });
    },

    OnChangeSelectProductCategory: function (e) {
        Common.ProductManage.GetProductCategoryDetail($(e).val())
            .then(function (options) {
                $("#inputUpdateProductDetail").html(options);
            });
    },
    GetProductCategoryDetail: function (id) {
        return new Promise(function (resolve, reject) {
            var option = "";
            if (!Common.Empty(id)) {
                Common.Ajax({
                    type: "POST",
                    url: ProductManage.Url.GetProductCategoryDetail,
                    cache: false,
                    dataType: "json",
                    data: {
                        productCatId: id
                    }
                }, function (res) {
                    for (var i = 0; i < res.length; i++) {
                        option += '<option value = "' + res[i].Value + '">' + res[i].Text + '</option>';
                    }
                    resolve(option);
                }, true);
            } else {
                resolve(option);
            }
        });
    },
};
