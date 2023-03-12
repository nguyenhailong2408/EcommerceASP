var Product = function () {
    return this.Init();
};

Product.prototype = {
    Init: function (options) {
        this.RegisterEvent();
    },
    RegisterEvent: function () {
        this.IsPaging = false;
        var that = this;
    },
    SubmitForm: function () {
        $("#form-search-product").submit();
    },
    SetPage: function (page) {
        $("#form-search-product").find("input[name='PageCurrent']").val(page);
    },
    Paging: function (page) {
        Common.Product.SetPage(page);
        Common.Product.IsPaging = true;
        Common.Product.SubmitForm();
    },
    SuccessForm: function (res) {
        //Common.ShowLoading(false);
        Common.Product.RegisterEvent();
    },
    BeforeSend: function () {
        //Common.ShowLoading(true);
        if (Common.Product.IsPaging) {
            Common.Product.SetPage(1);
        }
    },
};
