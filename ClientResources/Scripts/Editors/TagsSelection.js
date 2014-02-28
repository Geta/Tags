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
            this.storeurl = this.storeurl || routes.getRestPath({ moduleArea: "app", storeName: "tags" });
            var store = new JsonRest(dojo.mixin({ target: this.storeurl }));
            this.set("store", store);
            // call base implementation            
            this.inherited(arguments);
        }
    });
});