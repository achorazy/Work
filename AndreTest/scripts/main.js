define(["require", "exports", "TFS/Build/RestClient", "VSS/Service"], function (require, exports, RestClient_1, Service_1) {
    "use strict";
    var andreTestMenu = (function () {
        "use strict";
        return {
            execute: function (actionContext) {
                var vsoContext = VSS.getWebContext();
                var buildClient = Service_1.getCollectionClient(RestClient_1.BuildHttpClient);
                VSS.ready(function () {
                    // get the build
                    buildClient.getBuild(actionContext.id, vsoContext.project.name).then(function (build) {
                        // and queue it again
                        alert("Andre Test... ");
                        alert("Build info: " + build.buildNumber);
                        //buildClient.queueBuild(build, build.definition.project.id).then(newBuild => {
                        //	// and navigate to the build summary page
                        //	// e.g. https://myproject.visualstudio.com/DefaultCollection/someproject/_BuildvNext#_a=summary&buildId=1347
                        //	var buildPageUrl = `${vsoContext.host.uri}/${vsoContext.project.name}/_BuildvNext#_a=summary&buildId=${newBuild.id}`;
                        //	window.parent.location.href = buildPageUrl;
                        //});
                    });
                });
            }
        };
    }());
    VSS.register("andreTestMenu", andreTestMenu);
});
