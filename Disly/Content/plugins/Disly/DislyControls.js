/*! ========================================================================
 * DislyControls: inputText.js v3.3.5
  * ========================================================================
 * Copyright 2017-201 it-serv, D-Boriskin.
 * ======================================================================== */
+function ($) {

    // ucFile PUBLIC CLASS DEFINITION
    // ==============================

    var inputText = function (element, options) {
        this.$element = $(element)
        this.options = $.extend({}, this.defaults(), options)
        this.render()
    }

    inputText.DEFAULTS = {
        title: null,
        help: null,
        type: 'text',
        width: null,
        height: null,
        required: null
    }

    inputText.prototype.defaults = function () {
        return {
            title: this.$element.attr('title') || inputText.DEFAULTS.title,
            help: this.$element.attr('data-help') || inputText.DEFAULTS.help,
            type: this.$element.attr('data-type') || inputText.DEFAULTS.type,
            width: this.$element.attr('data-width') || inputText.DEFAULTS.width,
            height: this.$element.attr('data-height') || inputText.DEFAULTS.height,
            required: this.$element.attr('required') || inputText.DEFAULTS.required
        }
    }

    inputText.prototype.render = function () {
        this.$element.wrap('<div class="form-group">');

        if (this.options.title) {
            var $toggleTitle = $('<label for="' + this.$element.attr('id') + '">').html(this.options.title + ':');
            this.$element.before($toggleTitle);
        }

        if (this.options.type == 'date' || this.options.type == 'datetime' || this.options.help) {
            this.$element.wrap('<div class="input-group"></div>');
        }

        if (this.options.help) {
            this.$element.after('<div class="input-group-addon"><div class="icon-help-circled" data-toggle="popover" data-placement="auto bottom" data-content="' + this.options.help + '"></div></div>');
            this.$element.next().find('div').popover();
        }


        if (this.options.type == 'date' || this.options.type == 'datetime')
        {
            var $InputTime = $('<input style="width:70px;" class="form-control" placeholder="00:00">')
            $InputTime.attr('value', this.$element.attr('value').replace(/(\d+).(\d+).(\d+) (\d+):(\d+):(\d+)/, '$4:$5'));
            $InputTime.mask('Hh:Mm', {
                'translation': {
                    H: { pattern: /[0-2]/ },
                    h: { pattern: /[0-9]/ },
                    M: { pattern: /[0-5]/ },
                    m: { pattern: /[0-9]/ }
                },
                'placeholder:': '00:00'
            });

            //$InputTime.attr('data-mask', '99:99');

            var $InputDate = $('<input data-type="date" class="form-control" value="">');
            $InputDate.attr('value', this.$element.attr('value').replace(/(\d+).(\d+).(\d+) (\d+:\d+:\d+)/, '$1.$2.$3'));
            $InputDate.attr('data-mask', '99.99.9999');


            if (this.$element.attr('required') == 'required') {
                $InputDate.attr('required', 'required');
            }


            this.$element.hide();
            this.$element.after($InputTime);
            this.$element.after($InputDate);
            
            if (this.options.type == 'date') {
                $InputTime.hide();
            }
            else {
                $InputTime.wrap('<div class="input-group-addon"><div></div></div>');
            }
            var $TargetInput = this.$element;

            $InputDate.keyup(function () {
                SpotDate();                
            });
            $InputTime.keyup(function () {                
                SpotDate();
            });
            $InputDate.datepicker()
                .on("input change", function (e) {
                    SpotDate();
                });
            function SpotDate() {
                var time = $InputTime.val();
                var Length = time.length;
                switch (Length) {                    
                    case (5): time +=       ':00'; break;
                    case (4): time +=      '0:00'; break;
                    case (3): time +=     '00:00'; break;
                    case (2): time +=    ':00:00'; break;
                    case (1): time +=   '0:00:00'; break;
                    default:  time +=  '00:00:00'; break;
                }               
                $TargetInput.attr('value', $InputDate.val() + ' ' + time); 
            }
        }
        this.$element.addClass('form-control');
    }

    //inputText.prototype.destroy = function () {
    //    this.$element.removeData('bs.inputText')
    //    this.$element.unwrap()
    //}

    // inputText PLUGIN DEFINITION
    // ========================

    function Plugin(option) {
        return this.each(function () {
            var $this = $(this)
            var data = $this.data('bs.inputText')
            var options = typeof option == 'object' && option

            if (!data) $this.data('bs.inputText', (data = new inputText(this, options)))
            if (typeof option == 'string' && data[option]) data[option]()
        })
    }

    var old = $.fn.DislyInput

    $.fn.DislyInput = Plugin
    $.fn.DislyInput.Constructor = inputText

    // TOGGLE NO CONFLICT
    // ==================

    $.fn.toggle.noConflict = function () {
        $.fn.DislyInput = old
        return this
    }

    // DislyFile DATA-API
    // ==================

    $(function () {
        $('input:not([type=checkbox]):not([type=file]):visible, textarea:visible').DislyInput()
    })

}(jQuery);

/*! ========================================================================
 * DislyControls: inputFile.js v3.3.5
  * ========================================================================
 * Copyright 2017-201 it-serv, D-Boriskin.
 * ======================================================================== */
+function ($) {

    // ucFile PUBLIC CLASS DEFINITION
    // ==============================

    var inputFile = function (element, options) {
        this.$element = $(element)
        this.options = $.extend({}, this.defaults(), options)
        this.render()
    }

    inputFile.DEFAULTS = {
        title: 'Выбор файла',
        help: null,
        url: null,
        name: 'Name',
        size: 'Size'
    }

    inputFile.prototype.defaults = function () {
        return {
            title: this.$element.attr('title') || inputFile.DEFAULTS.title,
            help: this.$element.attr('data-help') || inputFile.DEFAULTS.help,
            url: this.$element.attr('data-url') || inputFile.DEFAULTS.url,
            name: this.$element.attr('data-name') || inputFile.DEFAULTS.name,
            size: this.$element.attr('data-size') || inputFile.DEFAULTS.size
        }
    }

    var $DelPreview;

    inputFile.prototype.render = function () {
        var $inputTitle = $('<label for="' + this.$element.prop('id') + '">').html(this.options.title);
        var $inputGroup = $('<div class="form-group fileupload">');
        this.$element.wrap($inputGroup);
        this.$element.before($inputTitle);

        if (this.options.url != null) {
            var $HiddenInput = $('<input type="hidden" id="' + this.$element.prop('id') + '" value=""/>');
            $HiddenInput.attr('value', this.options.url);
            this.$element.after($HiddenInput);
        }

        this.create()
    }

    inputFile.prototype.change = function (silent) {
    }

    inputFile.prototype.delete = function () {
        this.$element.removeAttr('data-url');
    }

    inputFile.prototype.create = function () {
        var $PreviewBlock = $("<div/>", { "class": "preview" });
        var $Wrap = $("<div/>", { "class": "wrap_info" });
        this.$element.css('display', 'none');

        if (this.$element.attr('data-url')) {
            $Wrap.append('<div class="preview_img"><img src="' + this.$element.attr('data-url') + '" /></div>');
            var $InfoBlock = $("<div/>", { 'class': 'preview_info' });
            $InfoBlock.append('<div class="preview_name">' + this.$element.attr('data-name') + '</div>');
            $InfoBlock.append('<div class="preview_size">' + 'Размер: ' + this.$element.attr('data-size') + '</div>');

            $DelPreview = $("<div/>", { 'class': 'preview_btn' });
            $DelLink = $('<a class="preview_del button icon-delete" data-action="noPreloader-accept">Удалить</a>');
            $DelPreview.append($DelLink);
            $InfoBlock.append($DelPreview);
            $Wrap.append($InfoBlock);

        } else {
            var $ButtonLoad = $('<div/>', { 'class': 'btn_load' }).append('Выберите файл');
            $Wrap.append($ButtonLoad);
        }

        $PreviewBlock.append($Wrap);

        if (this.$element.attr('data-help')) {
            $Wrap.addClass('col-md-12');
            var $FileHelp = $('<div class="file_help col-md-12">' + this.$element.attr('data-help') + '</div>');
            $PreviewBlock.append($FileHelp);
        }

        this.$element.after($PreviewBlock);

        this.$element.addClass('form-control');
    }

    // ucFile PLUGIN DEFINITION
    // ========================

    function Plugin(option) {
        return this.each(function () {
            var $this = $(this)
            var data = $this.data('bs.inputFile')
            var options = typeof option == 'object' && option

            if (!data) $this.data('bs.inputFile', (data = new inputFile(this, options)))
            if (typeof option == 'string' && data[option]) data[option]()
        })
    }

    var old = $.fn.DislyFile

    $.fn.DislyFile = Plugin
    $.fn.DislyFile.Constructor = inputFile

    // TOGGLE NO CONFLICT
    // ==================

    $.fn.DislyFile.noConflict = function () {
        $.fn.DislyFile = old
        return this
    }

    // DislyFile DATA-API
    // ==================

    $(function () {
        $('input[type=file]:visible').DislyFile()
    })

    $(document).on('click.bs.inputFile', '.preview_del', function (e) {
        var $file = $(this).parents('.fileupload').find('input:file');

        $file.removeAttr('data-url');
        $file.removeAttr('data-size');
        $file.removeAttr('data-name');
        $file.removeAttr('value');

        $file.siblings('.preview').remove();

        $file.siblings('input[type=hidden]#' + $file.attr('id')).removeAttr('value');
        $('.fileupload').siblings('input[type=hidden]').removeAttr('value');

        $file.DislyFile('create');
    })


    $(document).on('change.bs.inputFile', 'input:file', function (e) {
        var File = $(this)[0].files[0];
        var fileName = File.name;
        var fileType = File.type;
        var fileSize = File.size;
        $(this).attr('data-size', sizer(fileSize));

        if (fileType.indexOf('image') > -1) {
            $(this).attr('data-url', window.URL.createObjectURL(File));
        }

        $(this).attr('data-name', fileName);
        $(this).attr('name', 'upload');
        $(this).siblings('.preview').remove();
        $(this).DislyFile('create');
    })

    $(document).on('click.bs.inputFile', '.btn_load', function () {
        var label = $(this).parent().parent().siblings('label');
        label.trigger("click");
    })

    function sizer(size) {
        var resultSize = Math.floor(size).toString() + ' b';
        if (size > 1073741824) {
            resultSize = roundDown((size / 1024), 2).toString() + ' Gb'
        } else if (size > 1048576) {
            resultSize = roundDown((size / 1048576), 2).toString() + ' mb';
        } else if (size > 1024) {
            resultSize = roundDown((size / 1024), 2).toString() + ' kb';
            return resultSize;
        } else {
            resultSize;
        }
    }

    function roundDown(number, decimals) {
        decimals = decimals || 0;
        return (Math.floor(number * Math.pow(10, decimals)) / Math.pow(10, decimals));
    }

}(jQuery);

/*! ========================================================================
 * DislyControls: inputFile.js v3.3.5
  * ========================================================================
 * Copyright 2017-201 it-serv, D-Boriskin.
 * ======================================================================== */
+function ($) {

    // ucFile PUBLIC CLASS DEFINITION
    // ==============================

    var select = function (element, options) {
        this.$element = $(element)
        this.options = $.extend({}, this.defaults(), options)
        this.render()
    }

    select.DEFAULTS = {
        title: null,
        help: null,
        width: null,
        height: null
    }

    select.prototype.defaults = function () {
        return {
            title: this.$element.attr('title') || select.DEFAULTS.title,
            help: this.$element.attr('data-help') || select.DEFAULTS.help,
            width: this.$element.attr('data-width') || select.DEFAULTS.width,
            height: this.$element.attr('data-height') || select.DEFAULTS.height
        }
    }

    select.prototype.render = function () {
        this.$element.wrap('<div class="form-group">');

        if (this.options.title) {
            var $toggleTitle = $('<label for="' + this.$element.attr('id') + '">').html(this.options.title + ':');
            this.$element.before($toggleTitle);
        }
        if (this.options.help) {
            this.$element.wrap('<div class="input-group"></div>');
            this.$element.after('<div class="input-group-addon"><div class="icon-help-circled" data-toggle="popover" data-placement="auto bottom" data-content="' + this.options.help + '"></div></div>');

            this.$element.next().find('div').popover();
        }

        this.$element.addClass('form-control');
    }

    // ucFile PLUGIN DEFINITION
    // ========================

    function Plugin(option) {
        return this.each(function () {
            var $this = $(this)
            var data = $this.data('bs.select')
            var options = typeof option == 'object' && option

            if (!data) $this.data('bs.select', (data = new select(this, options)))
            if (typeof option == 'string' && data[option]) data[option]()
        })
    }

    var old = $.fn.DislySelect

    $.fn.DislySelect = Plugin
    $.fn.DislySelect.Constructor = select

    // TOGGLE NO CONFLICT
    // ==================

    $.fn.toggle.noConflict = function () {
        $.fn.DislySelect = old
        return this
    }

    // DislyFile DATA-API
    // ==================

    $(function () {
        //$('select').DislySelect()
    })

}(jQuery);