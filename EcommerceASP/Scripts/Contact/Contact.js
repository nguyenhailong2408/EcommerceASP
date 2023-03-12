var Contact = function () {
    return this.Init();
};

Contact.prototype = {
    Init: function (options) {
        this.RegisterEvent();
    },

    RegisterEvent: function () {
        var that = this;
    },
    SuccessForm: function (res) {
        Common.ShowLoading(false);
        Common.Contact.RegisterEvent();
        if (res.Status) {
            Common.ShowAlert("Thông báo","Gửi thành công");
            Common.Contact.Reset();
        }
        else {
            Common.ShowAlert("Thông báo", res.Message);
        }
        
    },
    BeforeSend: function () {
        Common.ShowLoading(true);
    },

    Reset: function () {
        $("#FullName").val(null);
        $("#Email").val(null);
        $("#Phone").val(null);
        $("#Content").val(null);
        $("#Address").val(null);
        $("#FullName").focus();
    }
};