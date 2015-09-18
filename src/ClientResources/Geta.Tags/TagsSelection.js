define([
    "dojo/_base/declare",
    "dijit/form/TextBox",
],
function (
    declare,
    TextBox) {

    return declare([TextBox], {
        postCreate: function () {
            var $domNode = $(this.domNode),
                isReadonly = $domNode.hasClass('dijitReadOnly');
            $domNode.find('input').tagit({
                autocomplete: { delay: 0, minLength: 2, source: '/getatags?groupKey=' + this.groupKey },
                readOnly: isReadonly
            });
        }
    });
});