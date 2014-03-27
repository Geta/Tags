define([
    "dojo/_base/declare",
    "dojo/store/JsonRest",
    "dijit/form/TextBox"
],
function (
    declare,
    JsonRest,
    TextBox) {

    return declare([TextBox], {
        postCreate: function() {
            $(this.domNode).find('input').tagit({
                autocomplete: { delay: 0, minLength: 2, source: '/getatags' }
            });
        }
    });
});