define([
    "dojo/_base/declare",
    "dijit/form/TextBox"
],
function (
    declare,
    TextBox
) {
    return declare([TextBox], {

        _tagWidget: null,

        postCreate: function () {
            this.inherited(arguments);
            this._createTags();
        },

        destroy: function() {
            this._destroyTags();
            this.inherited(arguments);
        },

        _createTags: function () {
            this._destroyTags();
            this._tagWidget = $(this.textbox).tagit({
                autocomplete: { delay: 0, minLength: 2, source: '/getatags?groupKey=' + this.groupKey },
                allowSpaces: this.allowSpaces,
                allowDuplicates: this.allowDuplicates,
                caseSensitive: this.caseSensitive,
                readOnly: this.readOnly,
                beforeTagAdded: function () {
                    this.onFocus();
                }.bind(this),
                afterTagAdded: function () {
                    if (this._tagWidget) {
                        var value = this._tagWidget.val();
                        this._set("value", value);
                        this.onChange(value);
                    }
                }.bind(this)
            });
        },

        _destroyTags: function() {
            this._tagWidget && this._tagWidget.tagit("destroy");
            this._tagWidget = null;
        },

        _setValueAttr: function (value, priorityChange) {
            this.inherited(arguments);
            this._started && !priorityChange && this._createTags();
        }
    });
});