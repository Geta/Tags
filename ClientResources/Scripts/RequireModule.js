define([
    "dojo",
//CMS
    "epi/dependency"
], function (
    dojo,
//CMS
    dependency
) {
    return {
        load: function (/*String*/id, /*function*/require, /*function*/load) {
            var moduleManager = dependency.resolve("epi.ModuleManager");
            dojo.when(moduleManager.startModules(id), function () {
                return load(moduleManager.getModule(id));
            });
        }
    };
});