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
        height: null
    }

    inputText.prototype.defaults = function () {
        return {
            title: this.$element.attr('title') || inputText.DEFAULTS.title,
            help: this.$element.attr('data-help') || inputText.DEFAULTS.help,
            type: this.$element.attr('data-type') || inputText.DEFAULTS.type,
            width: this.$element.attr('data-width') || inputText.DEFAULTS.width,
            height: this.$element.attr('data-height') || inputText.DEFAULTS.height
        }
    }

    inputText.prototype.render = function () {
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

        if (this.options.type == 'date') {
            this.$element.attr('value', this.$element.attr('value').replace(/(\d+).(\d+).(\d+) (\d+:\d+:\d+)/, '$1.$2.$3'));
            this.$element.attr('data-mask', '99.99.9999');
        }
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
        title: 'Title',
        help: null,
        url: null,
        name: 'Name',
        size: 'Size'

        //on: 'On',
        //off: 'Off',
        //onstyle: 'primary',
        //offstyle: 'default',
        //size: 'normal',
        //style: '',
        //width: null,
        //height: null
    }

    inputFile.prototype.defaults = function () {
        return {
            title: this.$element.attr('title') || inputFile.DEFAULTS.title,
            help: this.$element.attr('data-help') || inputFile.DEFAULTS.help,
            url: this.$element.attr('data-url') || inputFile.DEFAULTS.url,
            name: this.$element.attr('data-name') || inputFile.DEFAULTS.name,
            size: this.$element.attr('data-size') || inputFile.DEFAULTS.size

            //on: this.$element.attr('data-on') || inputFile.DEFAULTS.on,
            //off: this.$element.attr('data-off') || inputFile.DEFAULTS.off,
            //onstyle: this.$element.attr('data-onstyle') || inputFile.DEFAULTS.onstyle,
            //offstyle: this.$element.attr('data-offstyle') || inputFile.DEFAULTS.offstyle,
            //size: this.$element.attr('data-size') || inputFile.DEFAULTS.size,
            //style: this.$element.attr('data-style') || inputFile.DEFAULTS.style,
            //width: this.$element.attr('data-width') || inputFile.DEFAULTS.width,
            //height: this.$element.attr('data-height') || inputFile.DEFAULTS.height
        }
    }

    var $DelPreview;

    inputFile.prototype.render = function () {
        var $inputTitle = $('<label for="' + this.$element.prop('id') + '">').html(this.options.title);
        var $inputGroup = $('<div class="form-group fileupload">');
        this.$element.wrap($inputGroup);
        this.$element.before($inputTitle);
        //this.$element.css('opacity', '0.5');
        var $PreviewBlock = $("<div/>", { "class": "preview" });
        var $Wrap = $("<div/>", {"class" : "wrap_info"});

        //if (this.options.help && !this.options.url) {
        //    this.$element.wrap('<div class="input-group"></div>');
        //    this.$element.after('<div class="input-group-addon"><div class="icon-help-circled" data-toggle="popover" data-placement="auto bottom" data-content="' + this.options.help + '"></div></div>');

        //    $(this).next().find('div').popover();
        //}
        if (this.options.url) {
            this.$element.css('display', 'none');
            //var $PreviewBlock = $("<div/>", { "class": "preview" });

            //$PreviewBlock.append('<div class="preview_img"><img src="' + this.options.url + '" /></div>');
            $Wrap.append('<div class="preview_img"><img src="' + this.options.url + '" /></div>');
            var $InfoBlock = $("<div/>", { 'class': 'preview_info' });
            $InfoBlock.append('<div class="preview_name">' + this.options.name + '</div>');
            $InfoBlock.append('<div class="preview_size">' + 'Размер: ' + this.options.size + '</div>');

            $DelPreview = $("<div/>", { 'class': 'preview_btn' });
            $DelLink = $('<a class="preview_del">Удалить</a>');
            $DelPreview.append($DelLink);
            $InfoBlock.append($DelPreview);
            $Wrap.append($InfoBlock);


            //if (this.options.help) {
            //    var $FileHelp = $('<div class="file_help">' + this.options.help + '</div>');
            //    $PreviewBlock.append($FileHelp);
            //}
            //this.$element.after($PreviewBlock);
        } else {
            var $ButtonLoad = $('<div/>', { 'class': 'btn_load' }).append('Выберите файл');
            $Wrap.append($ButtonLoad);
            $PreviewBlock.addClass('empty_file');
        }

        $PreviewBlock.append($Wrap);

        if (this.options.help) {
            var $FileHelp = $('<div class="file_help">' + this.options.help + '</div>');
            $PreviewBlock.append($FileHelp);
        }

        this.$element.after($PreviewBlock);

        this.$element.addClass('form-control');

        //this._onstyle = 'btn-' + this.options.onstyle
        //this._offstyle = 'btn-' + this.options.offstyle
        //var size = this.options.size === 'large' ? 'btn-lg'
        //  : this.options.size === 'small' ? 'btn-sm'
        //  : this.options.size === 'mini' ? 'btn-xs'
        //  : ''
        //var $toggleOn = $('<label class="btn">').html(this.options.on)
        //  .addClass(this._onstyle + ' ' + size)
        //var $toggleOff = $('<label class="btn">').html(this.options.off)
        //  .addClass(this._offstyle + ' ' + size + ' active')
        //var $toggleHandle = $('<span class="toggle-handle btn btn-default">')
        //  .addClass(size)
        //var $toggleGroup = $('<div class="toggle-group">')
        //  .append($toggleOn, $toggleOff, $toggleHandle)
        //var $toggle = $('<div class="toggle btn" data-toggle="toggle">')
        //  .addClass(this.$element.prop('checked') ? this._onstyle : this._offstyle + ' off')
        //  .addClass(size).addClass(this.options.style)

        //this.$element.wrap($toggle)
        //$.extend(this, {
        //    $toggle: this.$element.parent(),
        //    $toggleOn: $toggleOn,
        //    $toggleOff: $toggleOff,
        //    $toggleGroup: $toggleGroup
        //})
        //this.$toggle.append($toggleGroup)
        //var width = this.options.width || Math.max($toggleOn.outerWidth(), $toggleOff.outerWidth()) + ($toggleHandle.outerWidth() / 2)
        //var height = this.options.height || Math.max($toggleOn.outerHeight(), $toggleOff.outerHeight())
        //$toggleOn.addClass('toggle-on')
        //$toggleOff.addClass('toggle-off')
        //this.$toggle.css({ width: width, height: height })
        //if (this.options.height) {
        //    $toggleOn.css('line-height', $toggleOn.height() + 'px')
        //    $toggleOff.css('line-height', $toggleOff.height() + 'px')
        //}
        //this.update(true)
        //this.trigger(true)
    }

    inputFile.prototype.change = function (silent) {
        //this.options.size = sizer(13336);
        alert(this.files.length)
        var File = this.files[0];
        this.options.name = File.name;
        //this.options. = File.type;
        this.options.size = sizer(File.size);

        alert(this.options.size);

        //alert(sizer(fileSize));
        //this.$element.attr('data-size', sizer(fileSize));
        //this.$element.size = sizer(fileSize);


        //if (this.$element.prop('disabled')) return false
        //this.$toggle.removeClass(this._offstyle + ' off').addClass(this._onstyle)
        //this.$element.prop('checked', true)
        //if (!silent) this.trigger()
    }

    inputFile.prototype.delete = function (silent) {
        //alert('ds')
        this.$element.remove()
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

    //$(document).on('click.bs.inputFile', function (e) {
    $(document).on('click.bs.inputFile', '.preview_del', function (e) {
        var $file = $(this).parent().parent()
        $file.DislyFile('delete')
        e.preventDefault()
    })

    $(document).on('change.bs.inputFile', 'input:file', function (e) {
        $(this).DislyFile('change');
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