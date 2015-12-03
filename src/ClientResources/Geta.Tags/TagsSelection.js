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
                readOnly: this.readOnly
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