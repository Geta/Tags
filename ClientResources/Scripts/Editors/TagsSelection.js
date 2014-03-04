define([
    "dojo/_base/declare",
    "dojo/store/JsonRest",
    "dojox/form/MultiComboBox",
    "epi/routes"
],
function (
    declare,
    JsonRest,
    MultiComboBox,
    routes) {

    return declare([MultiComboBox], {
        postMixInProperties: function () {
            var store = new JsonRest(dojo.mixin({ target: '/api/tags' }));
            this.set("store", store);
            // call base implementation            
            this.inherited(arguments);
        },
        _setValueAttr: function (value) {
            value = value || '';
            if (this.delimiter && value.length != 0) {
                value = value + this.delimiter + " ";
                arguments[0] = this._addPreviousMatches(value);
            }
            this.inherited(arguments);
        }
    });
});