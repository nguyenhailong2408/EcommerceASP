Common = function () {
    /*this.AddCustomValidation();*/
    return this.Init();
};
Common.prototype = {
    Init: function () {
        $(document).ready(function () {
            $(".main-sidebar").unbind("mouseenter mouseleave");
            //of string concatenation
            String.prototype.format = function () {
                var st = this;
                for (i = 0; i < arguments.length; i++) {
                    var value = arguments[i];
                    var re = new RegExp("\{[" + i + "]\}", "g");
                    st = st.replace(re, value);
                }
                return st;
            };

            $(window).scroll(function () {
                if ($(window).scrollTop() > 350) {
                    $("#footer-container-move").show();
                }
                else {
                    $("#footer-container-move").hide();
                }
            });

            $('body').on('hidden.bs.modal', function (event) {
                if ($('div.modal:visible').length) {
                    $(this).addClass('modal-open');
                }
            });
        });
    },

    InitializeForm: function (form) {
        var that = this;
        if ($(form).data("user-guide") == true) {
            $(form).find("input,select").attr("title", "Sử dụng chuột phải để xóa");
            $(form).find(".action-popup .glyphicon-plus-sign").attr("title", "Thêm");
            $(form).find(".action-popup .glyphicon-minus-sign").attr("title", "Xóa những trường đã chọn");
        }
        $(form).find("input,select").bind("contextmenu", function (e) {
            $(this).val("");
            return false;
        });
        $(form).find("input[data-val-date]").datepicker({
            dateFormat: $.datepicker._defaults.dateFormat,
            onSelect: function () {
                var validator = $(this).closest("form").validate();
                validator.element($(this));
            }
        });
        $(form).find("input[data-val-date]").attr("readonly");
        $(form).find("select[data-popup]").each(function () {
            $(this).parent().find(".action-popup .glyphicon").data("element", this);
            $(this).parent().find(".action-popup .glyphicon-plus-sign").unbind("click").click(function () {
                var element = $(this).data("element");
                if (!Common.empty($(element).attr("disabled"))) {
                    return;
                }

                var action = $(element).data("popup");
                Common.ShowPopup({
                    Url: that.Url.PopupSearchDialog + "/" + action,
                    CallBack: function (response) {
                        var option = "<option value='#Id' selected>#Name</option>";
                        var html = [];
                        for (var index in response) {
                            var model = response[index];
                            if ($(element).find("option[value='" + model.Id + "']").length == 0) {
                                html.push(Common.BindHtml(option, model));
                            }
                        }
                        $(element).html($(element).html() + html.join(""));
                        $(element).change();
                    }
                });
            });
            $(this).parent().find(".action-popup .glyphicon-minus-sign").unbind("click").click(function () {
                var element = $(this).data("element");
                if (!Common.empty($(element).attr("disabled"))) {
                    return;
                }
                var selectedValues = $(element).find("option:selected");
                selectedValues.remove();
                $(element).change();
            });
        });
        this.RebindValidate(form);
    },
    RebindValidate: function (form) {
        var $form = $(form);
        if ($form.length > 0) {
            $form.unbind();
            $form.data("validator", null);

            $.validator.unobtrusive.parse($form);
            // Re add validation with changes
            $form.validate($form.data("unobtrusiveValidation").options);
            if ($form.data("validate-tooltip") == true) {
                this.EnableValidateToolTip($form);
            }
        }
    },
    Keys: {
        8: true,
        35: true,
        36: true,
        37: true,
        39: true,
    },
    //cho phép nhập dấu trừ
    ValidateDecimalTrueMinus: function (event, target) {
        var keyCode = event.charCode || event.keyCode || event.which;
        if ((keyCode >= 48 && keyCode <= 57) || keyCode == 45) {
            var text = $(target).val();
            if (text.indexOf("-") != -1 && keyCode == 45)
                event.preventDefault();
        } else {
            if (!Common.Empty(String.fromCharCode(keyCode)) && !Common.Keys[keyCode]) {
                event.preventDefault();
            }
        }
    },
    ValidateDecimal: function (event, target) {
        var keyCode = event.charCode || event.keyCode || event.which;
        if ((keyCode >= 48 && keyCode <= 57) || keyCode == 13) {
            var text = $(target).val();
        } else {
            event.preventDefault();
        }
    },
    ValidateDecimalAllowPoint: function (event, target) {
        var keyCode = event.charCode || event.keyCode || event.which;
        if ((keyCode >= 48 && keyCode <= 57)) {
            var text = $(target).val();
        } else if (keyCode == 46) {
            if (Common.Empty($(target).val()) || $(target).val().indexOf('.') != -1) {
                event.preventDefault();
            }
        } else {
            if (!Common.Empty(String.fromCharCode(keyCode)) && !Common.Keys[keyCode]) {
                event.preventDefault();
            }
        }
    },
    ValidateInteger: function (event, target) {
        var keyCode = event.charCode || event.keyCode || event.which;
        if ((keyCode < 48 && keyCode > 57)) {
            if (!Common.Empty(String.fromCharCode(keyCode)) && !Common.Keys[keyCode]) {
                event.preventDefault();
            }
        }
    },
    AddCustomValidation: function () {
        $.datepicker.setDefaults({
            dateFormat: "dd/mm/yy",
            //changeMonth: true,
            //changeYear: true,
        });
    },
    ShowForm: function (option) {
        var that = this;
        if (Common.Empty(this.LoadingPages)) {
            this.LoadingPages = new Object();
        }
        if (Common.Empty(this.LoadingPages[option.url])) {
            this.LoadingPages[option.url] = true;
            Common.ShowLoading(true);
            $.ajax({
                url: option.url,
                data: option.data,
                complete: function () {
                    setTimeout(function () {
                        delete that.LoadingPages[option.url];
                    }, 1000);
                },
                success: function (html) {
                    $(option.element).html(html);
                    if (option.callback != undefined && typeof (option.callback) == "function") {
                        option.callback();
                    }
                },
            });
        }
    },
    Empty: function (target) {
        if (target == null || target == undefined || target == "" || target.length == 0 || jQuery.trim(target).length == 0)
            return true;
        return false;
    },
    Serialize: function (el) {
        var serialized = $(el).serialize();
        if (!serialized) // not a form
            serialized = $(el).
                find('input[name],select[name],textarea[name]').serialize();
        return serialized;
    },
    ShowLoading: function (isShow) {
        if (isShow) {
            $(".loading").show();
        } else {
            $(".loading").hide();
        }
    },
    GetDateFormat: function (date) {
        return $.datepicker.formatDate($.datepicker._defaults.dateFormat, date);
    },
    // Common.DisableButton("#btn_previous_calendar", true);
    DisableButton: function (selector, isDisable) {
        if (isDisable) {
            $(selector).prop('disabled', true);
        } else {
            $(selector).prop('disabled', false);
        }
    },
    ShowAlert: function (title, body, buttons, className, funcHidden) {
        target = $("#modal-alert");
        $("#modal-alert").modal({ backdrop: 'static', keyboard: false });
        function HideButtons() {
            $(target).find(".modal-footer .btn").hide();
        }
        function ShowButtons(buttons) {
            var footer = $(target).find(".modal-footer");
            $(footer).find(".new-btn").remove();
            if (buttons.Close != undefined) {
                if (buttons.Close.Display) {
                    $(footer).find(".btn-close").show();
                } else {
                    $(footer).find(".btn-close").hide();
                }
                if (buttons.Close.OnClick) {
                    $(footer).find(".btn-close").click(function (e, v) {
                        buttons.Close.OnClick(e, v);
                    });
                }
            }
            if (!Common.Empty(buttons.Items)) {
                for (var index in buttons.Items) {
                    var button = buttons.Items[index];
                    if (button.Name == undefined) continue;
                    var classs = "";
                    var dataDismiss = ""
                    if (button.Class == undefined) {
                        classs = "btn-primary";
                    }
                    else {
                        if (!Common.Empty(button.Class)) {
                            classs = button.Class;
                        }
                    }
                    if (button.DataDismiss == undefined) {
                        dataDismiss = "";
                    }
                    else {
                        if (!Common.Empty(button.DataDismiss)) {
                            dataDismiss = button.DataDismiss;
                        }
                    }

                    $(footer).prepend('<button type="button" class="btn btn-sm ' + classs + ' new-btn btn-' + button.Name + '" ' + dataDismiss + ' ></button');
                    var element = $(footer).find(".new-btn.btn-" + button.Name);
                    if (button.Value != undefined) {
                        element.html(button.Value);
                    }
                    $(element).data("this", button);
                    if (button.OnClick != undefined) {
                        element.click(button.OnClick);
                    }
                }
            }
        }
        $(target).find(".modal-title").html(title);
        $(target).find(".modal-body").html(body);
        HideButtons();
        if (buttons == undefined) {
            $(target).find(".modal-footer .btn-close").show();
        } else {
            ShowButtons(buttons);
        }
        if (typeof className != 'undefined') {
            FocusButton(className);
        }
        function FocusButton(className) {
            $(target).unbind("shown.bs.modal").off("shown.bs.modal").on('shown.bs.modal', function () {
                $(target).off("shown.bs.modal");
                $('.btn-' + className).focus();
            });
        }
        if (funcHidden != 'undefined') {
            $(target).unbind("hidden.bs.modal").off("hidden.bs.modal").on('hidden.bs.modal', function () {
                $(target).off("hidden.bs.modal");
                try { funcHidden(); } catch (ex) { }
            });
        }
        $(target).modal("toggle");
        Common.ShowLoading(false);
    },
    HideAlert: function (close, timeout) {
        target = $("#modal-alert");
        target.modal("hide");
        //$(".modal-backdrop").unbind("click");
        //$(".modal-backdrop").remove();
        $(target).unbind("hidden.bs.modal").off("hidden.bs.modal").on('hidden.bs.modal', function () {
            // $('.modal:visible').length && $(document.body).addClass('modal-open');//scroll
            $(target).off("hidden.bs.modal");
            if (typeof close == 'function') {
                close();
            }
        });
    },
    ShowPopup: function (option) {
        var that = this;
        if (!Common.Empty(option)) {
            $('<div class="modal-backdrop fade in"></div>').appendTo(document.body);
            var params = !Common.Empty(option.Data) ? "?" + $.param(option.Data, true) : "";
            var target = window.open(option.Url + params, '', "width=1024,height=600,directories=no,titlebar=no,toolbar=no,location=no,status=no,menubar=no,scrollbars=yes,resizable=yes");
            $(target).focusout(function () {
                alert("abc");
            });
            var myEvent = target.attachEvent || target.addEventListener;
            var chkevent = target.attachEvent ? 'onbeforeunload' : 'beforeunload'; /// make IE7, IE8 compitable
            $(".modal-backdrop").unbind("click").click(function () {
                target.focus();
            });
            myEvent(chkevent, function (e) { // For >=IE7, Chrome, Firefox
                $(".modal-backdrop").unbind("click");
                $(".modal-backdrop").remove();
                if (typeof (option.CallBack) == "function") {
                    option.CallBack(this.Response);
                }
            });
        }
    },
    LoadingCount: 0, //Count loading request
    Ajax: function (setting, done) {
        var idbrach = Common.getCookie("Branch");
        if (!Common.Empty($("#select-branch")) && $("#select-branch").val() != idbrach) {
            window.location.reload();
        }
        Common.ShowLoading(true);
        //There is one more loading request
        Common.LoadingCount++;
        //$('.loading').stop().fadeToggle(Common.LoadingCount > 0);
        //Add default setting
        setting = $.extend({
            type: "POST",
            async: true,
            cache: false,
            dataType: "json"
        }, setting);

        return $.ajax(setting).done(function (data, textStatus, jqXHR) {
            //Call done callback
            if (typeof data == "string" && data.indexOf("Common.RedirectLoginUrl") == 0) {
                eval(data);
            } else {
                done(data);
            }
        }).then(function (jqXHR, textStatus, errorThrown) {
            //Server not respond
            if (!jqXHR.responseJSON) return;

            switch (jqXHR.responseJSON.errorCode) {
                case '403':
                    //Not login error
                    alert(jqXHR.responseJSON.errorMessage);
                    //Redirect to login page
                    if (window && window.location) {
                        window.location.href = "/";
                    }
                    break;
                case '500':
                    alert(jqXHR);
                    alert(textStatus);
                    alert(errorThrown);
                    break;

                default:
                    //Other error
                    alert(jqXHR.responseJSON.errorMessage);
                    break;
            }
        }).always(function () {
            // There is one less loading request
            Common.LoadingCount--;
            if (Common.LoadingCount == 0) {
                Common.ShowLoading(false);
            } else {
                Common.ShowLoading(true);
            }
            //$('.loading').stop().fadeToggle(Common.LoadingCount > 0);
            //Common.ShowLoading(false);
        });
    },
    AjaxNoLoading: function (setting, done) {
        var idbrach = Common.getCookie("Branch");
        if (!Common.Empty($("#select-branch")) && $("#select-branch").val() != idbrach) {
            window.location.reload();
        }
        Common.ShowLoading(false);
        //There is one more loading request
        Common.LoadingCount++;
        //$('.loading').stop().fadeToggle(Common.LoadingCount > 0);
        //Add default setting
        setting = $.extend({
            type: "POST",
            async: true,
            cache: false,
            dataType: "json"
        }, setting);

        return $.ajax(setting).done(function (data, textStatus, jqXHR) {
            //Call done callback
            if (typeof data == "string" && data.indexOf("Common.RedirectLoginUrl") == 0) {
                eval(data);
            } else {
                done(data);
            }
        }).error(function (jqXHR, textStatus, errorThrown) {
            //Server not respond
            if (!jqXHR.responseJSON) return;

            switch (jqXHR.responseJSON.errorCode) {
                case '403':
                    //Not login error
                    alert(jqXHR.responseJSON.errorMessage);
                    //Redirect to login page
                    if (window && window.location) {
                        window.location.href = "/";
                    }
                    break;
                case '500':
                    alert(jqXHR);
                    alert(textStatus);
                    alert(errorThrown);
                    break;

                default:
                    //Other error
                    alert(jqXHR.responseJSON.errorMessage);
                    break;
            }
        }).always(function () {
            // There is one less loading request
            Common.LoadingCount--;
            if (Common.LoadingCount == 0) {
                Common.ShowLoading(false);
            } else {
                Common.ShowLoading(true);
            }
            //$('.loading').stop().fadeToggle(Common.LoadingCount > 0);
            //Common.ShowLoading(false);
        });
    },
    ReplaceFormatCurrency: function (num) {
        if (!Common.Empty(num)) {
            var str = num.toString();
            var decPoint = ",";
            if (str.indexOf(decPoint) >= 0) {
                str = str.replace(new RegExp(decPoint, "g"), "");
                return str;
            }
        }
        return num;
    },
    //format number(money)
    FormatCurrency: function (num) {
        if (!Common.Empty(num) && num != null) {
            num = Common.ReplaceFormatCurrency(num);
            var str = num;
            var parts = [];
            var output = "";
            var i = 1;
            var sub = "";
            var formatted = null;
            if (str.indexOf(".") > 0) {
                parts = str.split(".");
                str = parts[0];
            }
            if (str.indexOf("-") != -1) {
                sub = str.substr(0, 1);
                str = str.substr(1);
            }
            var len = str.length;
            while (len > 3) {
                output = "," + str.substr(len - 3, 3) + output;
                len -= 3;
            }
            output = str.substr(0, len) + output;
            return (sub + output + (parts.length == 2 ? "." + parts[1] : ""));
        }
        return num;
    },
    CheckPopupBlocked: function (poppedWindow) {
        setTimeout(function () {
            Common.DoCheckPopupBlocked(poppedWindow);
        }, 500);
    },
    DoCheckPopupBlocked: function (poppedWindow) {
        var result = false;

        try {
            if (typeof poppedWindow == 'undefined') {
                // Safari with popup blocker... leaves the popup window handle undefined
                result = true;
            }
            else if (poppedWindow && poppedWindow.closed) {
                // This happens if the user opens and closes the client window...
                // Confusing because the handle is still available, but it's in a "closed" state.
                // We're not saying that the window is not being blocked, we're just saying
                // that the window has been closed before the test could be run.
                result = false;
            }
            else if (poppedWindow && poppedWindow.outerWidth == 0) {
                // This is usually Chrome's doing. The outerWidth (and most other size/location info)
                // will be left at 0, EVEN THOUGH the contents of the popup will exist (including the
                // test function we check for next). The outerWidth starts as 0, so a sufficient delay
                // after attempting to pop is needed.
                result = true;
            }
            else if (poppedWindow && poppedWindow.test) {
                // This is the actual test. The client window should be fine.
                result = false;
            }
            else {
                // Else we'll assume the window is not OK
                result = true;
            }
        } catch (err) {
            //if (console) {
            //    console.warn("Could not access popup window", err);
            //}
        }

        if (result) {
            //tiến hành lưu rồi in phiếu bán hàng và giấy bảo hành
            Common.ShowAlert("Thông Báo", "Chức năng in không thể thực hiện do trình duyệt không cho phép hiển thị Popup. Hãy cho phép hiển thị Popup để sử dụng chức năng này.", {
                Close: { Display: false },
                Items: {
                    Yes: {
                        Name: "Close",
                        Data: $(this),
                        OnClick: function (target) {
                            Common.HideAlert();
                            $(".modal-backdrop").unbind("click");
                            $(".modal-backdrop").remove();
                        },
                        Value: "Đóng"
                    }
                }
            });
            window.setTimeout(function () { $("#modal-alert").find(".close-modal.btn-Close").focus() }, 500);
        }
    },

    AddZero: function (i) {
        if (i < 10) {
            i = "0" + i;
        }
        return i;
    },
    NewGuild: function () {
        var d = new Date().getTime();
        var uuid = 'xxxxxxxx-xxxx-4xxx-yxxx-xxxxxxxxxxxx'.replace(/[xy]/g, function (c) {
            var r = (d + Math.random() * 16) % 16 | 0;
            d = Math.floor(d / 16);
            return (c == 'x' ? r : (r & 0x7 | 0x8)).toString(16);
        });
        return uuid;
    },
    getCookie: function (cname) {
        var name = cname + "=";
        var ca = document.cookie.split(';');
        for (var i = 0; i < ca.length; i++) {
            var c = ca[i];
            while (c.charAt(0) == ' ') c = c.substring(1);
            if (c.indexOf(name) == 0) return c.substring(name.length, c.length);
        }
        return "";
    },
    setCookie: function (cname, cvalue, exdays) {
        var d = new Date();
        d.setTime(d.getTime() + (exdays * 24 * 60 * 60 * 1000));
        var expires = "expires=" + d.toUTCString();
        document.cookie = cname + "=" + cvalue + "; " + expires;
    },
    dateTimeReviver: function (key, value) {
        var a;
        if (typeof value === 'string') {
            a = /\/Date\((\d*)\)\//.exec(value);
            if (a) {
                var date = new Date(+a[1]);
                return (date.getMonth() + 1) + "/" + date.getDate() + "/" + date.getFullYear();
            }
        }
        return value;
    },
    SetFocusPosition: function (current) {
        var strLength = current.val().length
        current.focus();
        current[0].setSelectionRange(strLength, strLength);
        current.select();
    },
    ParseDate: function (format, date) {
        return $.datepicker.parseDate(format, date);
    },
    CalcuDate: function (date1, date2) {
        var date = {};
        var diff = date2.getTime() - date1.getTime();

        date.days = Math.floor(diff / (1000 * 60 * 60 * 24));
        diff -= date.days * (1000 * 60 * 60 * 24);

        date.hours = Math.floor(diff / (1000 * 60 * 60));
        diff -= date.hours * (1000 * 60 * 60);

        date.mins = Math.floor(diff / (1000 * 60));
        diff -= date.mins * (1000 * 60);

        date.seconds = Math.floor(diff / (1000));
        diff -= date.seconds * (1000);
        return date;
    },
    RedirectLoginUrl: function (url) {
        window.location = url;
    },
    ReadCookie: function (name) {
        var nameEQ = name + "=";
        var ca = document.cookie.split(';');
        for (var i = 0; i < ca.length; i++) {
            var c = ca[i];
            while (c.charAt(0) == ' ') c = c.substring(1, c.length);
            if (c.indexOf(nameEQ) == 0) return c.substring(nameEQ.length, c.length).toString();
        }
        return null;
    },
    RemoveVietnameseTones: function removeVietnameseTones(str) {
        str = str.replace(/à|á|ạ|ả|ã|â|ầ|ấ|ậ|ẩ|ẫ|ă|ằ|ắ|ặ|ẳ|ẵ/g, "a");
        str = str.replace(/è|é|ẹ|ẻ|ẽ|ê|ề|ế|ệ|ể|ễ/g, "e");
        str = str.replace(/ì|í|ị|ỉ|ĩ/g, "i");
        str = str.replace(/ò|ó|ọ|ỏ|õ|ô|ồ|ố|ộ|ổ|ỗ|ơ|ờ|ớ|ợ|ở|ỡ/g, "o");
        str = str.replace(/ù|ú|ụ|ủ|ũ|ư|ừ|ứ|ự|ử|ữ/g, "u");
        str = str.replace(/ỳ|ý|ỵ|ỷ|ỹ/g, "y");
        str = str.replace(/đ/g, "d");
        str = str.replace(/À|Á|Ạ|Ả|Ã|Â|Ầ|Ấ|Ậ|Ẩ|Ẫ|Ă|Ằ|Ắ|Ặ|Ẳ|Ẵ/g, "A");
        str = str.replace(/È|É|Ẹ|Ẻ|Ẽ|Ê|Ề|Ế|Ệ|Ể|Ễ/g, "E");
        str = str.replace(/Ì|Í|Ị|Ỉ|Ĩ/g, "I");
        str = str.replace(/Ò|Ó|Ọ|Ỏ|Õ|Ô|Ồ|Ố|Ộ|Ổ|Ỗ|Ơ|Ờ|Ớ|Ợ|Ở|Ỡ/g, "O");
        str = str.replace(/Ù|Ú|Ụ|Ủ|Ũ|Ư|Ừ|Ứ|Ự|Ử|Ữ/g, "U");
        str = str.replace(/Ỳ|Ý|Ỵ|Ỷ|Ỹ/g, "Y");
        str = str.replace(/Đ/g, "D");
        // Some system encode vietnamese combining accent as individual utf-8 characters
        // Một vài bộ encode coi các dấu mũ, dấu chữ như một kí tự riêng biệt nên thêm hai dòng này
        str = str.replace(/\u0300|\u0301|\u0303|\u0309|\u0323/g, ""); // ̀ ́ ̃ ̉ ̣  huyền, sắc, ngã, hỏi, nặng
        str = str.replace(/\u02C6|\u0306|\u031B/g, ""); // ˆ ̆ ̛  Â, Ê, Ă, Ơ, Ư
        // Remove extra spaces
        // Bỏ các khoảng trắng liền nhau
        str = str.replace(/ + /g, " ");
        str = str.trim();
        // Remove punctuations
        // Bỏ dấu câu, kí tự đặc biệt
        str = str.replace(/!|@|%|\^|\*|\(|\)|\+|\=|\<|\>|\?|\/|,|\.|\:|\;|\'|\"|\&|\#|\[|\]|~|\$|_|`|-|{|}|\||\\/g, " ");
        return str;
    }
};
this.Common = window.Common = new Common();